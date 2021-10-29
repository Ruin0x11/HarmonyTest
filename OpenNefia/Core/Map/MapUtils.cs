using FluentResults;
using OpenNefia.Core.Map;
using OpenNefia.Core.Object;
using OpenNefia.Core.Util;
using System;

namespace OpenNefia.Core
{
    public static class MapUtils
    {
        public static bool FindPositionToSpawnObject(MapObject obj, TilePos pos, out int foundX, out int foundY)
        {
            foundX = pos.X;
            foundY = pos.Y;
            return true;
        }

        internal static bool TrySpawn(MapObject obj, TilePos pos)
        {
            if (!obj.IsAlive)
            {
                return false;
            }

            if (obj.IsOwned)
            {
                return false;
            }

            return pos.Map.TakeObject(obj, pos.X, pos.Y);
        }
    }
}