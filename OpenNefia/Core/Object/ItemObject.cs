using OpenNefia.Core.Data.Types;
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
        public Stat<ChipDef> Chip;

        public ItemObject(int x, int y, ChipDef chip) : base(x, y)
        {
            Chip = new Stat<ChipDef>(chip);
        }

        public override string TypeKey => "Item";

        public override void Refresh()
        {
            Chip.Refresh();
        }

        public override void ProduceMemory(MapObjectMemory memory)
        {
            memory.ChipIndex = Chip.Value.Tile.TileIndex;
            memory.Color = Love.Color.White;
            memory.IsVisible = true;
            memory.ScreenXOffset = 0;
            memory.ScreenYOffset = 0;
        }
    }
}
