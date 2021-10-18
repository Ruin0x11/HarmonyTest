using OpenNefia.Core.Data.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect.Composite
{
    public class SequenceEffect : BaseEffect
    {
        [DefRequired]
        public List<IEffect> Effects;

        public SequenceEffect() : this(new List<IEffect>()) { }

        public SequenceEffect(List<IEffect> effects)
        {
            Effects = effects;
        }

        public override EffectResult Apply(EffectArguments args)
        {
            var result = EffectResult.Succeeded;

            foreach (var effect in Effects)
            {
                var childResult = effect.Apply(args);
                if (childResult != EffectResult.Succeeded)
                    result = childResult;
            }

            return result;
        }
    }
}
