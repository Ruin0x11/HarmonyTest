using NUnit.Framework;
using OpenNefia.Core;
using OpenNefia.Core.Object;
using OpenNefia.Serial;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Test.Core.Object
{
    public class MapObjectTests
    {
        private class MyCustomLocation : ILocation
        {
            Pool _Pool;

            public ILocation? _ParentLocation;
            public ILocation? ParentLocation { get => _ParentLocation; }

            public ulong Uid => _Pool.Uid;

            public MyCustomLocation(ILocation? parentLocation = null)
            {
                this._Pool = new Pool(this);
                this._ParentLocation = parentLocation;
            }

            public bool TakeObject(MapObject obj, int x = 0, int y = 0)
            {
                if (!CanReceiveObject(obj, 0, 0))
                    return false;

                return _Pool.TakeObject(obj, 0, 0);
            }

            public bool CanReceiveObject(MapObject obj, int x = 0, int y = 0) => obj is Chara;

            public bool HasObject(MapObject obj) => _Pool.HasObject(obj);
            public void ReleaseObject(MapObject obj) => _Pool.ReleaseObject(obj);
            public void SetPosition(MapObject mapObject, int x, int y) => _Pool.SetPosition(mapObject, x, y);
            public IEnumerable<MapObject> At(int x, int y) => _Pool.At(x, y);
            public IEnumerable<T> At<T>(int x, int y) where T : MapObject => _Pool.At<T>(x, y);
            public IEnumerable<T> EnumerateType<T>() where T : MapObject => _Pool.EnumerateType<T>();
            public IEnumerator<MapObject> GetEnumerator() => _Pool.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => _Pool.GetEnumerator();

            public void Expose(DataExposer data)
            {
                data.ExposeWeak(ref _ParentLocation, nameof(_ParentLocation));
                data.ExposeDeep(ref _Pool, nameof(Pool));
            }

            public string GetUniqueIndex() => $"{nameof(MyCustomLocation)}_{Uid}";
        }

        [Test]
        public void TestMapObject_CurrentLocation_None()
        {
            var chara = Chara.Create().Value;

            Assert.IsNull(chara.CurrentLocation);
            Assert.IsFalse(chara.IsOwned);
        }

        [Test]
        public void TestMapObject_CurrentLocation_Nested()
        {
            var storage = new MyCustomLocation();
            var storage2 = new MyCustomLocation(storage);
            var chara = Chara.Create(storage, 0, 0).Value;

            Assert.IsTrue(chara.IsOwned);
            Assert.AreEqual(storage, chara.CurrentLocation);
        }

        [Test]
        public void TestMapObject_CurrentLocation_TopLevel()
        {
            var storage = new MyCustomLocation();
            var storage2 = new MyCustomLocation(storage);
            var chara = Chara.Create(storage2, 0, 0).Value;

            Assert.AreEqual(storage2, chara.CurrentLocation);
        }

        [Test]
        public void TestMapObject_CurrentLocation_Type()
        {
            var storage = new MyCustomLocation();
            var chara = Chara.Create(storage, 0, 0).Value;

            Assert.IsInstanceOf<MyCustomLocation>(chara.CurrentLocation);
            Assert.IsInstanceOf<Pool>(chara._InternalLocation);
        }
    }
}
