using OpenNefia.Game.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    internal class Pool : ILocation
    {
        private ulong _Uid;
        public ulong Uid { get => _Uid; }
        internal int Width;
        internal int Height;

        internal List<MapObject> DeepObjects;
        internal HashSet<ulong> ContainedObjectUids;

        public Pool(ulong uid, int width, int height)
        {
            _Uid = uid;
            Width = width;
            Height = height;
            DeepObjects = new List<MapObject>();
            ContainedObjectUids = new HashSet<ulong>();
        }

        public void TakeObject(MapObject obj)
        {
            if (obj.Disposed)
            {
                throw new Exception($"Object {obj} is already disposed.");
            }

            if (ContainedObjectUids.Contains(obj.Uid))
            {
                return;
            }

            if (obj._CurrentLocation != null)
            {
                obj._CurrentLocation.ReleaseObject(obj);
            }

            obj._CurrentLocation = this;
            DeepObjects.Add(obj);
            ContainedObjectUids.Add(obj.Uid);
        }

        public void ReleaseObject(MapObject obj)
        {
            if (!ContainedObjectUids.Contains(obj.Uid))
                throw new Exception($"Object {obj} not managed by pool {this}");

            DeepObjects.Remove(obj);
            ContainedObjectUids.Remove(obj.Uid);
            obj._CurrentLocation = null;
        }

        public bool HasObject(MapObject obj)
        {
            return ContainedObjectUids.Contains(obj.Uid);
        }

        public void Expose(DataExposer data)
        {
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

        public IEnumerator<MapObject> GetEnumerator() => DeepObjects.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => DeepObjects.GetEnumerator();
    }
}
