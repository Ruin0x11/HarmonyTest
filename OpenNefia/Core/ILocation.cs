using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    public interface ILocation : IDataExposable, IDataReferenceable, IEnumerable<MapObject>
    {
        ILocation? ParentLocation { get; }

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