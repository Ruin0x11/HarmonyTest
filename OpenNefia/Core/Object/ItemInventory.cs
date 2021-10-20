using OpenNefia.Core.Stat;
using OpenNefia.Game;
using OpenNefia.Serial;
using System.Collections;
using System.Collections.Generic;

namespace OpenNefia.Core.Object
{
    public class ItemInventory : IRefreshable, ILocation
    {
        public ValueStat<int> MaxWeight;

        private MapObject _ParentObject;
        public MapObject ParentObject { get => _ParentObject; }
        private Pool _Pool;

        public ILocation? ParentLocation { get => ParentObject.CurrentLocation; }

        public ulong Uid => this._Pool.Uid;

        public ItemInventory(MapObject parent)
        {
            _ParentObject = parent;
            MaxWeight = new ValueStat<int>(0);
            _Pool = new Pool(this);
        }

        public void Refresh()
        {
            MaxWeight.Refresh();
        }

        public void Expose(DataExposer data)
        {
            data.ExposeWeak(ref _ParentObject!, nameof(_ParentObject));
            data.ExposeDeep(ref MaxWeight, nameof(MaxWeight));
            data.ExposeDeep(ref _Pool!, nameof(_Pool));
        }

        public string GetUniqueIndex() => $"{nameof(ItemInventory)}_{_Pool.Uid}";

        public bool TakeObject(MapObject obj, int x = 0, int y = 0)
        {
            if (!CanReceiveObject(obj, 0, 0))
                return false;

            return _Pool.TakeObject(obj, 0, 0);
        }

        public bool CanReceiveObject(MapObject obj, int x = 0, int y = 0) => obj is Item;

        public bool HasObject(MapObject obj) => _Pool.HasObject(obj);
        public void ReleaseObject(MapObject obj) =>  _Pool.ReleaseObject(obj);
        public void SetPosition(MapObject mapObject, int x, int y) =>_Pool.SetPosition(mapObject, x, y);
        public IEnumerable<MapObject> At(int x, int y) => _Pool.At(x, y);
        public IEnumerable<T> At<T>(int x, int y) where T : MapObject => _Pool.At<T>(x, y);
        public IEnumerable<T> EnumerateType<T>() where T : MapObject => _Pool.EnumerateType<T>();
        public IEnumerator<MapObject> GetEnumerator() => _Pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _Pool.GetEnumerator();
    }
}
