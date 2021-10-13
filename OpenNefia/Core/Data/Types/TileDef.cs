using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public enum TileKind
    {
        None = 0,
        Dryground = 1,
        Crop = 2,
        Water = 3,
        Snow = 4,
        MountainWater = 5,
        HardWall = 6,
        Sand = 7,
        SandHard = 8,
        Coast = 9,
        SandWater = 10
    }

    public enum TileWallKind
    {
        None = 0,
        WallBottom = 1,
        WallTop = 2
    }

    public class TileDef : Def
    {
        [DefRequired]
        public TileSpec Tile = null!;

        public bool IsSolid = false;

        public bool IsOpaque = false;

        public TileDef? Wall = null;

        [DefIgnored]
        public TileWallKind WallKind = TileWallKind.None;

        public TileKind Kind = TileKind.None;
        public TileKind Kind2 = TileKind.None;

        public TileDef(string id) : base(id)
        {
        }

        public override void OnResolveReferences()
        {
            if (this.Wall != null && this.Wall != this)
            {
                this.WallKind = TileWallKind.WallTop;
                this.Wall.WallKind = TileWallKind.WallBottom;
            }
        }

        public override void OnValidate(List<string> errors)
        {
            if (this.Wall != null)
            {
                if (this.Wall.WallKind != TileWallKind.WallBottom)
                {
                    errors.Add($"Declared as wall top, but invalid bottom wall");
                }
                if (this.WallKind != TileWallKind.WallTop)
                {
                    errors.Add($"Declared as wall top, but kind was not top");
                }
                if (this.Wall == this)
                {
                    errors.Add($"Cannot specify wall recursively");
                }
            }
            else
            {
                if (this.WallKind == TileWallKind.WallTop)
                {
                    errors.Add($"Tile is not wall top, but was declared as one");
                }
            }
        }
    }
}
