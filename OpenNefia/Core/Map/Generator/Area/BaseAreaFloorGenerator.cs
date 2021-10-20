using FluentResults;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Map.Generator.Area
{
    public abstract class BaseAreaFloorGenerator : IAreaFloorGenerator
    {
        public abstract Result<InstancedMap> GenerateFloor(InstancedArea area, int floor);

        public virtual void DeserializeDefField(IDefDeserializer deserializer, XElement element, Type containingModType)
        {
            deserializer.PopulateAllFields(element, this, containingModType);
        }
    }
}
