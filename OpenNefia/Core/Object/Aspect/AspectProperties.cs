using OpenNefia.Core.Data.Serial;
using System;
using System.Xml.Linq;

namespace OpenNefia.Core.Object.Aspect
{
    public abstract class AspectProperties : IDefDeserializable
    {
        public virtual void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }
}