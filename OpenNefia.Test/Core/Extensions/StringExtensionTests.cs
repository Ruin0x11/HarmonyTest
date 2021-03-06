using NUnit.Framework;
using OpenNefia.Core.Extensions;

namespace OpenNefia.Core.Extensions.Test
{
    public class StringExtensionTests
    {
        [Test]
        public void TestGetWideWidth()
        {
            Assert.AreEqual(2, "ほ"[0].GetWideWidth());
            Assert.AreEqual(2, "　"[0].GetWideWidth());
            Assert.AreEqual(1, "a"[0].GetWideWidth());
        }

        [Test]
        public void TestWideSub()
        {
            Assert.AreEqual("abcde".Substring(0, 3), "abcde".WideSubstring(0, 3));
            Assert.AreEqual("abcde".Substring(2, 3), "abcde".WideSubstring(2, 3));
            Assert.AreEqual("", "ほげaぴよ".WideSubstring(0, 0));
            Assert.AreEqual("", "ほげaぴよ".WideSubstring(0, 1));
            Assert.AreEqual("ほ", "ほげaぴよ".WideSubstring(0, 2));
            Assert.AreEqual("ほ", "ほげaぴよ".WideSubstring(0, 3));
            Assert.AreEqual("ほげ", "ほげaぴよ".WideSubstring(0, 4));
            Assert.AreEqual("ほげa", "ほげaぴよ".WideSubstring(0, 5));
            Assert.AreEqual("ほげa", "ほげaぴよ".WideSubstring(0, 6));
            Assert.AreEqual("ほげaぴ", "ほげaぴよ".WideSubstring(0, 7));
            Assert.AreEqual("ほげaぴ", "ほげaぴよ".WideSubstring(0, 8));
            Assert.AreEqual("ほげaぴよ", "ほげaぴよ".WideSubstring(0, 9));
            Assert.AreEqual("ほげaぴよ", "ほげaぴよ".WideSubstring(0, 10));
            Assert.AreEqual("ほげaぴよ", "ほげaぴよ".WideSubstring(0, 99));
            Assert.AreEqual("", "ほげaぴよ".WideSubstring(99, 0));
            Assert.AreEqual("ほげaぴよ", "ほげaぴよ".WideSubstring(0));
            Assert.AreEqual("aぴよ", "ほげaぴよ".WideSubstring(3, 6));
            Assert.AreEqual("aぴよ", "ほげaぴよ".WideSubstring(3));
        }
    }
}