using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Map
{
    public static class MapgenUtils
    {
        public static void SprayTile(InstancedMap map, TileDef tile, int density)
        {
            var n = map.Width * map.Height * density / 100;

            for (int i = 0; i < n; i++)
            {
                var x = Rand.NextInt(map.Width);
                var y = Rand.NextInt(map.Height);
                map.SetTile(x, y, tile);
            }
        }
    }
}
