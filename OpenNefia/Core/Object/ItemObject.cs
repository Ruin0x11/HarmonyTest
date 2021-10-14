using OpenNefia.Core.Data.Types.DefOf;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public sealed class ItemObject : MapObject
    {
        public ItemObject(int x, int y) : base(x, y)
        {

        }

        public override string TypeKey => "Item";

        public override void ProduceMemory(ref MapObjectMemory memory)
        {
            memory.ChipIndex = ChipDefOf.ItemComputer.Tile.TileIndex;
            memory.Color = Love.Color.White;
            memory.IsVisible = true;
            memory.XOffset = 0;
            memory.YOffset = 0;
        }
    }
}
