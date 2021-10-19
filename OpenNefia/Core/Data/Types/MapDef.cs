using OpenNefia.Core.Data.Serial;
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
        public MapDef(string id, IMapGenerator generator) : base(id)
        {
            this.Generator = generator;
        }

        public MapDef(string id) : this(id, new EmptyMapGenerator())
        {
        }

        [DefRequired]
        public IMapGenerator Generator;
    }
}
