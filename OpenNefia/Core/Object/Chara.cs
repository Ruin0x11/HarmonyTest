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

        public Chara(ChipDef chip) : this()
        {
            Chip = new DefStat<ChipDef>(chip);
            _Inventory = new ItemInventory(this);

            Inventory.TakeObject(new Item(ChipDefOf.ItemPutitoro));
            Inventory.TakeObject(new Item(ChipDefOf.ItemPutitoro));
            Inventory.TakeObject(new Item(ChipDefOf.ItemPutitoro));
        }

#pragma warning disable CS8618
        private Chara() : base() 
        {
            this.IsSolid = true;
        }
#pragma warning restore CS8618

        public static Chara? Player 
        { 
            get => Current.Game.Player;
            internal set => Current.Game.Player = value;
        }

        public static Result<Chara> Create()
        {
            var chara = new Chara(ChipDefOf.CharaRaceSlime);

            return Result.Ok(chara);
        }

        public static Result<Chara> Create(ILocation location, int x, int y)
        {
            var charaResult = Chara.Create();
            if (charaResult.IsFailed)
            {
                return charaResult;
            }

            var chara = charaResult.Value;

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
