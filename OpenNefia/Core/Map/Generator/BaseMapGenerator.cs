using FluentResults;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Map.Generator
{
    public abstract class BaseMapGenerator : IMapGenerator
    {
        public abstract Result<InstancedMap> Generate(MapDef mapDef);

        public void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }
}
