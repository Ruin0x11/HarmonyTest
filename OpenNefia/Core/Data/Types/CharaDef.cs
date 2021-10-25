using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class CharaDef : MapObjectDef
    {
        public CharaDef(string id) : base(id)
        {
        }
        
        [DefRequired]
        public ChipDef Chip = null!;
    }
}
