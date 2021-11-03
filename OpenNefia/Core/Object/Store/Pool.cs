using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public class Pool<T> : Pool, IEnumerable<T> where T : MapObject
    {
        internal List<T> _DeepObjects = new List<T>();

        public override int Count { get => _DeepObjects.Count; }

        public Pool(IMapObjectHolder owner) : base(owner)
        {
        }

        public Pool() : base()
        {
        }

        public virtual void OnOwnerDestroyed()
        {
            this.Clear();
        }

        public void Clear()
        {
            for (int i = this.Count; i >= 0; i--)
            {
                var obj = this._DeepObjects[i];
                this.DoRemove(obj);
            }
        }

        public void ClearAndDestroyAll()
        {
            for (int i = this.Count; i >= 0; i--)
            {
                var obj = this._DeepObjects[i];
                this.DoRemove(obj);
                obj.Destroy();
            }
        }

        protected override MapObject? GetAt(int index) => this._DeepObjects[index];

        protected override void DoAdd(MapObject obj)
        {
            this._DeepObjects.Add((T)obj);
            obj._PoolContainingMe = this;
        }

        protected override void DoRemove(MapObject obj)
        {
            this._DeepObjects.Remove((T)obj);
            obj._PoolContainingMe = null;
        }

        public override bool CanReceiveObject(MapObject obj)
        {
            if (!(obj is T))
            {
                return false;
            }
            return base.CanReceiveObject(obj);
        }

        public override bool Contains(MapObject obj) => this._DeepObjects.Contains(obj);

        public override void Expose(DataExposer data)
        {
            data.ExposeValue(ref this.ExposeMode, nameof(ExposeMode));
            data.ExposeCollection(ref _DeepObjects, nameof(_DeepObjects), this.ExposeMode);
            if (data.Stage == SerialStage.ResolvingRefs)
            {
                this._DeepObjects.RemoveAll(x => x == null);
                foreach (var obj in this._DeepObjects)
                {
                    obj._PoolContainingMe = this;
                }
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this._DeepObjects[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.Count; i++)
            {
                yield return this.GetAt(i);
            }
        }
    }

    public abstract class Pool : IEnumerable<MapObject>, IDataExposable
    {
        protected IMapObjectHolder? _Owner;
        protected ExposeMode ExposeMode = ExposeMode.Deep;

        public IMapObjectHolder? Owner { get => _Owner; }

        internal Pool(IMapObjectHolder owner)
        {
            _Owner = owner;
        }

        internal Pool()
        {
            _Owner = null;
        }
        
        public bool TakeOrTransferObject(MapObject obj)
        {
            if (obj._PoolContainingMe != null)
            {
                obj._PoolContainingMe.ReleaseObject(obj);
            }
            return this.TakeObject(obj);
        }

        public abstract int Count { get; }
        protected abstract void DoAdd(MapObject obj);
        protected abstract void DoRemove(MapObject obj);
        protected abstract MapObject? GetAt(int index);
        public abstract bool Contains(MapObject obj);

        public bool TakeObject(MapObject obj)
        {
            if (obj == null)
            {
                Logger.Error($"Object {obj} is null.");
                return false;
            }

            if (this.Contains(obj))
            {
                Logger.Error($"Object {obj} is already in this pool.");
                return false;
            }

            if (obj._PoolContainingMe != null)
            {
                Logger.Error($"Object {obj} is already owned.");
                return false;
            }

            if (!this.CanReceiveObject(obj))
            {
                return false;
            }

            obj._PoolContainingMe = this;
            this.DoAdd(obj);

            return true;
        }

        public void ReleaseObject(MapObject obj)
        {
            if (!this.Contains(obj))
            {
                throw new Exception($"Object {obj} not managed by pool {this}");
            }

            this.DoRemove(obj);
            obj._PoolContainingMe = null;
        }

        public bool DropObject(MapObject obj, InstancedMap map, int x, int y)
        {
            if (!this.Contains(obj))
            {
                return false;
            }

            return true;
        }

        public virtual bool CanReceiveObject(MapObject obj)
        {
            return obj != null;
        }

        public abstract void Expose(DataExposer data);

        public IEnumerator<MapObject> GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this.GetAt(i)!;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (var i = 0; i < this.Count; i++)
            {
                yield return this.GetAt(i);
            }
        }
    }
}
