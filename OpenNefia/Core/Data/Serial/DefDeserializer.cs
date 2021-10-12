using Love;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data.Serial
{
    public class DefDeserializer : IDefDeserializer
    {
        internal List<string> Errors;

        internal List<DefCrossRef> CrossRefs { get; }

        internal DefDeserializer()
        {
            this.Errors = new List<string>();
            this.CrossRefs = new List<DefCrossRef>();
        }

        public Def? DeserializeDef(Type defType, XmlNode node, Type containingModType)
        {
            var id = node.Attributes?["Id"]?.Value;

            if (id == null)
                throw new DefLoadException($"Def has no ID.");

            var className = node.Attributes?["Class"]?.Value;

            if (className != null)
            {
                var klassType = Type.GetType(className);
                if (klassType == null)
                {
                    this.Errors.Add($"Could not find class with name '{className}' (parent: '{defType.FullName}')");
                    return null;
                }
                else if (!klassType.IsSubclassOf(defType))
                {
                    this.Errors.Add($"Class '{klassType}' is not a subclass of parent def type '{defType.FullName}'");
                    return null;
                }
                else
                {
                    defType = klassType;
                }
            }

            var defInstance = (Def)Activator.CreateInstance(defType, id)!;

            PopulateAllFields(node, defInstance, containingModType);

            if (this.Errors.Count > 0)
            {
                var errors = "";
                foreach (var error in this.Errors)
                {
                    errors += $"{error}\n";
                }
                throw new Exception($"Error loading def {defType.Name}:{id} ({this.Errors.Count} errors)\n{errors}");
            }

            return defInstance;
        }

        private bool IsRequired(PropertyInfo prop)
        {
            return Nullable.GetUnderlyingType(prop.PropertyType) != null;
        }

        public void PopulateAllFields(XmlNode node, object target, Type containingModType)
        {
            foreach (var prop in target.GetType().GetProperties())
            {
                PopulateFieldByName(prop.Name, node, target, containingModType);
            }
        }

        public void PopulateFieldByName(string name, XmlNode node, object target, Type containingModType)
        {
            var prop = target.GetType().GetProperty(name)!;
            if (prop.GetCustomAttribute<DefSerialUseAttributeAttribute>() != null)
            {
                if (node.Attributes?[prop.Name] != null)
                {
                    PopulateField(node.Attributes[prop.Name]!, target, prop, containingModType);
                }
            }
            else
            {
                if (node[prop.Name] != null)
                {
                    PopulateField(node[prop.Name]!, target, prop, containingModType);
                }
                //
                // TODO
                //
                //else if (!IsNullable(prop))
                //{
                //    Errors.Add($"Required def property {prop.Name} ({prop.GetType().Name}) was not provided.");
                //}
            }
        }

        public void PopulateFieldByNode(string name, XmlNode node, object target, Type containingModType)
        {
            var prop = target.GetType().GetProperty(name)!;
            PopulateField(node, target, prop, containingModType);
        }

        public void PopulateField(XmlNode childNode, object target, PropertyInfo prop, Type containingModType)
        {
            var value = childNode.InnerText!;
            var typeCode = Type.GetTypeCode(prop.PropertyType);

            switch (typeCode)
            {
                case TypeCode.Boolean:
                    prop.SetValue(target, bool.Parse(value));
                    break;
                case TypeCode.Char:
                    prop.SetValue(target, char.Parse(value));
                    break;
                case TypeCode.SByte:
                    prop.SetValue(target, sbyte.Parse(value));
                    break;
                case TypeCode.Byte:
                    prop.SetValue(target, byte.Parse(value));
                    break;
                case TypeCode.Int16:
                    prop.SetValue(target, short.Parse(value));
                    break;
                case TypeCode.UInt16:
                    prop.SetValue(target, ushort.Parse(value));
                    break;
                case TypeCode.Int32:
                    prop.SetValue(target, int.Parse(value));
                    break;
                case TypeCode.UInt32:
                    prop.SetValue(target, uint.Parse(value));
                    break;
                case TypeCode.Int64:
                    prop.SetValue(target, long.Parse(value));
                    break;
                case TypeCode.UInt64:
                    prop.SetValue(target, ulong.Parse(value));
                    break;
                case TypeCode.Single:
                    prop.SetValue(target, float.Parse(value));
                    break;
                case TypeCode.Double:
                    prop.SetValue(target, double.Parse(value));
                    break;
                case TypeCode.Decimal:
                    prop.SetValue(target, decimal.Parse(value));
                    break;
                case TypeCode.DateTime:
                    prop.SetValue(target, DateTime.Parse(value));
                    break;
                case TypeCode.String:
                    prop.SetValue(target, value);
                    break;

                case TypeCode.Object:
                    PopulateObjectField(childNode, target, prop, containingModType);
                    break;
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    this.Errors.Add($"Unsupported typecode '{typeCode}' for type '{target.GetType()}', property '{prop.Name}' ({prop.PropertyType})");
                    break;
            }
        }

        private IResourcePath ParseModLocalPath(string value, Type containingModType) => new ModLocalPath(containingModType, value);

        private Love.Color ParseLoveColor(XmlNode childNode)
        {
            var color = new Love.Color();

            var r = childNode["R"];
            if (r != null)
                color.r = byte.Parse(r.InnerText!);
            var g = childNode["G"];
            if (g != null)
                color.g = byte.Parse(g.InnerText!);
            var b = childNode["B"];
            if (b != null)
                color.b = byte.Parse(b.InnerText!);
            var a = childNode["A"];
            if (a != null)
                color.a = byte.Parse(a.InnerText!);

            return color;
        }

        private void PopulateObjectField(XmlNode childNode, object target, PropertyInfo prop, Type containingModType)
        {
            var value = childNode.InnerText!;
            var ty = prop.PropertyType;

            if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                prop.SetValue(target, path);
            }
            else if (ty == typeof(Love.Color))
            {
                Love.Color color = ParseLoveColor(childNode);
                prop.SetValue(target, color);
            }
            else if (ty.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefCrossRef(target, prop, ty, value);
                this.CrossRefs.Add(crossRef);
            }
            else if (ty.IsEnum)
            {
                if (Enum.IsDefined(ty, value))
                {
                    var parsed = Enum.Parse(ty, value);
                    prop.SetValue(target, parsed);
                }
                else
                {
                    this.Errors.Add($"Enum '{prop.PropertyType}' does not have variant '{value}' (on property '{prop.Name}')");
                }
            }
            else if (typeof(IDefSerializable).IsAssignableFrom(ty))
            {
                var fieldInstance = (IDefSerializable)Activator.CreateInstance(ty)!;
                fieldInstance.DeserializeDefField(this, childNode, containingModType);
                PopulateAllFields(childNode, fieldInstance, containingModType);
                prop.SetValue(target, fieldInstance);
            }
            else
            {
                this.Errors.Add($"Cannot set property '{prop.Name}' of type '{prop.PropertyType}'");
            }
        }
    }
}