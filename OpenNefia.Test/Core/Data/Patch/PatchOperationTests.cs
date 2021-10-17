using NUnit.Framework;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Patch;
using OpenNefia.Core.Data.Types;
using System.Linq;
using System.Xml;

namespace OpenNefia.Test.Core.Data.Patch
{
    public class PatchOperationTests
    {
        [Test]
        public void TestNodeToAffectedDefs()
        {
            DefTypes.ScanAllTypes();

            var xml = @"
<Defs>
  <AssetDef Id=""Test"">
    <a><b><c/></b></a>
  </AssetDef>
</Defs>
";
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var nav = doc.CreateNavigator()!;
            var node = nav.SelectSingleNode("/Defs/AssetDef[@Id='Test']/a/b/c")!;

            var result = PatchOperation.NodeToAffectedDefs(node);
            Assert.AreEqual(1, result.AffectedDefs.Count());

            var list = result.AffectedDefs.ToList();
            Assert.AreEqual(typeof(AssetDef), list[0].DefType);
            Assert.AreEqual("Test", list[0].DefId);

            result = PatchOperation.NodeToAffectedDefs(doc.CreateNavigator());
            Assert.AreEqual(0, result.AffectedDefs.Count());
        }
    }
}
