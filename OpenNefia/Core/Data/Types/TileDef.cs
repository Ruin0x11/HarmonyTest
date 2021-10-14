using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Util;
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

    public class TileDef : Def
    {
        [DefRequired]
        public TileSpec Tile = null!;

        public bool IsSolid = false;

        public bool IsOpaque = false;

        public TileSpec? Wall = null;

        public TileKind Kind = TileKind.None;
        public TileKind Kind2 = TileKind.None;

        public TileDef(string id) : base(id)
        {
        }

        public override void OnResolveReferences()
        {
            Tile.TileIndex = $"{this.Id}:Tile";
            if (this.Wall != null)
            {
                Wall.TileIndex = $"{this.Id}:Tile_Wall";
            }
        }
    }
}
