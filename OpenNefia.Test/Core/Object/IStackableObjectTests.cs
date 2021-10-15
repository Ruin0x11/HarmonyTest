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
    public class IStackableObjectTests
    {
        [Test]
        public void TestCanStackWithSelf()
        {
            var chipDef = new ChipDef("Test");
            var item1 = new Item(0, 0, chipDef);
            Assert.IsFalse(item1.CanStackWith(item1));
        }

        [Test]
        public void TestCanStackWithOther()
        {
            var chipDef = new ChipDef("Test");
            var item1 = new Item(0, 0, chipDef);
            var item2 = new Item(1, 1, chipDef);
            Assert.IsTrue(item1.CanStackWith(item2));
            Assert.IsTrue(item2.CanStackWith(item1));
        }

        [Test]
        public void TestCanStackWithDisposed()
        {
            var chipDef = new ChipDef("Test");
            var item1 = new Item(0, 0, chipDef);
            var item2 = new Item(1, 1, chipDef);
            item2.Dispose();
            Assert.IsFalse(item1.CanStackWith(item2));
            Assert.IsFalse(item2.CanStackWith(item1));
        }

        [Test]
        public void TestCannotStackWithOther()
        {
            var chipDef1 = new ChipDef("Test1");
            var chipDef2 = new ChipDef("Test2");

            var item1 = new Item(0, 0, chipDef1);
            var item2 = new Item(1, 1, chipDef2);

            Assert.IsFalse(item1.CanStackWith(item2));
            Assert.IsFalse(item2.CanStackWith(item1));
        }

        [Test]
        public void TestSeparateInvalidRange()
        {
            var chipDef1 = new ChipDef("Test1");

            var item1 = new Item(0, 0, chipDef1);
            item1.Amount = 10;

            Item? separated;

            Assert.Throws<ArgumentOutOfRangeException>(() => item1.Separate(-1, out separated));
            Assert.Throws<ArgumentOutOfRangeException>(() => item1.Separate(11, out separated));
        }

        [Test]
        public void TestSeparateZeroAmount()
        {
            var chipDef1 = new ChipDef("Test1");

            var item1 = new Item(0, 0, chipDef1);
            item1.Amount = 10;

            Item? separated;

            Assert.IsFalse(item1.Separate(0, out separated));
            Assert.IsNull(separated);
            Assert.AreEqual(10, item1.Amount);
        }

        [Test]
        public void TestSeparate()
        {
            var chipDef1 = new ChipDef("Test1");
            var parent = new Chara(0, 0, chipDef1);
            var item1 = new Item(0, 0, chipDef1);
            item1.Amount = 10;

            parent.Inventory.TakeObject(item1);

            Item? separated;

            Assert.IsTrue(item1.Separate(4, out separated));
            Assert.IsNotNull(separated);
            Assert.AreEqual(4, separated!.Amount);
            Assert.AreEqual(6, item1.Amount);
            Assert.IsNull(separated.CurrentLocation);
            Assert.IsNotNull(item1.CurrentLocation);
            Assert.AreNotEqual(item1.Uid, separated.Uid);
        }

        [Test]
        public void TestStackWithSame()
        {
            var chipDef1 = new ChipDef("Test1");
            var item1 = new Item(0, 0, chipDef1);

            Assert.IsFalse(item1.StackWith(item1));
        }

        [Test]
        public void TestStackWith()
        {
            var chipDef1 = new ChipDef("Test1");
            var item1 = new Item(0, 0, chipDef1);
            var item2 = new Item(0, 0, chipDef1);
            item1.Amount = 5;
            item2.Amount = 8;

            Assert.IsTrue(item1.StackWith(item2));

            Assert.IsTrue(item2.Disposed);
            Assert.AreEqual(13, item1.Amount);
            Assert.AreEqual(0, item2.Amount);

            Assert.IsFalse(item1.StackWith(item2));
        }

        [Test]
        public void TestStackAll()
        {
            var chipDef1 = new ChipDef("Test1");
            var chipDef2 = new ChipDef("Test2");
            var item1 = new Item(0, 0, chipDef1);
            var item2 = new Item(0, 0, chipDef1);
            var item3 = new Item(0, 0, chipDef2);
            var parent = new Chara(0, 0, chipDef1);

            parent.Inventory.TakeObject(item1);
            parent.Inventory.TakeObject(item2);
            parent.Inventory.TakeObject(item3);

            item1.Amount = 5;
            item2.Amount = 8;
            item3.Amount = 2;

            Assert.IsTrue(item1.StackAll());

            Assert.IsTrue(item2.Disposed);
            Assert.AreEqual(13, item1.Amount);
            Assert.AreEqual(0, item2.Amount);
            Assert.AreEqual(2, item3.Amount);

            Assert.IsFalse(item1.StackAll());
        }
    }
}
