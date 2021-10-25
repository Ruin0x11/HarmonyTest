using System.Collections.Generic;

namespace OpenNefia.Core.Object
{
    public interface IOwned
    {
        /// <summary>
        /// Returns the most general <see cref="ILocation"/> containing this object.
        /// 
        /// Consider an <see cref="ItemInventory"/> that implements <see cref="ILocation"/>
        /// by containing a <see cref="Pool"/> and forwarding all the ILocation methods to that pool.
        /// The ItemInventory has some extra logic to prevent items from being taken if their
        /// weight exceeds a certain value. But the Pool, which actually tracks the item ownership 
        /// internally, does not have these checks. Therefore, CurrentLocation in this case must return
        /// the ItemInventory, not the Pool.
        /// 
        /// This property is meant to be retrieved by using <see cref="ILocation.ParentLocation"/> combined with
        /// checking the <see cref="ILocation.Uid"/> of each parent, until a differing or null root storage is found.
        /// </summary>
        public IPoolOwner? CurrentOwner { get; }
    }
}