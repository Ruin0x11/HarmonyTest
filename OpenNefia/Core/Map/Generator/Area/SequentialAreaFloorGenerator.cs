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
    internal class SequentialAreaFloorGenerator : BaseAreaFloorGenerator
    {
        [DefRequired]
        public List<IAreaFloorGenerator> InnerGenerators = new List<IAreaFloorGenerator>();

        public override Result<InstancedMap> GenerateFloor(InstancedArea area, int floor)
        {
            foreach (var generator in InnerGenerators)
            {
                var result = generator.GenerateFloor(area, floor);
                if (result.IsSuccess)
                {
                    return result;
                }
            }

            return Result.Fail("No floor generators matched.");
        }
    }
}
