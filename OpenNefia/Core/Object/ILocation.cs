using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    public interface ILocation : IDataExposable, IDataReferenceable, IEnumerable<MapObject>
    {
        ILocation? ParentLocation { get; }
        
        /// <summary>
        /// This is used for comparison, to get the most general <see cref="ILocation"/>
        /// to run overridden ILocations operations with.
        /// 
        /// For example, consider the following hierarchy of location parents:
        /// 
        /// Map -> Chara -> Item Inventory -> Pool
        /// 
        /// In this case, the <see cref="ItemInventory"/> uses a <see cref="Pool"/> internally.
        /// The ItemInventory class might also have some checks to see if an item is too heavy to
        /// be stored in the container, and would like to prevent items from being moved into its 
        /// Pool if so.
        /// 
        /// However, the Pool does not have those item weight checks, so trying to run methods like 
        /// <see cref="ILocation.TakeObject(MapObject, int, int)"/> in a naive way will bypass
        /// these checks by calling them on the Pool inside the ItemInventory, not the ItemInventory itself.
        /// 
        /// To solve this, it is known that the ItemInventory uses the same storage as the Pool, and the Pool
        /// is tracked by a UID. Therefore it suffices to recurse up the chain of <see cref="ParentLocation"/>
        /// properties until the parent is null or its internal storage UID differs from the root.
        /// </summary>
        public ulong Uid { get; }

        public bool TakeObject(MapObject obj, int x = 0, int y = 0);
        public bool HasObject(MapObject obj);
        public bool CanReceiveObject(MapObject obj, int x = 0, int y = 0);
        public void ReleaseObject(MapObject obj);
        void SetPosition(MapObject mapObject, int x, int y);

        IEnumerable<MapObject> At(int x, int y);
        IEnumerable<T> At<T>(int x, int y) where T: MapObject;
        IEnumerable<T> EnumerateType<T>() where T: MapObject;
    }
}