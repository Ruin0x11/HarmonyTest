using OpenNefia.Core.Object.Aspect;
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

        public abstract Type MapObjectType { get; }

        public Love.Color Color = Love.Color.White;

        public List<AspectProperties> Aspects = new List<AspectProperties>();

        [Localize]
        public string Name = string.Empty;
    }
}
