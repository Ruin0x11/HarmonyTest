using FluentResults;
using OpenNefia.Core.Object;
using OpenNefia.Core.Util;
using System;

namespace OpenNefia.Core
{
    public static class MapUtils
    {
        public static bool FindPositionToSpawnObject(InstancedMap instancedMap, MapObject obj, int x, int y, out int foundX, out int foundY)
        {
            foundX = x;
            foundY = y;
            return true;
        }

        internal static bool TrySpawn(MapObject obj, InstancedMap map, int x, int y)
        {
            if (!obj.IsAlive)
            {
                return false;
            }

            return true;
        }
    }
}