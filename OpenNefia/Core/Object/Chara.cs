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

        private ItemInventory _Inventory;
        public ItemInventory Inventory { get => _Inventory; }

        public Chara(ChipDef chip) : base()
        {
            Chip = new DefStat<ChipDef>(chip);
            _Inventory = new ItemInventory(this);

            Inventory.TakeObject(new Item(ChipDefOf.ItemPutitoro));
            Inventory.TakeObject(new Item(ChipDefOf.ItemPutitoro));
            Inventory.TakeObject(new Item(ChipDefOf.ItemPutitoro));
        }

#pragma warning disable CS8618
        private Chara() : base() { }
#pragma warning restore CS8618

        public static Chara? Player 
        { 
            get => GameWrapper.Instance.State.Player; 
            internal set => GameWrapper.Instance.State.Player = value; 
        }

        public override string TypeKey => "Chara";

        public static Result<Chara> Create(ILocation location, int x, int y)
        {
            var chara = new Chara(ChipDefOf.CharaRaceSlime);

            if (!location.CanReceiveObject(chara, x, y))
            {
                return Result.Fail("Location could not receive object.");
            }
            if (!location.TakeObject(chara, x, y))
            {
                return Result.Fail("Location failed to receive object.");
            }

            return Result.Ok(chara);
        }

        public override void Refresh()
        {
            Chip.Refresh();
        }

        public bool TakeItem(Item item)
        {
            if (!this.Inventory.CanReceiveObject(item))
                return false;

            return this.Inventory.TakeObject(item);
        }

        public bool DropItem(Item item)
        {
            var map = this.GetCurrentMap();
            if (map == null)
                return false;

            if (!this.Inventory.HasObject(item))
                return false;

            return map.TakeObject(item, this.X, this.Y);
        }

        public override void Expose(DataExposer data)
        {
            base.Expose(data);

            data.ExposeDeep(ref Chip, nameof(Chip), ChipDefOf.CharaBlank);
            data.ExposeDeep(ref _Inventory, nameof(_Inventory), this);
        }

        public override void ProduceMemory(MapObjectMemory memory)
        {
            memory.ChipIndex = Chip.FinalValue.Image.TileIndex;
            memory.Color = this.Color;
            memory.IsVisible = true;
            memory.ScreenXOffset = 0;
            memory.ScreenYOffset = 0;
        }
    }
}
