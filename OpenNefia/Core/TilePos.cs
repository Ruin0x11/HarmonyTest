using System;
using System.Collections.Generic;
using OpenNefia.Core.Rendering;

namespace OpenNefia.Core
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
            GraphicsEx.Coords.TileToScreen(this.X, this.Y, out sx, out sy);
        }

        public bool IsInWindowFov()
        {
            if (this.Map != Current.Map)
                return false;

            return this.Map.IsInWindowFov(this.X, this.Y);
        }

        public bool HasLos(TilePos pos)
        {
            if (this.Map != Current.Map || this.Map != pos.Map)
                return false;

            return this.Map.HasLos(this.X, this.Y, pos.X, pos.Y);
        }
    }
}
