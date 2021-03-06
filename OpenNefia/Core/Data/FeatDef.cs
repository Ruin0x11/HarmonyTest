using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using System;

namespace OpenNefia.Core.Data.Types
{
    public class FeatDef : MapObjectDef
    {
        public FeatDef(string id) : base(id)
        {
        }

        public override Type MapObjectType => typeof(Item);

        [DefRequired]
        public ChipDef Chip = null!;
    }
}
