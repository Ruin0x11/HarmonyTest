using OpenNefia.Core.Extensions;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Map
{
    public static class TilePos_ObjectQueries
    {
        public static IEnumerable<MapObject> GetMapObjects(this TilePos pos)
        {
            return pos.Map._Pool.Where(obj => obj.X == pos.X && obj.Y == pos.Y);
        }

        public static IEnumerable<T> GetMapObjects<T>(this TilePos pos) where T : MapObject
        {
            return pos.Map._Pool
                .Select(obj => obj as T)
                .WhereNotNull()
                .Where(obj => obj.X == pos.X && obj.Y == pos.Y);
        }
    }
}
