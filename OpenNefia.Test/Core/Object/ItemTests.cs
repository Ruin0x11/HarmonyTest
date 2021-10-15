using NUnit.Framework;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;

namespace OpenNefia.Test.Core.Object
{
    public class ItemTests
    {
        [Test]
        public void TestGetOwningCharaNone()
        {
            var item = new Item(new ChipDef("Test"));

            Assert.IsNull(item.GetOwningChara());
        }

        [Test]
        public void TestGetOwningCharaSome()
        {
            var chip = new ChipDef("Test");
            var item = new Item(chip);
            var chara = new Chara(chip);
            chara.Inventory.TakeObject(item);

            Assert.AreEqual(chara, item.GetOwningChara());
        }

        [Test]
        public void TestGetOwningCharaNested()
        {
            var chip = new ChipDef("Test");
            var item = new Item(chip);
            var container = new Item(chip);
            var chara = new Chara(chip);

            var inv = new ItemInventory(container);

            inv.TakeObject(item);
            chara.TakeItem(container);

            Assert.AreEqual(chara, container.GetOwningChara());
            Assert.AreEqual(chara, item.GetOwningChara());
        }
    }
}
