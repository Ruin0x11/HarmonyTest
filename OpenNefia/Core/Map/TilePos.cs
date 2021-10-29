using System;
using System.Collections.Generic;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Util;

namespace OpenNefia.Core.Map
{
    public struct TilePos
    {
        public int X;
        public int Y;
        public InstancedMap Map;

        public TilePos(int x, int y, InstancedMap map)
        {
            X = x;
            Y = y;
            Map = map;
        }

        public void GetScreenPos(out int sx, out int sy)
        {
            GraphicsEx.Coords.TileToScreen(X, Y, out sx, out sy);
        }

        public bool IsInWindowFov()
        {
            if (Map != Current.Map)
                return false;

            return Map.IsInWindowFov(X, Y);
        }

        public bool HasLos(TilePos pos)
        {
            if (Map != Current.Map || Map != pos.Map)
                return false;

            return Map.HasLos(X, Y, pos.X, pos.Y);
        }

        public int DistanceTo(TilePos other)
        {
            return (int)PosUtils.Dist(this.X, this.Y, other.X, other.Y);
        }
    }
}
