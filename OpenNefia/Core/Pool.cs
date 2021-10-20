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
    public sealed class Pool : ILocation
    {
        private ulong _Uid;
        public ulong Uid { get => _Uid; }
        internal int Width;
        internal int Height;

        private ILocation _ParentLocation;
        public ILocation? ParentLocation { get => _ParentLocation; }

        internal List<MapObject> DeepObjects;
        internal HashSet<ulong> ContainedObjectUids;

        public Pool(ILocation parent, int width, int height, ulong uid)
        {
            _Uid = uid;
            _ParentLocation = parent;
            Width = width;
            Height = height;
            DeepObjects = new List<MapObject>();
            ContainedObjectUids = new HashSet<ulong>();
        }

        public Pool(ILocation parent, int width, int height) : this(parent, width, height, Current.Game.Uids.GetNextAndIncrement())
        {
        }

        public Pool(ILocation parent) : this(parent, 1, 1)
        {
        }

        public bool TakeObject(MapObject obj, int x, int y)
        {
            if (obj.Disposed)
            {
                throw new Exception($"Object {obj} is already disposed.");
            }

            if (ContainedObjectUids.Contains(obj.Uid))
            {
                return false;
            }

            if (obj._InternalLocation != null)
            {
                obj._InternalLocation.ReleaseObject(obj);
            }

            obj._X = x;
            obj._Y = y;
            obj._InternalLocation = this;
            DeepObjects.Add(obj);
            ContainedObjectUids.Add(obj.Uid);

            return true;
        }

        public void ReleaseObject(MapObject obj)
        {
            if (!ContainedObjectUids.Contains(obj.Uid))
                throw new Exception($"Object {obj} not managed by pool {this}");

            obj.RefreshTileOnMap();
            DeepObjects.Remove(obj);
            ContainedObjectUids.Remove(obj.Uid);
            obj._InternalLocation = null;
        }

        public bool HasObject(MapObject obj)
        {
            return ContainedObjectUids.Contains(obj.Uid);
        }

        public bool CanReceiveObject(MapObject obj, int x, int y)
        {
            return !HasObject(obj) && x >= 0 && y >= 0 && x < Width && y < Height;
        }

        public void Expose(DataExposer data)
        {
            data.ExposeWeak(ref _ParentLocation!, nameof(_ParentLocation));
            data.ExposeValue(ref Width, nameof(Width));
            data.ExposeValue(ref Height, nameof(Height));
            data.ExposeValue(ref _Uid, nameof(Uid));

            data.ExposeCollection(ref DeepObjects, nameof(DeepObjects), ExposeMode.Deep);
            if (data.Stage == SerialStage.ResolvingRefs)
            {
                foreach (var obj in this.DeepObjects)
                {
                    this.ContainedObjectUids.Add(obj.Uid);
                }
            }
        }

        public string GetUniqueIndex() => $"Pool_{Uid}";

        public void SetPosition(MapObject mapObject, int x, int y)
        {
        }

        public IEnumerable<MapObject> At(int x, int y)
        {
            foreach (var obj in this.DeepObjects)
            {
                if (obj.X == x && obj.Y == y)
                {
                    yield return obj;
                }
            }
        }

        public IEnumerable<T> At<T>(int x, int y) where T : MapObject => At(x, y).Where(x => x is T).Select(x => (x as T)!);

        public IEnumerable<T> EnumerateType<T>() where T : MapObject => this.Where(x => x is T).Select(x => (x as T)!);

        public IEnumerator<MapObject> GetEnumerator() => DeepObjects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => DeepObjects.GetEnumerator();
    }
}
