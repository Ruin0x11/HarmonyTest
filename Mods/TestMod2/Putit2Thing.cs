using OpenNefia;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod2
{
    class Putit2Thing : IThingData
    {
        public string ID => "Putit2Thing";

        public IResourcePath Image => new ModLocalPath(typeof(TestMod2), "Graphic/putitgrammingFrame1.png");
    }
}
