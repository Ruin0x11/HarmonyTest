using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class MefDef : MapObjectDef
    {
        public MefDef(string id) : base(id)
        {
        }

        public override Type MapObjectType => MefClass;
        
        [DefRequired]
        public Type MefClass = null!;

        [DefRequired]
        public ChipDef Chip = null!;
    }
}
