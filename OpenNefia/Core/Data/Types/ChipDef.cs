using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Util;

namespace OpenNefia.Core.Data.Types
{
    public class ChipDef : Def
    {
        [DefRequired]
        public TileSpec Image = null!;

        public ChipDef(string id) : base(id)
        {
        }

        public override void OnResolveReferences()
        {
            Image.TileIndex = $"{this.Id}:{this.Image.TileId}";
        }
    }
}
