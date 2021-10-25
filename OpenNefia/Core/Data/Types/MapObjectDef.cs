using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public abstract class MapObjectDef : Def
    {
        internal MapObjectDef(string id) : base(id)
        {
        }

        public AspectDefinitions Aspects = new AspectDefinitions();
    }
}
