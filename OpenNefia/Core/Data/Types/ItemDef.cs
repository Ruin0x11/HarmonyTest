using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class ItemDef : MapObjectDef
    {
        public ItemDef(string id) : base(id)
        {
        }
        
        [DefRequired]
        public ChipDef Chip = null!;
    }
}
