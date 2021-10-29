using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Effect;
using OpenNefia.Core.Logic;
using OpenNefia.Core.Map;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object.Aspect.Types
{
    public class PotionAspect : MapObjectAspect, IEffectArgsProvider, ICanDrinkAspect, ICanThrowAspect
    {
        public PotionAspectProps PotionProps => (PotionAspectProps)Props;

        public PotionAspect(MapObject owner) : base(owner)
        {
        }

        public override void Initialize(AspectProperties props)
        {
            base.Initialize(props);
        }

        #region IEffectArgsGenerator implementation.

        public virtual EffectArguments GetEffectArgs(Chara chara, TriggeredBy triggeredBy)
        {
            MapObject potionItem = Owner;
            return PotionProps.EffectArgs.ToArgs(source: potionItem, triggeredBy: triggeredBy);
        }

        #endregion

        #region ICanDrinkAspect implementation. 

        // NOTE: How many should be consumed? Is it always 1?
        public bool ShouldConsumeOnDrink => true;

        public virtual bool CanDrink(Chara chara)
        {
            return true;
        }

        public virtual TurnResult OnDrink(Chara chara)
        {
            PotionProps.Effect.Apply(chara, GetEffectArgs(chara, TriggeredBy.Drinking));
            return TurnResult.TurnEnd;
        }

        #endregion

        #region ICanBeThrownAspect

        public virtual bool CanThrow(Chara chara)
        {
            return true;
        }

        public virtual bool OnThrownImpact(TilePos pos)
        {
            var chara = pos.GetPrimaryChara();

            if (chara != null)
            {
                PotionProps.Effect.Apply(chara, GetEffectArgs(chara, TriggeredBy.ThrownItem));
            }
            else
            {
                var puddle = MefGen.Create(MefDefOf.Potion, pos, new MefGenOpts() { ExtraAspects = new List<AspectProperties>() { this.Props }});
                if (puddle.IsSuccess)
                {
                    puddle.Value.Color = this.Owner.Color;
                }
            }

            return true;
        }

        #endregion
    }

    [AspectClass(typeof(PotionAspect))]
    public class PotionAspectProps : AspectProperties
    {
        // Power, range, curse state.
        [DefRequired]
        public EffectArgumentsBase EffectArgs = null!;

        [DefRequired]
        public IEffect Effect = null!;

        public PotionAspectProps()
        {
        }
    }
}
