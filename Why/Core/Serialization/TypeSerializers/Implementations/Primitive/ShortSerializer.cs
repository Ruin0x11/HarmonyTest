﻿using System.Globalization;
using Why.Core.IoC;
using Why.Core.Serialization.Manager;
using Why.Core.Serialization.Manager.Attributes;
using Why.Core.Serialization.Manager.Result;
using Why.Core.Serialization.Markdown;
using Why.Core.Serialization.Markdown.Validation;
using Why.Core.Serialization.Markdown.Value;
using Why.Core.Serialization.TypeSerializers.Interfaces;

namespace Why.Core.Serialization.TypeSerializers.Implementations.Primitive
{
    [TypeSerializer]
    public class ShortSerializer : ITypeSerializer<short, ValueDataNode>
    {
        public ValidationNode Validate(ISerializationManager serializationManager, ValueDataNode node,
            IDependencyCollection dependencies, ISerializationContext? context = null)
        {
            return short.TryParse(node.Value, out _)
                ? new ValidatedValueNode(node)
                : new ErrorNode(node, $"Failed parsing short value: {node.Value}");
        }

        public DeserializationResult Read(ISerializationManager serializationManager, ValueDataNode node,
            IDependencyCollection dependencies, bool skipHook, ISerializationContext? context = null)
        {
            return new DeserializedValue<short>(short.Parse(node.Value, CultureInfo.InvariantCulture));
        }

        public DataNode Write(ISerializationManager serializationManager, short value, bool alwaysWrite = false,
            ISerializationContext? context = null)
        {
            return new ValueDataNode(value.ToString(CultureInfo.InvariantCulture));
        }

        public short Copy(ISerializationManager serializationManager, short source, short target, bool skipHook,
            ISerializationContext? context = null)
        {
            return source;
        }
    }
}