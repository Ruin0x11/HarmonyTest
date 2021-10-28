using FluentResults;
using OpenNefia.Core.Data.Types;
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
    public sealed class Chara : MapObject
    {
        public DefStat<ChipDef> Chip;
        public Direction Direction;
        public ItemInventory Inventory;

        public override bool IsInLiveState => true;

        public Chara(CharaDef def) : base(def) 
        {
            this.IsSolid = true;
            this.Chip = new DefStat<ChipDef>(def.Chip);
            this.Inventory = new ItemInventory(this);
        }

        public override void Refresh()
        {
            Chip.Refresh();
        }

        public override void Expose(DataExposer data)
        {
            base.Expose(data);

            data.ExposeDeep(ref Chip, nameof(Chip), ChipDefOf.CharaBlank);
            data.ExposeValue(ref Direction, nameof(Direction), Direction.Center);
        }

        public override void ProduceMemory(MapObjectMemory memory)
        {
            memory.ChipIndex = Chip.FinalValue.Image.TileIndex;
            memory.Color = this.Color;
            memory.IsVisible = true;
            memory.ScreenXOffset = 0;
            memory.ScreenYOffset = 0;
        }

        public override void GetChildPoolOwners(List<IMapObjectHolder> outOwners)
        {
            outOwners.Add(this.Inventory);
            base.GetChildPoolOwners(outOwners);
        }
    }
}
