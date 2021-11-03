using NUnit.Framework;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Logic;
using OpenNefia.Core.Object;

namespace OpenNefia.Test.Core.Object
{
    public class ItemTests
    {
        [Test]
        public void TestGetOwningCharaNone()
        {
            var item = new Item(new ItemDef("Test"));

            Assert.IsNull(item.GetOwningChara());
        }

        [Test]
        public void TestGetOwningCharaSome()
        {
            var item = new Item(new ItemDef("Test"));
            var chara = new Chara(new CharaDef("Test"));
            chara.Inventory.TakeItem(item);

            Assert.AreEqual(chara, item.GetOwningChara());
        }

        [Test]
        public void TestGetOwningCharaNested()
        {
            var itemDef = new ItemDef("Test");
            var item = new Item(itemDef);
            var container = new Item(itemDef);
            var chara = new Chara(new CharaDef("Test"));

            var inv = new ItemInventory(container);

            inv.TakeItem(item);
            CharaAction.PickUpItem(chara, container);

            Assert.AreEqual(chara, container.GetOwningChara());
            Assert.AreEqual(chara, item.GetOwningChara());
        }
    }
}
