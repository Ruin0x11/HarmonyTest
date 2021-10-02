using OpenNefia;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod1
{
    class PutitThing : IThingData
    {
        public string ID => "PutitThing";

        public IResourcePath Image => new ModLocalPath(typeof(TestMod1), "Graphic/putitgrammingFrame1.png");
    }
}
