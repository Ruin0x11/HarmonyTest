using NUnit.Framework;
using OpenNefia.Core.Stat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Test.Core.Stat
{
    public class ValueStatTests
    {
        [Test]
        public void TestValueStatEquality()
        {
            var stat1 = new ValueStat<int>(1);
            var stat2 = new ValueStat<int>(1);
            Assert.AreEqual(stat1, stat2);
            Assert.IsTrue(stat1 == stat2);
        }

        [Test]
        public void TestValueStatInequalityBase()
        {
            var stat1 = new ValueStat<int>(1);
            var stat2 = new ValueStat<int>(2);
            Assert.AreNotEqual(stat1, stat2);
            Assert.IsTrue(stat1 != stat2);
        }

        [Test]
        public void TestValueStatInequalityFinal()
        {
            var stat1 = new ValueStat<int>(1);
            var stat2 = new ValueStat<int>(1);
            stat2.FinalValue = 2;
            Assert.AreNotEqual(stat1, stat2);
            Assert.IsTrue(stat1 != stat2);
        }

        [Test]
        public void TestValueStatRefresh()
        {
            var stat1 = new ValueStat<int>(1);
            stat1.FinalValue = 2;
            stat1.Refresh();
            Assert.AreEqual(1, stat1.BaseValue);
            Assert.AreEqual(1, stat1.FinalValue);
        }
    }
}
