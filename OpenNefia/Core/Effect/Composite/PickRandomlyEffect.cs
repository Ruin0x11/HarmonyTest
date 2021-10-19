using OpenNefia.Core.Data.Serial;
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

        public override EffectResult Apply(EffectArguments args)
        {
            var result = Effects[Random.Rnd(Effects.Count)].Apply(args);
            return result;
        }
    }
}
