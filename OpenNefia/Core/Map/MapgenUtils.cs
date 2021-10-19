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
                var x = Random.Rnd(map.Width);
                var y = Random.Rnd(map.Height);
                map.SetTile(x, y, tile);
            }
        }
    }
}
