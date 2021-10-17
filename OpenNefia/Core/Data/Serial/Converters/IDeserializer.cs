using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Serial.Converters
{
    public interface IDeserializer
    {
        object? ReadFromXml(XElement node, Type objectType, object? existingValue);
    }
}
