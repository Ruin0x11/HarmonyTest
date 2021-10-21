using FluentResults;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Serial.Attributes;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Map.Generator.Area
{
    internal class PerFloorAreaFloorGenerator : BaseAreaFloorGenerator
    {
        [DefRequired]
        [DefDictionaryFieldNames(Entry: "Floor", Key: "Num", Value: "MapDef", UseAttributes: true)]
        public Dictionary<int, MapDef> Floors = new Dictionary<int, MapDef>();

        public override Result<InstancedMap> GenerateFloor(InstancedArea area, int floor)
        {
            if (Floors.TryGetValue(floor, out MapDef? mapDef))
            {
                return mapDef.GenerateMap(area, floor);
            }

            return Result.Fail($"No MapDef found for floor {mapDef}.");
        }
    }
}
