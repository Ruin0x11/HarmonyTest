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
        internal List<T> DeepObjects = new List<T>();

        public int Count { get => DeepObjects.Count; }

        public Pool(IPoolOwner owner) : base(owner)
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
                var obj = this.DeepObjects[i];
                this.DoRemove(obj);
            }
        }

        public void ClearAndDestroyAll()
        {
            for (int i = this.Count; i >= 0; i--)
            {
                var obj = this.DeepObjects[i];
                this.DoRemove(obj);
                obj.Destroy();
            }
        }

        protected override void DoAdd(MapObject obj)
        {
            this.DeepObjects.Add((T)obj);
            obj._PoolContainingMe = this;
        }

        protected override void DoRemove(MapObject obj)
        {
            this.DeepObjects.Remove((T)obj);
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

        public override bool Contains(MapObject obj) => this.DeepObjects.Contains(obj);

        public override void Expose(DataExposer data)
        {
            data.ExposeWeak(ref _Owner!, nameof(_Owner));
            data.ExposeValue(ref this.ExposeMode, nameof(ExposeMode));
            data.ExposeCollection(ref DeepObjects, nameof(DeepObjects), this.ExposeMode);
            if (data.Stage == SerialStage.ResolvingRefs)
            {
                this.DeepObjects.RemoveAll(x => x == null);
                foreach (var obj in this.DeepObjects)
                {
                    obj._PoolContainingMe = this;
                }
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() => this.DeepObjects.GetEnumerator();
    }

    public abstract class Pool : IEnumerable, IDataExposable
    {
        protected IPoolOwner? _Owner;
        protected ExposeMode ExposeMode = ExposeMode.Deep;

        public IPoolOwner? Owner { get => _Owner; }

        internal Pool(IPoolOwner owner)
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

        protected abstract void DoAdd(MapObject obj);
        protected abstract void DoRemove(MapObject obj);
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
    }
}
