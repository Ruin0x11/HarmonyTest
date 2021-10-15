using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Data.Types.DefOf;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Stat;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public sealed class Item : MapObject
    {
        public DefStat<ChipDef> Chip;

        public Item(int x, int y, ChipDef chip) : base(x, y)
        {
            Chip = new DefStat<ChipDef>(chip);
        }

#pragma warning disable CS8618
        private Item() : base(0, 0) { }
#pragma warning restore CS8618

        public override string TypeKey => "Item";

        public override void Refresh()
        {
        }

        public override void Expose(DataExposer data)
        {
            base.Expose(data);

            data.ExposeDeep(ref Chip, nameof(Chip), ChipDefOf.CharaBlank);
        }

        public override void ProduceMemory(MapObjectMemory memory)
        {
            memory.ChipIndex = Chip.FinalValue.Tile.TileIndex;
            memory.Color = this.Color;
            memory.IsVisible = true;
            memory.ScreenXOffset = 0;
            memory.ScreenYOffset = 0;
        }
    }
}
