﻿using OpenNefia.Game;
using OpenNefia.Game.Serial;

namespace OpenNefia.Core
{
    public abstract class MapObject : IDataExposable, IDataReferenceable
    {
        internal int _X;
        internal int _Y;
        internal ulong _Uid;
        public int X { get => _X; }
        public int Y { get => _Y; }
        public ulong Uid { get => _Uid; }

        public MapObject(int x, int y)
        {
            this._Uid = GameWrapper.Instance.State.UidTracker.GetNextAndIncrement();
            this.SetPosition(x, y);
        }

        private bool _Disposed = false;
        public bool Disposed { get => _Disposed; }

        internal ILocation? _CurrentLocation;

        public void SetPosition(int x, int y)
        {
            this._X = x;
            this._Y = y;
            this._CurrentLocation?.SetPosition(this, x, y);
        }

        public void Dispose()
        {
            this._CurrentLocation?.ReleaseObject(this);
            this._Disposed = true;
        }

        public bool IsOwned() => _CurrentLocation != null;

        public virtual void Expose(DataExposer data)
        {
            data.ExposeValue(ref _Uid, nameof(Uid));
            data.ExposeValue(ref _X!, nameof(X));
            data.ExposeValue(ref _Y!, nameof(Y));
            data.ExposeValue(ref _Disposed!, nameof(Disposed));
            data.ExposeWeak(ref _CurrentLocation, nameof(_CurrentLocation));
        }

        public string GetUniqueIndex() => $"MapObject_{Uid}";
    }
}
