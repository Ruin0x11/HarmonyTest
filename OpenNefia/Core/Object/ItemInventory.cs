using OpenNefia.Core.Stat;
using OpenNefia.Game;
using OpenNefia.Serial;
using System.Collections;
using System.Collections.Generic;

namespace OpenNefia.Core.Object
{
    public class ItemInventory : IRefreshable, IMapObjectHolder
    {
        public ValueStat<int> MaxWeight;
        
        private IMapObjectHolder _ParentObject;
        private Pool<Item> _Pool;
        public IMapObjectHolder? ParentHolder => _ParentObject;
        public Pool? InnerPool => _Pool;

        public ItemInventory(IMapObjectHolder parent)
        {
            _ParentObject = parent;
            MaxWeight = new ValueStat<int>(0);
            _Pool = new Pool<Item>(this);
        }

        public void Refresh()
        {
            MaxWeight.Refresh();
        }

        public void Expose(DataExposer data)
        {
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
    }
}
