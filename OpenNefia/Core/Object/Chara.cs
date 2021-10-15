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
    public sealed class Chara : MapObject
    {
        public DefStat<ChipDef> Chip;

        private ItemInventory _Inventory;
        public ItemInventory Inventory { get => _Inventory; }

        public Chara(int x, int y, ChipDef chip) : base(x, y)
        {
            Chip = new DefStat<ChipDef>(chip);
            _Inventory = new ItemInventory(this);

            Inventory.TakeObject(new Item(0, 0, ChipDefOf.ItemPutitoro));
            Inventory.TakeObject(new Item(0, 0, ChipDefOf.ItemPutitoro));
            Inventory.TakeObject(new Item(0, 0, ChipDefOf.ItemPutitoro));
        }

#pragma warning disable CS8618
        private Chara() : base(0, 0) { }
#pragma warning restore CS8618

        public override string TypeKey => "Chara";

        public override void Refresh()
        {
            Chip.Refresh();
        }

        public override void Expose(DataExposer data)
        {
            base.Expose(data);

            data.ExposeDeep(ref Chip, nameof(Chip), ChipDefOf.CharaBlank);
            data.ExposeDeep(ref _Inventory, nameof(_Inventory), this);
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
