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
    internal class DefDeserializer
    {
        private List<string> Errors;

        internal DefDeserializer()
        {
            this.Errors = new List<string>();
        }

        public Def DeserializeDef(Type defType, XmlNode node, Type containingModType)
        {
            this.Errors.Clear();

            var id = node.Attributes?["Id"]!.Value;

            if (id == null)
                throw new DefLoadException($"Def has no ID.");

            var defInstance = (Def)Activator.CreateInstance(defType, id)!;

            PopulateDef(node, defInstance, defType, containingModType);

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

        private void PopulateDef(XmlNode node, Def target, Type defType, Type containingModType)
        {
            var map = new Dictionary<string, XmlNode>(StringComparer.OrdinalIgnoreCase);

            foreach (var child in node.ChildNodes)
            {
                var childNode = (XmlNode)child;
                var propertyName = childNode.Name;

                if (map.ContainsKey(propertyName))
                    this.Errors.Add($"Overwriting existing def property '{propertyName}'");

                map[propertyName] = (XmlNode)child;
            }

            foreach (var prop in defType.GetProperties())
            {
                if (map.TryGetValue(prop!.Name, out XmlNode? childNode))
                {
                    PopulateField(childNode, target, prop, containingModType);
                }
            }
        }

        private void PopulateField(XmlNode childNode, Def target, PropertyInfo prop, Type containingModType)
        {
            var value = childNode.Value!;
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

        private void PopulateObjectField(XmlNode childNode, Def target, PropertyInfo prop, Type containingModType)
        {
            var value = childNode.InnerText!;
            var ty = prop.PropertyType;

            if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                prop.SetValue(target, path);
            }
            else
            {
                this.Errors.Add($"Cannot set property '{prop.Name}' of type '{prop.PropertyType}'");
            }
        }
    }
}