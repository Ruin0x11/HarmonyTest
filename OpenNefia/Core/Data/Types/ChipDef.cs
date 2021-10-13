using OpenNefia.Core.Rendering;

namespace OpenNefia.Core.Data.Types
{
    public class ChipDef : Def
    {
        public TileSpec Tile { get; set; } = null!;

        public ChipDef(string id) : base(id)
        {
        }
    }
}
