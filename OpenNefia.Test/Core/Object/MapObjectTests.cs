using NUnit.Framework;
using OpenNefia.Core;
using OpenNefia.Core.Data.Types;
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
        private class MyCustomLocation : IMapObjectHolder
        {
            private Pool _Pool;
            public Pool InnerPool { get => _Pool; }

            private IMapObjectHolder? _ParentHolder;
            public IMapObjectHolder? ParentHolder => this._ParentHolder;

            public MyCustomLocation(IMapObjectHolder? parent = null)
            {
                this._ParentHolder = parent;
                this._Pool = new Pool<MapObject>(this);
            }

            public void Expose(DataExposer data)
            {
                data.ExposeDeep(ref _Pool, nameof(Pool));
            }

            public void GetChildPoolOwners(List<IMapObjectHolder> outOwners)
            {
            }
        }

        [Test]
        public void TestMapObject_ParentHolder_None()
        {
            var chara = CharaGen.Create(new CharaDef("Test")).Value;

            Assert.IsNull(chara._PoolContainingMe);
            Assert.IsFalse(chara.IsOwned);
        }

        [Test]
        public void TestMapObject_ParentHolder_Nested()
        {
            var storage = new MyCustomLocation();
            var storage2 = new MyCustomLocation(storage);
            var chara = CharaGen.Create(new CharaDef("Test"), storage).Value;

            Assert.IsTrue(chara.IsOwned);
            Assert.AreEqual(storage, chara.ParentHolder);
        }

        [Test]
        public void TestMapObject_ParentHolder_TopLevel()
        {
            var storage = new MyCustomLocation();
            var storage2 = new MyCustomLocation(storage);
            var chara = CharaGen.Create(new CharaDef("Test"), storage2).Value;

            Assert.AreEqual(storage2, chara.ParentHolder);
        }

        [Test]
        public void TestMapObject_ParentHolder_Type()
        {
            var storage = new MyCustomLocation();
            var chara = CharaGen.Create(new CharaDef("Test"), storage).Value;

            Assert.IsInstanceOf<MyCustomLocation>(chara.ParentHolder);
            Assert.IsInstanceOf<Pool>(chara._PoolContainingMe);
        }
    }
}
