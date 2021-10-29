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

        /// <summary>
        /// Gets the primary character on this tile.
        /// 
        /// In Elona, traditionally only one character is allowed on each tile. However, extra features
        /// such as the Riding mechanic or the Tag Teams mechanic added in Elona+ allow multiple characters to
        /// occupy the same tile. This function retrieves the "primary" character used for things like
        /// damage calculation, spell effects, and so on, which should exclude the riding mount, tag team
        /// partner, etc.
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Chara? GetPrimaryChara(this TilePos pos)
        {
            return pos.GetMapObjects<Chara>().FirstOrDefault();
        }
    }
}
