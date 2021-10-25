using OpenNefia.Core.Object;
using OpenNefia.Core.Util;
using System;

namespace OpenNefia.Core
{
    public static class MapUtils
    {
        internal static Point2i? FindPositionToSpawnObject(InstancedMap instancedMap, MapObject obj, int x, int y)
        {
            return new Point2i(x, y);
        }
    }
}