using NUnit.Framework;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Test.Core.Object
{
    public class ItemTests_Stacking
    {
        [Test]
        public void TestCanStackWithSelf()
        {
            var itemDef = new ItemDef("Test");
            var item1 = new Item(itemDef);
            Assert.IsFalse(item1.CanStackWith(item1));
        }

        [Test]
        public void TestCanStackWithOther()
        {
            var itemDef = new ItemDef("Test");
            var item1 = new Item(itemDef);
            var item2 = new Item(itemDef);
            Assert.IsTrue(item1.CanStackWith(item2));
            Assert.IsTrue(item2.CanStackWith(item1));
        }

        [Test]
        public void TestCanStackWithDisposed()
        {
            var itemDef = new ItemDef("Test");
            var item1 = new Item(itemDef);
            var item2 = new Item(itemDef);
            item2.Destroy();
            Assert.IsFalse(item1.CanStackWith(item2));
            Assert.IsFalse(item2.CanStackWith(item1));
        }

        [Test]
        public void TestCannotStackWithOther()
        {
            var itemDef1 = new ItemDef("Test1");
            var itemDef2 = new ItemDef("Test2");

            var item1 = new Item(itemDef1);
            var item2 = new Item(itemDef2);

            Assert.IsFalse(item1.CanStackWith(item2));
            Assert.IsFalse(item2.CanStackWith(item1));
        }

        [Test]
        public void TestSeparateInvalidRange()
        {
            var itemDef1 = new ItemDef("Test1");

            var item1 = new Item(itemDef1);
            item1.Amount = 10;

            Assert.IsNull(item1.SplitOff(-1));
            Assert.IsNull(item1.SplitOff(0));
            var split = item1.SplitOff(11);
            Assert.IsNotNull(split);
            Assert.AreEqual(10, split!.Amount);
            Assert.AreEqual(0, item1.Amount);
            Assert.IsFalse(item1.IsAlive);
        }

        [Test]
        public void TestSeparateZeroAmount()
        {
            var itemDef1 = new ItemDef("Test1");

            var item1 = new Item(itemDef1);
            item1.Amount = 10;

            Item? separated = item1.SplitOff(0);

            Assert.IsNull(separated);
            Assert.AreEqual(10, item1.Amount);
        }

        [Test]
        public void TestSeparate()
        {
            var itemDef1 = new ItemDef("Test1");
            var parent = new Chara(new CharaDef("Test"));
            var item1 = new Item(itemDef1);
            item1.Amount = 10;

            parent.Inventory.TakeItem(item1);

            Item? separated = item1.SplitOff(4);

            Assert.IsNotNull(separated);
            Assert.AreEqual(4, separated!.Amount);
            Assert.AreEqual(6, item1.Amount);
            Assert.IsNull(separated.ParentHolder);
            Assert.IsNotNull(item1.ParentHolder);
            Assert.AreNotEqual(item1.Uid, separated.Uid);
        }

        [Test]
        public void TestStackWithSame()
        {
            var itemDef1 = new ItemDef("Test1");
            var item1 = new Item(itemDef1);

            Assert.IsFalse(item1.StackWith(item1));
        }

        [Test]
        public void TestStackWith()
        {
            var itemDef1 = new ItemDef("Test1");
            var item1 = new Item(itemDef1);
            var item2 = new Item(itemDef1);
            item1.Amount = 5;
            item2.Amount = 8;

            Assert.IsTrue(item1.StackWith(item2));

            Assert.IsTrue(item2.Destroyed);
            Assert.AreEqual(13, item1.Amount);
            Assert.AreEqual(0, item2.Amount);

            Assert.IsFalse(item1.StackWith(item2));
        }

        [Test]
        public void TestStackAll()
        {
            var itemDef1 = new ItemDef("Test1");
            var itemDef2 = new ItemDef("Test2");
            var item1 = new Item(itemDef1);
            var item2 = new Item(itemDef1);
            var item3 = new Item(itemDef2);
            var parent = new Chara(new CharaDef("Test"));

            parent.Inventory.TakeItem(item1);
            parent.Inventory.TakeItem(item2);
            parent.Inventory.TakeItem(item3);

            item1.Amount = 5;
            item2.Amount = 8;
            item3.Amount = 2;

            Assert.IsTrue(item1.StackAll());

            Assert.IsTrue(item2.Destroyed);
            Assert.AreEqual(13, item1.Amount);
            Assert.AreEqual(0, item2.Amount);
            Assert.AreEqual(2, item3.Amount);

            Assert.IsFalse(item1.StackAll());
        }
    }
}
