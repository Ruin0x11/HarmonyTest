using OpenNefia.Game.Serial;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    public interface ILocation : IDataExposable, IDataReferenceable, IEnumerable<MapObject>
    {
        public void TakeObject(MapObject obj);
        public bool HasObject(MapObject obj);
        public void ReleaseObject(MapObject obj);
        void SetPosition(MapObject mapObject, int x, int y);

        IEnumerable<MapObject> At(int x, int y);
    }
}