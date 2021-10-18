using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Serial
{
    public interface IDefDeserializable
    {
        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType);

        public void ValidateDefField(List<string> errors)
        {

        }
    }
}