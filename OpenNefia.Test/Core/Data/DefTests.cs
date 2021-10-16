using NUnit.Framework;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static OpenNefia.Core.Rendering.GraphicsEx;

namespace OpenNefia.Test.Core.Data
{
    public class DefTests
    {
        private static Ref<FontDef> TestFontDef = new FontDef("TestFontDef", 14, 12, FontFormatting.None);

        [Test]
        public void TestDefHotload()
        {
            var newFontDef = new Ref<FontDef>(new FontDef("NewFontDef", 16, 14, FontFormatting.Bold, null, null, null));

            Assert.AreEqual(FontFormatting.None, TestFontDef.Val.Formatting);
            Assert.AreEqual(14, TestFontDef.Val.Size);
            Assert.AreEqual(12, TestFontDef.Val.SmallSize);
            Assert.AreEqual(FontFormatting.Bold, newFontDef.Val.Formatting);
            Assert.AreEqual(16, newFontDef.Val.Size);
            Assert.AreEqual(14, newFontDef.Val.SmallSize);
            Assert.AreNotEqual(TestFontDef, newFontDef);

            TestFontDef.SetReference(newFontDef);

            Assert.AreEqual(FontFormatting.Bold, TestFontDef.Val.Formatting);
            Assert.AreEqual(16, TestFontDef.Val.Size);
            Assert.AreEqual(14, TestFontDef.Val.SmallSize);
            Assert.AreEqual(FontFormatting.Bold, newFontDef.Val.Formatting);
            Assert.AreEqual(16, newFontDef.Val.Size);
            Assert.AreEqual(14, newFontDef.Val.SmallSize);
            Assert.AreNotEqual(TestFontDef, newFontDef);
        }
    }
}
