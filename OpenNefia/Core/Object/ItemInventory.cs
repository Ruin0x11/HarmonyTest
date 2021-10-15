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

        private MapObject ParentObject;
        private Pool _Pool;

        public ILocation? ParentLocation { get => ParentObject.CurrentLocation; }

        public ItemInventory(MapObject parent)
        {
            ParentObject = parent;
            MaxWeight = new ValueStat<int>();
            _Pool = new Pool(GameWrapper.Instance.State.UidTracker.GetNextAndIncrement(), this);
        }

        public void Refresh()
        {
            MaxWeight.Refresh();
        }

        public void Expose(DataExposer data)
        {
            data.ExposeWeak(ref ParentObject!, nameof(ParentObject));
            data.ExposeDeep(ref MaxWeight, nameof(MaxWeight));
            data.ExposeDeep(ref _Pool!, nameof(_Pool));
        }

        public string GetUniqueIndex() => $"{nameof(ItemInventory)}_{_Pool.Uid}";

        public bool TakeObject(MapObject obj)
        {
            if (!(obj is Item))
            {
                return false;
            }

            return _Pool.TakeObject(obj);
        }
        public bool HasObject(MapObject obj) => _Pool.HasObject(obj);
        public void ReleaseObject(MapObject obj) =>  _Pool.ReleaseObject(obj);
        public void SetPosition(MapObject mapObject, int x, int y) =>_Pool.SetPosition(mapObject, x, y);
        public IEnumerable<MapObject> At(int x, int y) => _Pool.At(x, y);
        public IEnumerator<MapObject> GetEnumerator() => _Pool.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _Pool.GetEnumerator();
    }
}