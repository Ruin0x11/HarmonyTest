using FluentResults;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Map.Generator.Area
{
    internal class EmptyAreaFloorGenerator : BaseAreaFloorGenerator
    {
        public override Result<InstancedMap> GenerateFloor(InstancedArea area, int floor)
        {
            return MapDefOf.Default.GenerateMap(area, floor); 
        }
    }
}
