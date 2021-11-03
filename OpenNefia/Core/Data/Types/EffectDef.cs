using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Effect;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OpenNefia.Core.Data.Types
{
    public class EffectDef : Def
    {
        public EffectDef(string id) : base(id)
        {
        }

        [DefRequired]
        public IEffect Effect = null!;

        public EffectResult Apply(Chara chara, EffectArguments args) => Effect.Apply(chara, args);
    }
}
