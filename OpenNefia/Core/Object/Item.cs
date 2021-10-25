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
        public DefStat<ChipDef> Chip;

        internal Item(ItemDef def) : base(def)
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
            var otherItem = other as Item;
            if (otherItem == null)
            {
                return false;
            }

            return this.Chip == otherItem.Chip;
        }

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
                .Select(x => (x as ItemInventory)?.ParentObject as Chara)
                .WhereNotNull()
                .FirstOrDefault();
        }
    }
}
