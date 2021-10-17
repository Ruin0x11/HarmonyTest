using FluentResults;
using Love;
using OpenNefia.Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;

namespace OpenNefia.Core.Data.Serial
{
    public class DefDeserializer : IDefDeserializer
    {
        internal List<string> Errors;

        internal List<IDefCrossRef> CrossRefs { get; }

        internal DefDeserializer()
        {
            this.Errors = new List<string>();
            this.CrossRefs = new List<IDefCrossRef>();
        }

        /// <summary>
        /// Parses a def ID and type from an XML node.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public static Result<DefIdentifier> GetDefIdAndTypeFromNode(XmlNode node)
        {
            var defType = DefTypes.GetDefTypeFromName(node.Name);

            if (defType == null)
            {
                return Result.Fail($"Def type '{node.Name}' not found.");
            }

            var defId = node.Attributes?["Id"]?.Value;

            if (defId == null)
            {
                return Result.Fail($"Def has no ID.");
            }

            return Result.Ok(new DefIdentifier(defType, defId));
        }

        /// <summary>
        /// Parses a def ID and type from an XML node.
        /// </summary>
        /// <param name="nav"></param>
        /// <returns></returns>
        public static Result<DefIdentifier> GetDefIdAndTypeFromNode(XPathNavigator nav)
        {
            var defType = DefTypes.GetDefTypeFromName(nav.Name);

            if (defType == null)
            {
                return Result.Fail($"Def type '{nav.Name}' not found.");
            }

            var defId = nav.GetAttribute("Id", string.Empty);

            if (defId == null)
            {
                return Result.Fail($"Def has no ID.");
            }

            return Result.Ok(new DefIdentifier(defType, defId));
        }

        public static bool IsValidDefNode(XmlNode node) => GetDefIdAndTypeFromNode(node).IsSuccess;
        public static bool IsValidDefNode(XPathNavigator node) => GetDefIdAndTypeFromNode(node).IsSuccess;

        public Result<Type> GetConcreteTypeFromNode(XmlNode node, Type baseType)
        {
            var className = node.Attributes?["Class"]?.Value;

            if (className == null)
            {
                return Result.Ok(baseType);
            }

            var klassType = Type.GetType(className);

            if (klassType == null)
            {
                return Result.Fail($"Could not find class with name '{className}' (parent: '{baseType.FullName}')");
            }
            else if (!klassType.IsSubclassOf(baseType))
            {
                return Result.Fail($"Class '{klassType}' is not a subclass of parent type '{baseType.FullName}'");
            }
            else
            {
                return Result.Ok(klassType);
            }
        }

        public Result<Def> DeserializeDef(XmlNode node, ModInfo containingMod)
        {
            var (defBaseType, defId) = GetDefIdAndTypeFromNode(node).Value;

            var defTypeResult = GetConcreteTypeFromNode(node, defBaseType);

            if (defTypeResult.IsFailed)
            {
                return defTypeResult.ToResult<Def>();
            }

            var fullId = $"{containingMod.Metadata.Name}.{defId}";
            if (defId.Contains(".") || defId.Contains(":"))
            {
                throw new Exception("Def ID cannot contain period or colon");
            }
            node.Attributes!["Id"]!.Value = fullId;

            var defInstance = (Def)Activator.CreateInstance(defTypeResult.Value, fullId)!;
            defInstance.Mod = containingMod;

            var elonaId = node.Attributes?["ElonaId"]?.Value;

            if (elonaId != null)
                defInstance.ElonaId = int.Parse(elonaId);

            defInstance.DeserializeDefField(this, node, containingMod.GetType());

            if (this.Errors.Count > 0)
            {
                var errors = "";
                foreach (var error in this.Errors)
                {
                    errors += $"{error}\n";
                }
                throw new Exception($"Error loading def {fullId} ({this.Errors.Count} errors)\n{errors}");
            }

            defInstance.OriginalXml = node;

            return Result.Ok(defInstance);
        }

        private static string GetTargetXmlNodeName(FieldInfo field)
        {
            var nameAttrib = field.GetCustomAttribute<DefSerialNameAttribute>();
            if (nameAttrib != null)
            {
                return nameAttrib.Name;
            }

            return field.Name;
        }

        public void PopulateAllFields(XmlNode node, object target, Type containingModType)
        {
            foreach (var field in target.GetType().GetFields())
            {
                string nodeName = GetTargetXmlNodeName(field);
                PopulateFieldInfo(field, nodeName, node, target, containingModType);
            }
        }

        private void CheckRequiredValue(object target, string nodeName, FieldInfo field)
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
                    Errors.Add($"Required def field {field.Name} (name: {nodeName}) ({field.FieldType.Name}) was not provided.");
                }
            }
        }

        public void PopulateFieldByName(string name, XmlNode node, object target, Type containingModType)
        {
            var field = target.GetType().GetField(name)!;
            PopulateFieldInfo(field, field.Name, node, target, containingModType);
        }

        private void PopulateFieldInfo(FieldInfo field, string nodeName, XmlNode node, object target, Type containingModType)
        {
            if (field.GetCustomAttribute<DefUseAttributesAttribute>() != null)
            {
                if (node.Attributes?[nodeName] != null)
                {
                    PopulateField(node.Attributes[nodeName]!, target, field, containingModType);
                }
                else
                {
                    CheckRequiredValue(target, nodeName, field);
                }
            }
            else
            {
                if (node[field.Name] != null)
                {
                    PopulateField(node[nodeName]!, target, field, containingModType);
                }
                else
                {
                    CheckRequiredValue(target, nodeName, field);
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

        private Result<object> GetObject(XmlNode node, Type ty, Type containingModType)
        {
            var value = node.InnerText!;

            if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                return Result.Ok((object)path);
            }
            else if (ty == typeof(Love.Color))
            {
                Love.Color color = ParseLoveColor(node);
                return Result.Ok((object)color);
            }
            else if (ty.IsEnum)
            {
                if (Enum.IsDefined(ty, value))
                {
                    var parsed = Enum.Parse(ty, value);
                    return Result.Ok((object)parsed);
                }
                else
                {
                    return Result.Fail($"Enum '{ty}' does not have variant '{value}'");
                }
            }
            else if (typeof(IDefSerializable).IsAssignableFrom(ty))
            {
                var fieldInstance = (IDefSerializable)Activator.CreateInstance(ty)!;
                fieldInstance.DeserializeDefField(this, node, containingModType);
                PopulateAllFields(node, fieldInstance, containingModType);
                fieldInstance.ValidateDefField(this.Errors);
                return Result.Ok((object)fieldInstance);
            }
            else if (ty.IsGenericType)
            {
                if (ty.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listTy = ty.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(ty)!;

                    foreach (var childNode in node.ChildNodes.Cast<XmlNode>())
                    {
                        if (childNode.Name != "li" || childNode.ChildNodes.Count != 1)
                        {
                            return Result.Fail($"Generic list entries must have nodes named 'li' with one element only (type {ty}");
                        }
                        else
                        {
                            list.Add(GetObject(childNode.ChildNodes[0]!, listTy, containingModType));
                        }
                    }

                    return Result.Ok((object)list);
                }
                else
                {
                    return Result.Fail($"Cannot set generic field of type '{ty}'");
                }
            }

            return Result.Fail($"Cannot set field of type '{ty}'");
        }

        private void PopulateObjectField(XmlNode node, object target, FieldInfo field, Type containingModType)
        {
            var tyResult = GetConcreteTypeFromNode(node, field.FieldType);

            if (tyResult.IsFailed)
            {
                this.Errors.Add(tyResult.ToString());
                return;
            }

            var ty = tyResult.Value;

            var value = node.Value!;

            if (ty.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefFieldCrossRef(ty, value, target, field);
                this.CrossRefs.Add(crossRef);
            }
            else
            {
                var result = GetObject(node, ty, containingModType);
                if (result.IsSuccess)
                {
                    field.SetValue(target, result.Value);
                }
                else
                {
                    this.Errors.Add($"Error setting field {field.Name} ({ty}): {result}");
                }
            }
        }

        public void AddCrossRef<TRecv, T>(TRecv receiver, string defId, Action<TRecv, T> onResolveCrossRef) where T : Def
        {
            this.CrossRefs.Add(new DefCustomCrossRef<TRecv, T>(typeof(T), defId, receiver, onResolveCrossRef));
        }
    }
}