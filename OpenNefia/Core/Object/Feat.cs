using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public class Feat : MapObject
    {
        public Feat(FeatDef def) : base(def)
        {
        }

        public override bool IsInLiveState => true;

        public override void ProduceMemory(MapObjectMemory memory)
        {
        }

        public override void Refresh()
        {
        }
    }
}
