using NUnit.Framework;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Patch;
using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Game;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenNefia.Test.Core.Data.Patch
{
    public class DefDeserializerTests
    {
        [Test]
        public void TestGetDefIdAndTypeFromNode()
        {
            Engine.ModLoader.Execute();
            DefTypes.ScanAllTypes();

            var xml = @"
<Defs>
  <AssetDef Id=""Test"">
    <a><b><c/></b></a>
  </AssetDef>
</Defs>
";
            var doc = XDocument.Parse(xml);

            var elem = doc.XPathSelectElement("/Defs/AssetDef[@Id='Test']")!;
            Assert.AreEqual(new DefIdentifier(typeof(AssetDef), "Test"), DefDeserializer.GetDefIdAndTypeFromElement(elem).Value);
        }
    }
}
