using OpenNefia.Core.Data.Serial;
using System;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Types
{
    public class AspectDefinitions : IDefDeserializable
    {
        public AspectDefinitions()
        {
        }

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }
}