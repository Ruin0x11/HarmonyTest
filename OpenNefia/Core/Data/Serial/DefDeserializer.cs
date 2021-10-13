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

        public Def? DeserializeDef(Type defType, XmlNode node, ModInfo containingMod)
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

            var fullId = $"{containingMod.Metadata.Name}.{id}";

            var defInstance = (Def)Activator.CreateInstance(defType, fullId)!;
            defInstance.Mod = (BaseMod)containingMod.Instance!;

            var elonaId = node.Attributes?["ElonaId"]?.Value;

            if (elonaId != null)
                defInstance.ElonaId = int.Parse(elonaId);

            PopulateAllFields(node, defInstance, containingMod.GetType());

            if (this.Errors.Count > 0)
            {
                var errors = "";
                foreach (var error in this.Errors)
                {
                    errors += $"{error}\n";
                }
                throw new Exception($"Error loading def {fullId} ({this.Errors.Count} errors)\n{errors}");
            }

            return defInstance;
        }

        public void PopulateAllFields(XmlNode node, object target, Type containingModType)
        {
            foreach (var field in target.GetType().GetFields())
            {
                PopulateFieldByName(field.Name, node, target, containingModType);
            }
        }

        private void CheckRequiredValue(object target, FieldInfo field)
        {
            var required = field.GetCustomAttribute<DefRequiredAttribute>();
            if (required != null)
            {
                if (required.DefaultValue != null)
                {
                    field.SetValue(target, required.DefaultValue);
                }
                else
                {
                    Errors.Add($"Required def field {field.Name} ({field.FieldType.Name}) was not provided.");
                }
            }
        }

        public void PopulateFieldByName(string name, XmlNode node, object target, Type containingModType)
        {
            var field = target.GetType().GetField(name)!;
            if (field.GetCustomAttribute<DefUseAttributesAttribute>() != null)
            {
                if (node.Attributes?[field.Name] != null)
                {
                    PopulateField(node.Attributes[field.Name]!, target, field, containingModType);
                }
                else
                {
                    CheckRequiredValue(target, field);
                }
            }
            else
            {
                if (node[field.Name] != null)
                {
                    PopulateField(node[field.Name]!, target, field, containingModType);
                }
                else
                {
                    CheckRequiredValue(target, field);
                }
            }
        }

        public void PopulateFieldByNode(string name, XmlNode node, object target, Type containingModType)
        {
            var field = target.GetType().GetField(name)!;
            PopulateField(node, target, field, containingModType);
        }

        public void PopulateField(XmlNode childNode, object target, FieldInfo field, Type containingModType)
        {
            if (field.GetCustomAttribute<DefIgnoredAttribute>() != null)
                return;

            var value = childNode.InnerText!;
            var typeCode = Type.GetTypeCode(field.FieldType);

            if (!field.FieldType.IsValueType)
            {
                PopulateObjectField(childNode, target, field, containingModType);
            }
            else if (field.FieldType.IsEnum)
            {
                if (Enum.IsDefined(field.FieldType, value))
                {
                    var parsed = Enum.Parse(field.FieldType, value);
                    field.SetValue(target, parsed);
                }
                else
                {
                    this.Errors.Add($"Enum '{field.FieldType}' does not have variant '{value}' (on field '{field.Name}')");
                }
            }
            else
            {
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        field.SetValue(target, bool.Parse(value));
                        break;
                    case TypeCode.Char:
                        field.SetValue(target, char.Parse(value));
                        break;
                    case TypeCode.SByte:
                        field.SetValue(target, sbyte.Parse(value));
                        break;
                    case TypeCode.Byte:
                        field.SetValue(target, byte.Parse(value));
                        break;
                    case TypeCode.Int16:
                        field.SetValue(target, short.Parse(value));
                        break;
                    case TypeCode.UInt16:
                        field.SetValue(target, ushort.Parse(value));
                        break;
                    case TypeCode.Int32:
                        field.SetValue(target, int.Parse(value));
                        break;
                    case TypeCode.UInt32:
                        field.SetValue(target, uint.Parse(value));
                        break;
                    case TypeCode.Int64:
                        field.SetValue(target, long.Parse(value));
                        break;
                    case TypeCode.UInt64:
                        field.SetValue(target, ulong.Parse(value));
                        break;
                    case TypeCode.Single:
                        field.SetValue(target, float.Parse(value));
                        break;
                    case TypeCode.Double:
                        field.SetValue(target, double.Parse(value));
                        break;
                    case TypeCode.Decimal:
                        field.SetValue(target, decimal.Parse(value));
                        break;
                    case TypeCode.DateTime:
                        field.SetValue(target, DateTime.Parse(value));
                        break;
                    case TypeCode.String:
                        field.SetValue(target, value);
                        break;

                    case TypeCode.Object:
                        PopulateObjectField(childNode, target, field, containingModType);
                        break;
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        this.Errors.Add($"Unsupported typecode '{typeCode}' for type '{target.GetType()}', field '{field.Name}' ({field.FieldType})");
                        break;
                }
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

        private void PopulateObjectField(XmlNode childNode, object target, FieldInfo field, Type containingModType)
        {
            var value = childNode.InnerText!;
            var ty = field.FieldType;

            if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                field.SetValue(target, path);
            }
            else if (ty == typeof(Love.Color))
            {
                Love.Color color = ParseLoveColor(childNode);
                field.SetValue(target, color);
            }
            else if (ty.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefCrossRef(target, field, ty, value);
                this.CrossRefs.Add(crossRef);
            }
            else if (ty.IsEnum)
            {
                if (Enum.IsDefined(ty, value))
                {
                    var parsed = Enum.Parse(ty, value);
                    field.SetValue(target, parsed);
                }
                else
                {
                    this.Errors.Add($"Enum '{field.FieldType}' does not have variant '{value}' (on field '{field.Name}')");
                }
            }
            else if (typeof(IDefSerializable).IsAssignableFrom(ty))
            {
                var fieldInstance = (IDefSerializable)Activator.CreateInstance(ty)!;
                fieldInstance.DeserializeDefField(this, childNode, containingModType);
                PopulateAllFields(childNode, fieldInstance, containingModType);
                fieldInstance.ValidateDefField(this.Errors);
                field.SetValue(target, fieldInstance);
            }
            else
            {
                this.Errors.Add($"Cannot set field '{field.Name}' of type '{field.FieldType}'");
            }
        }
    }
}