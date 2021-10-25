using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Effect;
using OpenNefia.Core.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object.Aspect
{
    public class PotionAspect : MapObjectAspect, IEffectArgsProvider, ICanDrinkAspect, ICanThrowAspect
    {
        public PotionAspectProps PotionProps => (PotionAspectProps)this.Props;

        public PotionAspect(MapObject owner) : base(owner)
        {
        }

        public override void Initialize(AspectProperties props)
        {
            base.Initialize(props);
        }

        #region IEffectArgsGenerator implementation.

        public virtual EffectArguments GetEffectArgs(MapObject obj, TriggeredBy triggeredBy)
        {
            MapObject potionItem = this.Owner;
            return PotionProps.EffectArgs.ToArgs(obj, source: potionItem, triggeredBy: triggeredBy);
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
            this.PotionProps.Effect.Apply(this.GetEffectArgs(chara, TriggeredBy.Drinking));
            return TurnResult.TurnEnd;
        }

        #endregion

        #region ICanBeThrownAspect

        // NOTE: How many should be destroyed? Is it always 1?
        public bool ShouldDestroyOnThrow => true;

        public virtual bool CanThrow(Chara chara)
        {
            return true;
        }

        public virtual void OnThrownImpact(InstancedMap map, int x, int y)
        {
            var chara = map.MapObjectsAt<Chara>(x, y).FirstOrDefault();
            if (chara != null)
            {
                this.PotionProps.Effect.Apply(this.GetEffectArgs(chara, TriggeredBy.ThrownItem));
            }
            else
            {
                //Feat puddle = Feat.Create(FeatDefOf.DrinkablePuddle, map, x, y);

                //var props = new Feat_DrinkablePuddleProps()
                //{
                //    Power = this.PotionProps.Power,
                //    EffectParams = this.PotionProps.EffectParams,
                //};
                //IAspect aspect = Aspect.CreateFromProps<Feat_DrinkablePuddle>(props);

                //puddle.AddAspect(aspect);
            }
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
