using OpenNefia.Core.Rendering;
using OpenNefia.Game;
using OpenNefia.Serial;

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

        public MapObject(int x, int y)
        {
            this._Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            this.SetPosition(x, y);
        }

        private bool _Disposed = false;
        public bool Disposed { get => _Disposed; }

        public abstract string TypeKey { get; }

        internal ILocation? _CurrentLocation;
        public ILocation? CurrentLocation { get => _CurrentLocation; }

        public void SetPosition(int x, int y)
        {
            this._X = x;
            this._Y = y;
            this._CurrentLocation?.SetPosition(this, x, y);
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
            return newObject;
        }
    }
}
