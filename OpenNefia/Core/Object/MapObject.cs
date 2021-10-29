using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.Object.Aspect;
using OpenNefia.Core.Rendering;
using OpenNefia.Game;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.Object
{
    public abstract class MapObject : IDataExposable, IDataReferenceable, IRefreshable, IMapObjectHolder
    {
        internal MapObjectDef _Def;
        internal int _X;
        internal int _Y;
        internal ulong _Uid;

        public MapObjectDef BaseDef { get => _Def; }
        public int X { get => _X; internal set => _X = value; }
        public int Y { get => _Y; internal set => _Y = value; }
        public ulong Uid { get => _Uid; }
        public bool IsSolid = false;
        public bool IsOpaque = false;
        public Love.Color Color = Love.Color.White;
        public int Amount { get; set; } = 1;

        internal List<MapObjectAspect> _Aspects = new List<MapObjectAspect>();

        public MapObject(MapObjectDef def)
        {
            this._Def = def;
            this._Uid = Current.Game.Uids.GetNextAndIncrement();
        }

        private bool _Destroyed = false;
        public bool Destroyed { get => _Destroyed; }

        public abstract bool IsInLiveState { get; }
        public bool IsAlive { get => !_Destroyed && IsInLiveState; }

        internal Pool? _PoolContainingMe;

        public Pool? InnerPool { get => null; }

        public IMapObjectHolder? ParentHolder { get => _PoolContainingMe?.Owner; }

        public void SetPosition(int x, int y)
        {
            var map = this.GetContainingMap();

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

        public TilePos? GetTilePos()
        {
            var map = this.GetContainingMap();
            if (map == null)
                return null;

            return new TilePos(this.X, this.Y, map);
        }

        public virtual void AfterCreate()
        {

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
            this.GetContainingMap()?.RefreshTile(this.X, this.Y);
        }

        public IEnumerable<IMapObjectHolder> EnumerateParents()
        {
            var owner = this.ParentHolder;
            while (owner != null)
            {
                yield return owner;

                owner = owner.ParentHolder;
            }
        }

        public T? GetFirstParent<T>() where T: class, IMapObjectHolder
        {
            return EnumerateParents()
                .Where(x => x is T)
                .Select(x => x as T)
                .FirstOrDefault();
        }

        public IEnumerable<T> GetAspects<T>() where T: class
        {
            return this._Aspects.Select(x => x as T).WhereNotNull();
        }

        public IEnumerable<MapObjectAspect> GetAspects() => this._Aspects;

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

            var map = this.GetCurrentMap();

            foreach (var obj in this._PoolContainingMe)
            {
                var item = obj as Item;
                if (item != null && this.CanStackWith(item) && (map == null || (this.X == item.X && this.Y == item.Y)))
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

        public InstancedMap? GetCurrentMap() => this.ParentHolder as InstancedMap;
        public InstancedMap? GetContainingMap() => GetFirstParent<InstancedMap>();

        public virtual void Expose(DataExposer data)
        {
            data.ExposeDef(ref _Def, nameof(BaseDef));
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

        public virtual void GetChildPoolOwners(List<IMapObjectHolder> outOwners)
        {
            foreach (var aspect in this.GetAspects<IMapObjectHolder>())
            {
                aspect.GetChildPoolOwners(outOwners);
            }
        }

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
