using NUnit.Framework;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Test.Core.Data
{
    public class DefTests
    {
        [Test]
        public void TestGetDirectDefType()
        {
            var def = new AssetDef("Test");
            Assert.AreEqual(typeof(AssetDef), def.GetDirectDefType());

            var def2 = new CharaDef("Test");
            Assert.AreEqual(typeof(CharaDef), def2.GetDirectDefType());
        }
    }
}
