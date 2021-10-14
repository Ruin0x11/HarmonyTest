using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Rendering
{
    public enum ShadowType
    {
        None,
        Normal,
        DropShadow
    }

    internal enum MemoryState
    {
        Added,
        InUse,
        Removed
    }

    public class MapObjectMemory
    {
        public ulong ObjectUid;
        public bool IsVisible;
        public string ChipIndex = string.Empty;
        public int ScreenXOffset;
        public int ScreenYOffset;
        public float Rotation;
        public Love.Color Color;
        public ShadowType ShadowType;

        internal int Index;
        internal int TileX;
        internal int TileY;
        internal int ZOrder;
        internal MemoryState State;
        internal string TypeKey = string.Empty;
    }
}
