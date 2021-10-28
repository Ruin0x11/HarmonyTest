using OpenNefia.Core.Stat;
using OpenNefia.Game;
using OpenNefia.Serial;
using System.Collections;
using System.Collections.Generic;

namespace OpenNefia.Core.Object
{
    public class ItemInventory : IRefreshable, IMapObjectHolder, IEnumerable<Item>
    {
        public ValueStat<int> MaxWeight;
        
        private MapObject _Parent;
        private Pool<Item> _Pool;
        public IMapObjectHolder? ParentHolder => _Parent;
        public Pool? InnerPool => _Pool;

        public ItemInventory(MapObject parent)
        {
            _Parent = parent;
            MaxWeight = new ValueStat<int>(0);
            _Pool = new Pool<Item>(this);
        }

        public void Refresh()
        {
            MaxWeight.Refresh();
        }

        public void Expose(DataExposer data)
        {
            data.ExposeWeak(ref this._Parent!, nameof(_Parent));
            data.ExposeDeep(ref MaxWeight, nameof(MaxWeight));
            data.ExposeDeep(ref _Pool!, nameof(_Pool));
        }

        public void GetChildPoolOwners(List<IMapObjectHolder> outOwners)
        {
            foreach (Item obj in this._Pool)
            {
                obj.GetChildPoolOwners(outOwners);
            }
        }

        public bool TakeItem(Item item) => _Pool.TakeObject(item);
        public bool TakeOrTransferItem(Item item) => _Pool.TakeOrTransferObject(item);

        public IEnumerator<Item> GetEnumerator() => ((IEnumerable<Item>)_Pool).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _Pool.GetEnumerator();
    }
}
