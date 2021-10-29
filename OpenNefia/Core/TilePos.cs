using System;
using System.Collections.Generic;

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
    }
}