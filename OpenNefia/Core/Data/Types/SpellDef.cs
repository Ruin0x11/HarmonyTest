using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Effect;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class SpellDef : Def, IEffect
    {
        internal SpellDef(string id) : this(id, null!)
        {
        }

        public SpellDef(string id, IEffect onCastEffect) : base(id)
        {
            this.EffectOnCast = onCastEffect;
        }

        [DefRequired]
        public IEffect EffectOnCast;

        public SpellType SpellType;
        
        public SpellAlignment Alignment;

        public int MPCost = 0;

        public int StaminaCost = 0;

        public int TileRange;

        public int CastDifficulty;

        public TargetType TargetType;

        public EffectResult Apply(Chara chara, EffectArguments args) => this.EffectOnCast.Apply(chara, args);
    }
}
