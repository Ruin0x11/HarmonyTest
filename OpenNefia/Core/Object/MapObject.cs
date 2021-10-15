using OpenNefia.Core.Rendering;
using OpenNefia.Game;
using OpenNefia.Serial;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.Object
{
    public abstract class MapObject : IDataExposable, IDataReferenceable, IRefreshable, IOwned
    {
        internal int _X;
        internal int _Y;
        internal ulong _Uid;

        public int X { get => _X; }
        public int Y { get => _Y; }
        public ulong Uid { get => _Uid; }
        public Love.Color Color = Love.Color.White;

        public MapObject()
        {
            this._Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
        }

        private bool _Disposed = false;
        public bool Disposed { get => _Disposed; }

        public abstract string TypeKey { get; }

        internal ILocation? _CurrentLocation;
        public ILocation? CurrentLocation { get => _CurrentLocation; }

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
            
            this._CurrentLocation?.SetPosition(this, x, y);

            if (map != null)
            {
                map.RefreshTile(oldX, oldY);
                map.RefreshTile(x, y);
            }
        }

        public void ReleaseOwnership()
        {
            this.CurrentLocation?.ReleaseObject(this);
        }

        public void Dispose()
        {
            this.ReleaseOwnership();
            this._Disposed = true;
        }

        public virtual void GetScreenPos(out int screenX, out int screenY)
        {
            GameWrapper.Instance.State.Coords.TileToScreen(this.X, this.Y, out screenX, out screenY);
        }

        public bool IsOwned() => _CurrentLocation != null;

        public abstract void ProduceMemory(MapObjectMemory memory);

        public abstract void Refresh();

        public virtual void RefreshTileOnMap()
        {
            this.GetCurrentMap()?.RefreshTile(this.X, this.Y);
        }

        public IEnumerable<ILocation> EnumerateParents()
        {
            var location = this.CurrentLocation;
            while (location != null)
            {
                yield return location;

                location = location.ParentLocation;
            }
        }

        public T? GetFirstParent<T>() where T: class, ILocation
        {
            return EnumerateParents()
                .Where(x => x is T)
                .Select(x => x as T)
                .FirstOrDefault();
        }

        public InstancedMap? GetCurrentMap() => GetFirstParent<InstancedMap>();

        public virtual void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(Uid));
            data.ExposeValue(ref _X!, nameof(X));
            data.ExposeValue(ref _Y!, nameof(Y));
            data.ExposeValue(ref Color, nameof(Color));
            data.ExposeValue(ref _Disposed!, nameof(Disposed));
            data.ExposeWeak(ref _CurrentLocation, nameof(_CurrentLocation));
        }

        public string GetUniqueIndex() => $"MapObject_{Uid}";

        public MapObject Clone()
        {
            var newObject = (MapObject)this.MemberwiseClone();
            newObject._CurrentLocation = null;
            newObject._Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            return newObject;
        }
    }
}
