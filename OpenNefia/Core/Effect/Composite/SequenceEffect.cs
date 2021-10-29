using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;
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

        public override EffectResult Apply(Chara chara, EffectArguments args)
        {
            var result = EffectResult.Succeeded;

            foreach (var effect in Effects)
            {
                var childResult = effect.Apply(chara, args);
                if (childResult != EffectResult.Succeeded)
                    result = childResult;
            }

            return result;
        }
    }
}
