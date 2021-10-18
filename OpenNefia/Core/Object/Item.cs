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
    public sealed class Item : MapObject, IStackableObject<Item>
    {
        public DefStat<ChipDef> Chip;

        public Item(ChipDef chip) : base()
        {
            Chip = new DefStat<ChipDef>(chip);
        }

#pragma warning disable CS8618
        private Item() : base() { }
#pragma warning restore CS8618

        public override string TypeKey => "Item";

        public int Amount { get; set; } = 1;

        public override void Refresh()
        {
            this.Chip.Refresh();
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

        public bool CanStackWith(Item other)
        {
            if (this.Disposed || other.Disposed)
                return false;

            if (this == other)
                return false;

            return this.Chip.Equals(other.Chip);
        }

        public bool Separate(int amount, out Item? separated)
        {
            if (amount < 0 || amount > this.Amount)
            {
                throw new ArgumentOutOfRangeException(nameof(amount), amount, $"Amount must be > 0 and <= {this.Amount}");
            }

            if (amount == 0)
            {
                separated = null;
                return false;
            }

            separated = this.Clone();

            separated.Amount = amount;
            this.Amount -= amount;

            return true;
        }

        public bool StackWith(Item other)
        {
            if (!this.CanStackWith(other))
                return false;

            this.Amount += other.Amount;
            other.Amount = 0;
            other.Dispose();

            return true;
        }

        public bool StackAll(bool showMessage = false)
        {
            if (this.CurrentLocation == null)
                return false;

            var didStack = false;

            List<Item> toStack = new List<Item>();

            foreach (var obj in this.CurrentLocation!)
            {
                var item = obj as Item;
                if (item != null && item.X == this.X && item.Y == this.Y && this.CanStackWith(item))
                {
                    toStack.Add(item);
                }
            }

            foreach (var item in toStack)
            {
                item.ReleaseOwnership();
                this.StackWith(item);
                didStack = true;
            }
            
            if (didStack && showMessage)
            {
                // TODO
                Console.WriteLine("Stacked.");
            }

            return didStack;
        }

        public bool MoveSome(int amount, ILocation where, int x, int y)
        {
            if (!this.Separate(amount, out var separated))
                return false;

            if (!where.CanReceiveObject(separated!))
            {
                this.StackWith(separated!);
                return false;
            }

            if (!where.TakeObject(separated!))
            {
                return false;
            }

            return true;
        }

        public Item Clone()
        {
            // TODO: This doesn't even work. It won't handle Dictionaries or other complex data structures.
            // There will have to be an ICloneable interface implemented on map objects
            // and aspects that does the deep copying manually, but it shouldn't be too hard.
            var newObject = (Item)this.MemberwiseClone();
            newObject._CurrentLocation = null;
            newObject._Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            return newObject;
        }
    }
}
