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
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenNefia.Core.Data.Serial
{
    /// <summary>
    /// Mode to deserialize Def IDs with.
    /// 
    /// The reason there are two modes:
    /// 
    /// 1. We want to enforce that the ID will be automatically prefixed with the name of the mod 
    ///    that adds it. That means that the Id attribute as specified on-disk should not be 
    ///    namespaced, as in Id="Window".
    /// 2. But we also want to be able to use XPath selectors on Id attributes in a uniform way.
    ///    It would be bad if we had to call both [Id="Window"] and [Id="Core.Window"] to search
    ///    for the same Def.
    /// 
    /// <see cref="FromDisk"/> will add the mod name prefix, <see cref="AlreadyLoaded"/> will not touch it
    /// and assume it's been prefixed already.
    /// </summary>
    public enum DefDeserializeMode
    {
        // The IDs of each Def will not be namespaced with the adding mod name and a period.
        // The ID will be modified in-place to add them during the deserialize process.
        // Id="Window" => id="Core.Window"
        FromDisk,
        
        // The def was already loaded from disk, so the Id attribute will have already been modified.
        AlreadyLoaded
    }

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
        public static Result<DefIdentifier> GetDefIdAndTypeFromElement(XElement node)
        {
            var defType = DefTypes.GetDefTypeFromName(node.Name.LocalName);

            if (defType == null)
            {
                return Result.Fail($"Def type '{node.Name}' not found.");
            }

            var defId = node.Attribute("Id")?.Value;

            if (defId == null)
            {
                return Result.Fail($"Def has no ID.");
            }

            return Result.Ok(new DefIdentifier(defType, defId));
        }

        public Result<Type> GetConcreteTypeFromNode(XElement node, Type baseType)
        {
            var className = node.Attribute("Class")?.Value;

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

        public static bool IsNamespacedDefId(string defId)
        {
            return defId.Contains(".");
        }

        public Result<Def> DeserializeDef(XElement node, ModInfo containingMod, DefDeserializeMode mode)
        {
            var (defBaseType, defId) = GetDefIdAndTypeFromElement(node).Value;

            var defTypeResult = GetConcreteTypeFromNode(node, defBaseType);

            if (defTypeResult.IsFailed)
            {
                return defTypeResult.ToResult<Def>();
            }

            string namespacedDefId = string.Empty;

            switch (mode)
            {
                case DefDeserializeMode.FromDisk:
                    // This XElement is coming from disk.
                    // Id attribute should not be namespaced.
                    // Automatically prefix the ID with the mod that's adding it.
                    if (defId.Contains(".") || defId.Contains(":"))
                    {
                        throw new Exception($"Def ID cannot contain period or colon: {defId}");
                    }

                    namespacedDefId = $"{containingMod.Metadata.Name}.{defId}";
                    node.Attribute("Id")!.Value = namespacedDefId;

                    break;
                case DefDeserializeMode.AlreadyLoaded:
                    // This XElement was sourced from a document previously deserialized by
                    // a DefDeserializer in FromDisk mode.
                    // The Id should be namespaced already.
                    if (!IsNamespacedDefId(defId))
                    {
                        throw new Exception($"Expected a namespaced Def ID in Def XML, got: {defId}");
                    }

                    namespacedDefId = defId;

                    break;
            }

            var defInstance = (Def)Activator.CreateInstance(defTypeResult.Value, namespacedDefId)!;
            defInstance.Mod = containingMod;

            var elonaId = node.Attribute("ElonaId")?.Value;

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
                throw new Exception($"Error loading def {namespacedDefId} ({this.Errors.Count} errors)\n{errors}");
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

        public void PopulateAllFields(XElement node, object target, Type containingModType)
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

        public void PopulateFieldByName(string name, XElement node, object target, Type containingModType)
        {
            var field = target.GetType().GetField(name)!;
            PopulateFieldInfo(field, field.Name, node, target, containingModType);
        }

        private void PopulateFieldInfo(FieldInfo field, string nodeName, XElement node, object target, Type containingModType)
        {
            if (field.GetCustomAttribute<DefUseAttributesAttribute>() != null)
            {
                if (node.Attribute(nodeName) != null)
                {
                    PopulateField(node.Attribute(nodeName)!, target, field, containingModType);
                }
                else
                {
                    CheckRequiredValue(target, nodeName, field);
                }
            }
            else
            {
                if (node.Element(nodeName) != null)
                {
                    PopulateField(node.Element(nodeName)!, target, field, containingModType);
                }
                else
                {
                    CheckRequiredValue(target, nodeName, field);
                }
            }
        }

        public void PopulateFieldByElement(string name, XElement element, object target, Type containingModType)
        {
            var field = target.GetType().GetField(name)!;
            PopulateField(element, target, field, containingModType);
        }

        public void PopulateFieldByAttribute(string name, XAttribute attribute, object target, Type containingModType)
        {
            var field = target.GetType().GetField(name)!;
            PopulateField(attribute, target, field, containingModType);
        }

        public void PopulateField(XAttribute attribute, object target, FieldInfo field, Type containingModType)
        {
            if (field.GetCustomAttribute<DefIgnoredAttribute>() != null)
                return;

            var ty = field.FieldType;
            var value = attribute.Value;

            if (ty.IsValueType)
            {
                PopulateValueTypeField(value, target, field, containingModType);
            }
            else if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                field.SetValue(target, path);
            }
            else if (ty.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefFieldCrossRef(ty, value, target, field);
                this.CrossRefs.Add(crossRef);
            }
            else
            {
                throw new Exception($"Cannot populate object type {field.FieldType} with attributes.");
            }
        }

        public void PopulateField(XElement childElement, object target, FieldInfo field, Type containingModType)
        {
            if (field.GetCustomAttribute<DefIgnoredAttribute>() != null)
                return;

            if (field.FieldType.IsValueType)
            {
                PopulateValueTypeField(childElement.Value, target, field, containingModType);
            }
            else
            {
                PopulateObjectField(childElement, target, field, containingModType);
            }
        }

        private void PopulateValueTypeField(string value, object target, FieldInfo field, Type containingModType)
        {
            if (field.FieldType.IsEnum)
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
                var typeCode = Type.GetTypeCode(field.FieldType);

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
                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                        this.Errors.Add($"Unsupported typecode '{typeCode}' for type '{target.GetType()}', field '{field.Name}' ({field.FieldType})");
                        break;
                }
            }
        }

        private IResourcePath ParseModLocalPath(string value, Type containingModType) => new ModLocalPath(containingModType, value);

        private Love.Color ParseLoveColor(XElement childNode)
        {
            var color = new Love.Color();

            var r = childNode.Attribute("R");
            if (r != null)
                color.r = byte.Parse(r.Value!);
            var g = childNode.Attribute("G");
            if (g != null)
                color.g = byte.Parse(g.Value!);
            var b = childNode.Attribute("B");
            if (b != null)
                color.b = byte.Parse(b.Value!);
            var a = childNode.Attribute("A");
            if (a != null)
                color.a = byte.Parse(a.Value!);

            return color;
        }

        public Result<object> DeserializeObject(XElement element, Type baseType, Type containingModType)
        {
            var value = element.Value!; 
            
            var tyResult = GetConcreteTypeFromNode(element, baseType);

            if (tyResult.IsFailed)
            {
                return tyResult.ToResult<object>();
            }

            var ty = tyResult.Value;

            if (ty == typeof(string))
            {
                return Result.Ok((object)value);
            }
            else if (ty == typeof(XElement))
            {
                if (!element.HasElements)
                {
                    return Result.Fail("XML element must have a child element to act as the field value.");
                }
                else
                {
                    return Result.Ok((object)element.Elements().First());
                }
            }
            else if (ty == typeof(Love.Color))
            {
                return Result.Ok(Convert.ChangeType(ParseLoveColor(element), ty));
            }
            else if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                return Result.Ok((object)path);
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
                fieldInstance.DeserializeDefField(this, element, containingModType);
                PopulateAllFields(element, fieldInstance, containingModType);
                fieldInstance.ValidateDefField(this.Errors);
                return Result.Ok((object)fieldInstance);
            }
            else if (ty.IsAbstract)
            {
                return Result.Fail($"Cannot instantiate abstract type {ty}");
            }
            else if (ty.IsGenericType)
            {
                if (ty.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var listTy = ty.GetGenericArguments()[0];
                    var list = (IList)Activator.CreateInstance(ty)!;

                    foreach (var childElement in element.Elements())
                    {
                        if (childElement.Name != "li" || !childElement.HasElements)
                        {
                            return Result.Fail($"Generic list entries must have nodes named 'li' with one element only (type {ty}");
                        }
                        else
                        {
                            list.Add(DeserializeObject(childElement.Elements().First(), listTy, containingModType));
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

        private void PopulateObjectField(XElement node, object target, FieldInfo field, Type containingModType)
        {
            var ty = field.FieldType;

            var value = node.Value!;

            if (ty.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefFieldCrossRef(ty, value, target, field);
                this.CrossRefs.Add(crossRef);
            }
            else
            {
                var result = DeserializeObject(node, ty, containingModType);
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