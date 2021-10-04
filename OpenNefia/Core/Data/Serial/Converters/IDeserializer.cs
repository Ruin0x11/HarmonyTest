using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data.Serial.Converters
{
    public interface IDeserializer
    {
        object? ReadFromXml(XmlNode node, Type objectType, object? existingValue);
    }
}
