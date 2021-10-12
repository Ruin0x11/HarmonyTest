using System;
using System.Xml;

namespace OpenNefia.Core.Data.Serial
{
    public interface IDefSerializable
    {
        public void DeserializeDefField(IDefDeserializer deserializer, XmlNode node, Type containingModType)
        {
            deserializer.PopulateAllFields(node, this, containingModType);
        }
    }
}