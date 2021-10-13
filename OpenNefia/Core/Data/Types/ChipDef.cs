using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Util;

namespace OpenNefia.Core.Data.Types
{
    public class ChipDef : Def
    {
        [DefRequired]
        public TileSpec Tile = null!;

        public ChipDef(string id) : base(id)
        {
        }

        public override void OnResolveReferences()
        {
            Tile.TileIndex = new StructMultiKey<string, string>(this.Id, this.Tile.TileId);
        }
    }
}
