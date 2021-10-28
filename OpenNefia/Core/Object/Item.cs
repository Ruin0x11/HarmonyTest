using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Stat;
using OpenNefia.Game;
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
        public ItemDef Def => (ItemDef)BaseDef;

        public DefStat<ChipDef> Chip;

        public Item(ItemDef def) : base(def)
        {
            Chip = new DefStat<ChipDef>(def.Chip);
        }

        public override bool IsInLiveState => Amount > 0;

        public override void Refresh()
        {
            this.Chip.Refresh();
        }

        public override bool CanStackWith(MapObject other)
        {
            if (!this.IsAlive || !other.IsAlive || this == other)
                return false;

            var otherItem = other as Item;
            if (otherItem == null)
            {
                return false;
            }

            return this.BaseDef == otherItem.BaseDef
                && this.Chip == otherItem.Chip;
        }

        public new Item? SplitOff(int amount) => (Item?)base.SplitOff(amount);

        public override void Expose(DataExposer data)
        {
            base.Expose(data);

            data.ExposeDeep(ref Chip, nameof(Chip), ChipDefOf.CharaBlank);
        }

        public override void ProduceMemory(MapObjectMemory memory)
        {
            memory.ChipIndex = Chip.FinalValue.Image.TileIndex;
            memory.Color = this.Color;
            memory.IsVisible = true;
            memory.ScreenXOffset = 0;
            memory.ScreenYOffset = 0;
        }

        public Chara? GetOwningChara()
        {
            return EnumerateParents()
                .Select(x => (x as ItemInventory)?.ParentHolder as Chara)
                .WhereNotNull()
                .FirstOrDefault();
        }
    }
}
