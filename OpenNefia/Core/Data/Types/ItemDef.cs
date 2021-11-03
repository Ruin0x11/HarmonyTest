using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using System;

namespace OpenNefia.Core.Data.Types
{
    public class ItemDef : MapObjectDef
    {
        public ItemDef(string id) : base(id)
        {
        }

        public override Type MapObjectType => typeof(Item);

        [LocalizeOptional]
        public string? UnidentifiedName;

        [DefRequired]
        public ChipDef Chip = null!;
    }
}
