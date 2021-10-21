using FluentResults;
using Love;
using OpenNefia.Core.Data.Serial.Attributes;
using OpenNefia.Core.Data.Serial.CrossRefs;
using OpenNefia.Core.Util;
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
        // Id="Window" => Id="Core.Window"
        FromDisk,

        // The def was already loaded from disk, so the Id attribute will have already been modified.
        AlreadyLoaded
    }

    public sealed class DefDeserializer : IDefDeserializer
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

            return DefTypes.FindDefDeserializableTypeFromName(className, baseType);
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

            var defInstance = (Def)ReflectionUtils.CreateFromPublicOrPrivateCtor(defTypeResult.Value, new object[] { namespacedDefId })!;
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
                var result = DeserializeValueType(value, field.FieldType, containingModType, field);
                if (result.IsSuccess)
                {
                    field.SetValue(target, result.Value);
                }
                else
                {
                    this.Errors.Add(String.Join("\n", result.Errors));
                }
            }
            else if (ty == typeof(IResourcePath))
            {
                IResourcePath path = ParseModLocalPath(value, containingModType);
                field.SetValue(target, path);
            }
            else if (ty.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefFieldCrossRef(new DefIdentifier(ty, value), target, field);
                this.CrossRefs.Add(crossRef);
            }
            else
            {
                this.Errors.Add($"Cannot populate object type {field.FieldType} with attributes.");
            }
        }

        public void PopulateField(XElement element, object target, FieldInfo field, Type containingModType)
        {
            if (field.GetCustomAttribute<DefIgnoredAttribute>() != null)
                return;

            if (field.FieldType.IsSubclassOf(typeof(Def)))
            {
                // Defer setting until all defs have been loaded.
                var crossRef = new DefFieldCrossRef(new DefIdentifier(field.FieldType, element.Value), target, field);
                this.CrossRefs.Add(crossRef);
                return;
            }

            var result = this.DeserializeValueOrObject(element, field.FieldType, containingModType, field);

            if (result.IsSuccess)
            {
                field.SetValue(target, result.Value);
            }
            else
            {
                this.Errors.Add(string.Join("\n", result.Errors.Select(x => x.Message)) + $" (Field: {field.Name}, type: {field.FieldType})");
            }
        }

        private Result<object> DeserializeValueOrObject(XElement elem, Type type, Type containingModType, FieldInfo? field = null) 
        {
            if (type.IsValueType)
            {
                return DeserializeValueType(elem.Value, type, containingModType, field);
            }
            else
            {
                return DeserializeObject(elem, type, containingModType, field);
            }
        }

        private static Result<object> DeserializeValueType(string value, Type type, Type containingModType, FieldInfo? field = null)
        {
            if (type.IsEnum)
            {
                if (Enum.IsDefined(type, value))
                {
                    var parsed = Enum.Parse(type, value);
                    return Result.Ok(parsed);
                }
                else
                {
                    return Result.Fail($"Enum '{type}' does not have variant '{value}')");
                }
            }
            else
            {
                var typeCode = Type.GetTypeCode(type);

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        return Result.Ok((object)bool.Parse(value));
                    case TypeCode.Char:
                        return Result.Ok((object)char.Parse(value));
                    case TypeCode.SByte:
                        return Result.Ok((object)sbyte.Parse(value));
                    case TypeCode.Byte:
                        return Result.Ok((object)byte.Parse(value));
                    case TypeCode.Int16:
                        return Result.Ok((object)short.Parse(value));
                    case TypeCode.UInt16:
                        return Result.Ok((object)ushort.Parse(value));
                    case TypeCode.Int32:
                        return Result.Ok((object)int.Parse(value));
                    case TypeCode.UInt32:
                        return Result.Ok((object)uint.Parse(value));
                    case TypeCode.Int64:
                        return Result.Ok((object)long.Parse(value));
                    case TypeCode.UInt64:
                        return Result.Ok((object)ulong.Parse(value));
                    case TypeCode.Single:
                        return Result.Ok((object)float.Parse(value));
                    case TypeCode.Double:
                        return Result.Ok((object)double.Parse(value));
                    case TypeCode.Decimal:
                        return Result.Ok((object)decimal.Parse(value));
                    case TypeCode.DateTime:
                        return Result.Ok((object)DateTime.Parse(value));
                    case TypeCode.String:
                        return Result.Ok((object)value);

                    case TypeCode.Object:
                        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            var result = DeserializeValueType(value, Nullable.GetUnderlyingType(type)!, containingModType, field);
                            if (result.IsSuccess)
                            {
                                return Result.Ok((object)Activator.CreateInstance(type, result.Value)!);
                            }
                            else
                            {
                                return result;
                            }
                        }
                        else
                        {
                            return Result.Fail($"Unsupported object type '{type}'");
                        }

                    case TypeCode.Empty:
                    case TypeCode.DBNull:
                    default:
                        return Result.Fail($"Unsupported typecode '{typeCode}' for type '{type}'");
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

        public Result<object> DeserializeObject(XElement element, Type baseType, Type containingModType, FieldInfo? member = null)
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
            else if (typeof(IDefDeserializable).IsAssignableFrom(ty))
            {
                var fieldInstance = (IDefDeserializable)ReflectionUtils.CreateFromPublicOrPrivateCtor(ty)!;
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
                return DeserializeGenericType(element, containingModType, member, ty);
            }

            return Result.Fail($"Cannot set field of type '{ty}'");
        }

        private Result<object> DeserializeGenericType(XElement element, Type containingModType, FieldInfo? member, Type ty)
        {
            var genericType = ty.GetGenericTypeDefinition();
            if (genericType == typeof(List<>) || genericType == typeof(HashSet<>))
            {
                return DeserializeList(element, containingModType, ty);
            }
            else if (genericType == typeof(Dictionary<,>))
            {
                return DeserializeDictionary(element, containingModType, member, ty);
            }
            else
            {
                return Result.Fail($"Cannot set generic field of type '{ty}'");
            }
        }

        private Result<object> DeserializeList(XElement element, Type containingModType, Type targetCollectionType)
        {
            var listTy = targetCollectionType.GetGenericArguments()[0];

            var listDefTy = listTy;
            var needsCrossRef = false;
            if (typeof(Def).IsAssignableFrom(listTy))
            {
                needsCrossRef = true;
                listTy = typeof(string);
            }

            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(listDefTy))!;

            foreach (var childElement in element.Elements())
            {
                if (childElement.Name != "li")
                {
                    return Result.Fail($"Generic list entries must have nodes named 'li' (type {targetCollectionType}");
                }
                else
                {
                    var result = DeserializeValueOrObject(childElement, listTy, containingModType);
                    if (result.IsFailed)
                    {
                        return result;
                    }
                    list.Add(result.Value);
                }
            }

            if (needsCrossRef)
            {
                var defIdentList = list.Cast<string>().Select(x => new DefIdentifier(listDefTy, x)).ToList();

                if (targetCollectionType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    var crossRefType = (typeof(DefListCrossRef<>)).MakeGenericType(listDefTy);
                    var targetList = Activator.CreateInstance(targetCollectionType)!;
                    this.CrossRefs.Add((IDefCrossRef)Activator.CreateInstance(crossRefType, targetList, defIdentList)!);
                    return Result.Ok(targetList);
                }
                else
                {
                    return Result.Fail($"Cannot deserialize generic type {targetCollectionType}");
                }
            }
            else
            {
                if (targetCollectionType.GetGenericTypeDefinition() == typeof(List<>))
                {
                    return Result.Ok((object)list);
                }
                else if (targetCollectionType.GetGenericTypeDefinition() == typeof(HashSet<>))
                {
                    var hashSet = Activator.CreateInstance(targetCollectionType)!;
                    var method = targetCollectionType.GetMethod("Add")!;
                    foreach (var elem in list)
                    {
                        method.Invoke(hashSet, new object[] { elem });
                    }
                    return Result.Ok(hashSet);
                }
                else
                {
                    return Result.Fail($"Cannot deserialize generic type {targetCollectionType}");
                }
            }
        }

        private Result<object> DeserializeDictionary(XElement element, Type containingModType, FieldInfo? member, Type ty)
        {
            var keyTy = ty.GetGenericArguments()[0];
            var valueTy = ty.GetGenericArguments()[1];

            Type keyDefType = keyTy;
            Type valueDefType = valueTy;
            var keyNeedsCrossRef = false;
            var valueNeedsCrossRef = false;
            if (typeof(Def).IsAssignableFrom(keyTy))
            {
                keyNeedsCrossRef = true;
                keyTy = typeof(string);
            }
            if (typeof(Def).IsAssignableFrom(valueTy))
            {
                valueNeedsCrossRef = true;
                valueTy = typeof(string);
            }

            var dict = Activator.CreateInstance(ty)!;
            var keyList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(keyTy))!;
            var valueList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(valueTy))!;

            var entryName = DefDictionaryFieldNamesAttribute.DEFAULT_ENTRY_NAME;
            var keyName = DefDictionaryFieldNamesAttribute.DEFAULT_KEY_NAME;
            var valueName = DefDictionaryFieldNamesAttribute.DEFAULT_VALUE_NAME;
            var useAttributes = false;

            var dictNamesAttr = member?.GetCustomAttribute<DefDictionaryFieldNamesAttribute>();

            if (dictNamesAttr != null)
            {
                entryName = dictNamesAttr.Entry;
                keyName = dictNamesAttr.Key;
                valueName = dictNamesAttr.Value;
                useAttributes = dictNamesAttr.UseAttributes;
            }

            foreach (var childElement in element.Elements())
            {
                if (childElement.Name != entryName)
                {
                    return Result.Fail($"Expected dictionary to have entries like <{entryName} ... /> (type {ty}");
                }
                else
                {
                    if (useAttributes)
                    {
                        var keyAttr = childElement.Attribute(keyName);
                        var valueAttr = childElement.Attribute(valueName);

                        if (keyAttr == null || valueAttr == null)
                        {
                            return Result.Fail($"Expected dictionary to have entries like <{entryName} {keyName}=\"...\" {valueName}=\"...\" /> (type {ty})");
                        }

                        var keyResult = DeserializeValueType(keyAttr.Value, keyTy, containingModType);
                        if (keyResult.IsFailed)
                        {
                            return keyResult;
                        }
                        keyList.Add(keyResult.Value);

                        var valueResult = DeserializeValueType(keyAttr.Value, valueTy, containingModType);
                        if (valueResult.IsFailed)
                        {
                            return valueResult;
                        }
                        valueList.Add(valueResult.Value);
                    }
                    else
                    {
                        var keyElem = childElement.Attribute(keyName);
                        var valueElem = childElement.Attribute(valueName);

                        if (keyElem == null || valueElem == null)
                        {
                            return Result.Fail($"Expected dictionary to have entries like <{entryName}><{keyName}/><{valueName}/></{entryName}> (type {ty})");
                        }

                        var keyResult = DeserializeValueOrObject(childElement, keyTy, containingModType);
                        if (keyResult.IsFailed)
                        {
                            return keyResult;
                        }
                        keyList.Add(keyResult.Value);

                        var valueResult = DeserializeValueOrObject(childElement, valueTy, containingModType);
                        if (valueResult.IsFailed)
                        {
                            return valueResult;
                        }
                        valueList.Add(valueResult.Value);
                    }
                }
            }

            if (keyNeedsCrossRef)
            {
                keyList = keyList.Cast<string>().Select(id => new DefIdentifier(keyDefType, id)).ToList();
            }
            if (valueNeedsCrossRef)
            {
                valueList = valueList.Cast<string>().Select(id => new DefIdentifier(valueDefType, id)).ToList();
            }

            if (keyNeedsCrossRef || valueNeedsCrossRef)
            {
                var crossRefType = (typeof(DefDictionaryCrossRef<,>)).MakeGenericType(keyDefType, valueDefType);
                this.CrossRefs.Add((IDefCrossRef)Activator.CreateInstance(crossRefType, dict, keyList, valueList)!);
            }
            else
            {
                var methodAdd = dict.GetType().GetMethod("Add")!;
                for (int i = 0; i < keyList.Count; i++)
                {
                    methodAdd.Invoke(dict, new object?[] { keyList[i], valueList[i] });
                }
            }

            return Result.Ok(dict);
        }

        public void AddCrossRef(IDefCrossRef crossRef)
        {
            this.CrossRefs.Add(crossRef);
        }
    }
}