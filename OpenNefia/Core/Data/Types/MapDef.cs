using OpenNefia.Core.Map.Generator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class MapDef : Def
    {
        public MapDef(string id) : base(id)
        {
        }

        public IMapGenerator Generator = new EmptyMapGenerator();
    }
}
