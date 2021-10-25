using OpenNefia.Core.Rendering;
using OpenNefia.Game;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.Object
{
    public abstract class MapObject : IDataExposable, IDataReferenceable, IRefreshable
    {
        internal int _X;
        internal int _Y;
        internal ulong _Uid;

        public int X { get => _X; }
        public int Y { get => _Y; }
        public ulong Uid { get => _Uid; }
        public bool IsSolid = false;
        public bool IsOpaque = false;
        public Love.Color Color = Love.Color.White;
        public int Amount { get; set; } = 1;

        public MapObject()
        {
            this._Uid = Current.Game.Uids.GetNextAndIncrement();
        }

        private bool _Destroyed = false;
        public bool Destroyed { get => _Destroyed; }

        public abstract bool IsInLiveState { get; }
        public bool IsAlive { get => !_Destroyed && IsInLiveState; }

        /// <summary>
        /// Internal root location of this object, typically a <see cref="Pool"/>
        /// that's encapsulated by a different class that implements <see cref="ILocation"/>.
        /// 
        /// Do *NOT* run ILocation methods on this object unless you know what you're doing!
        /// </summary>
        internal Pool? _PoolContainingMe;

        public Pool? InnerPool { get => _PoolContainingMe; }

        public IPoolOwner? ParentPoolOwner { get => _PoolContainingMe?.Owner; }

        public void SetPosition(int x, int y)
        {
            var map = this.GetCurrentMap();

            if (map != null && !map.IsInBounds(x, y))
            {
                return;
            }

            var oldX = this._X;
            var oldY = this._Y;

            this._X = x;
            this._Y = y;

            if (map != null)
            {
                map.RefreshTile(oldX, oldY);
                map.RefreshTile(x, y);
            }
        }

        public void Destroy()
        {
            if (this._PoolContainingMe != null)
            {
                this._PoolContainingMe.ReleaseObject(this);
            }
            this._Destroyed = true;
        }

        public virtual void GetScreenPos(out int screenX, out int screenY)
        {
            Current.Game.Coords.TileToScreen(this.X, this.Y, out screenX, out screenY);
        }

        public bool IsOwned => _PoolContainingMe != null;

        public abstract void ProduceMemory(MapObjectMemory memory);

        public abstract void Refresh();

        public virtual void RefreshTileOnMap()
        {
            this.GetCurrentMap()?.RefreshTile(this.X, this.Y);
        }

        public IEnumerable<IPoolOwner> EnumerateParents()
        {
            var owner = this.ParentPoolOwner;
            while (owner != null)
            {
                yield return owner;

                owner = owner.ParentPoolOwner;
            }
        }

        public T? GetFirstParent<T>() where T: class, IPoolOwner
        {
            return EnumerateParents()
                .Where(x => x is T)
                .Select(x => x as T)
                .FirstOrDefault();
        }

        public virtual bool CanStackWith(MapObject other)
        {
            return false;
        }

        public bool StackWith(MapObject other)
        {
            if (other == null)
                return false;

            if (!this.IsAlive || !other.IsAlive)
                return false;

            if (!this.CanStackWith(other))
                return false;

            if (this == other)
                return false;


            this.Amount += other.Amount;
            other.Amount = 0;
            other.Destroy();

            return true;
        }

        public bool StackAll(bool showMessage = false)
        {
            if (this._PoolContainingMe == null)
                return false;

            var didStack = false;

            List<Item> toStack = new List<Item>();

            foreach (var obj in this._PoolContainingMe)
            {
                var item = obj as Item;
                if (item != null && item.X == this.X && item.Y == this.Y && this.CanStackWith(item))
                {
                    toStack.Add(item);
                }
            }

            foreach (var item in toStack)
            {
                this.StackWith(item);
                item.Destroy();
                didStack = true;
            }

            if (didStack && showMessage)
            {
                // TODO
                Console.WriteLine("Stacked.");
            }

            return didStack;
        }

        public MapObject? SplitOff(int amount)
        {
            if (amount <= 0)
            {
                return null;
            }

            amount = Math.Min(this.Amount, amount);

            var separated = this.Clone();

            separated.Amount = amount;
            this.Amount -= amount;

            return separated;
        }

        public InstancedMap? GetCurrentMap() => GetFirstParent<InstancedMap>();

        public virtual void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(Uid));
            data.ExposeValue(ref _X!, nameof(X));
            data.ExposeValue(ref _Y!, nameof(Y));
            data.ExposeValue(ref IsSolid, nameof(IsSolid));
            data.ExposeValue(ref IsOpaque, nameof(IsOpaque));
            data.ExposeValue(ref Color, nameof(Color));
            data.ExposeValue(ref _Destroyed, nameof(Destroyed));
            data.ExposeWeak(ref _PoolContainingMe, nameof(_PoolContainingMe));
        }

        public string GetUniqueIndex() => $"MapObject_{Uid}";

        public virtual MapObject Clone()
        {
            // TODO: This doesn't even work. It won't handle Dictionaries or other complex data structures.
            // There will have to be an ICloneable interface implemented on map objects
            // and aspects that does the deep copying manually, but it shouldn't be too hard.
            var newObject = (MapObject)this.MemberwiseClone();
            newObject._PoolContainingMe = null;
            newObject._Uid = Current.Game.Uids.GetNextAndIncrement();
            return newObject;
        }
    }
}
