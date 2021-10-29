using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect.Composite
{
    public class PickRandomlyEffect : BaseEffect
    {
        [DefRequired]
        public List<IEffect> Effects;

        public PickRandomlyEffect() : this(new List<IEffect>()) { }

        public PickRandomlyEffect(List<IEffect> effects)
        {
            Effects = effects;
        }

        public override EffectResult Apply(Chara chara, EffectArguments args)
        {
            var result = Effects[Rand.NextInt(Effects.Count)].Apply(chara, args);
            return result;
        }
    }
}
