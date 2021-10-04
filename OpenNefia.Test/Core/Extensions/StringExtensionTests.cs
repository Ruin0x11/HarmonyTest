using NUnit.Framework;
using OpenNefia.Core.Extensions;

namespace OpenNefia.Core.Extensions.Test
{
    public class StringExtensionTests
    {
        [Test]
        public void TestGetWideWidth()
        {
            Assert.AreEqual(2, "ÇŸ"[0].GetWideWidth());
            Assert.AreEqual(2, "Å@"[0].GetWideWidth());
            Assert.AreEqual(1, "a"[0].GetWideWidth());
        }

        [Test]
        public void TestWideSub()
        {
            Assert.AreEqual("abcde".Substring(0, 3), "abcde".WideSubstring(0, 3));
            Assert.AreEqual("abcde".Substring(2, 3), "abcde".WideSubstring(2, 3));
            Assert.AreEqual("", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 0));
            Assert.AreEqual("", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 1));
            Assert.AreEqual("ÇŸ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 2));
            Assert.AreEqual("ÇŸ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 3));
            Assert.AreEqual("ÇŸÇ∞", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 4));
            Assert.AreEqual("ÇŸÇ∞a", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 5));
            Assert.AreEqual("ÇŸÇ∞a", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 6));
            Assert.AreEqual("ÇŸÇ∞aÇ“", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 7));
            Assert.AreEqual("ÇŸÇ∞aÇ“", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 8));
            Assert.AreEqual("ÇŸÇ∞aÇ“ÇÊ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 9));
            Assert.AreEqual("ÇŸÇ∞aÇ“ÇÊ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 10));
            Assert.AreEqual("ÇŸÇ∞aÇ“ÇÊ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0, 99));
            Assert.AreEqual("", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(99, 0));
            Assert.AreEqual("ÇŸÇ∞aÇ“ÇÊ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(0));
            Assert.AreEqual("aÇ“ÇÊ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(3, 6));
            Assert.AreEqual("aÇ“ÇÊ", "ÇŸÇ∞aÇ“ÇÊ".WideSubstring(3));
        }
    }
}