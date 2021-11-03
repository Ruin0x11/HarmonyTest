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
    public class Item_PotionAspect : MapObjectAspect, IEffectArgsProvider, ICanDrinkAspect, ICanThrowAspect
    {
        [Localize]
        private static LocaleFunc<Chara, Item> TextDrinksPotion = null!;
        [Localize]
        private static string TextThrownShatters = string.Empty;
        [Localize]
        private static LocaleFunc<Chara> TextThrownHits = null!;

        public new Item_PotionAspectProps Props => (Item_PotionAspectProps)base.Props;

        public Item_PotionAspect(MapObject owner) : base(owner)
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
            return Props.EffectArgs.ToArgs(source: potionItem, triggeredBy: triggeredBy);
        }

        #endregion

        #region ICanDrinkAspect implementation. 

        // NOTE: How many should be consumed? Is it always 1?
        public bool ShouldConsumeOnDrink => true;

        public virtual bool Event_CanDrink(Chara chara)
        {
            return true;
        }

        public virtual TurnResult Event_OnDrink(Chara chara)
        {
            var item = Owner as Item;
            TriggeredBy triggeredBy;
            if (item != null)
            {
                Messages.Print(TextDrinksPotion(chara, item));
                triggeredBy = TriggeredBy.Drinking;
            }
            else
            {
                triggeredBy = TriggeredBy.Trap;
            }
            Props.Effect.Apply(chara, GetEffectArgs(chara, triggeredBy));
            return TurnResult.TurnEnd;
        }

        #endregion

        #region ICanBeThrownAspect

        public virtual bool Event_CanThrow(Chara chara)
        {
            return true;
        }

        public virtual bool Event_OnThrownImpact(TilePos pos)
        {
            var chara = pos.GetPrimaryChara();

            if (chara != null)
            {
                Messages.Print(TextThrownHits(chara), ColorDefOf.MesWhite);
                Props.Effect.Apply(chara, GetEffectArgs(chara, TriggeredBy.ThrownItem));
            }
            else
            {
                Messages.Print(TextThrownShatters, ColorDefOf.MesWhite);
                var result = MefGen.Create(MefDefOf.Potion, pos);
                if (result.IsSuccess)
                {
                    var puddle = result.Value;
                    puddle.Color = this.Owner.Color;
                    puddle.GetAspect<Mef_PotionAspect>().Effects.Add(new EffectAndPower(this.Props.Effect, this.Props.EffectArgs));
                }
            }

            return true;
        }

        #endregion
    }

    [AspectClass(typeof(Item_PotionAspect))]
    public class Item_PotionAspectProps : AspectProperties
    {
        [DefRequired]
        public EffectDef Effect = null!;

        [DefRequired]
        public EffectArgumentsBase EffectArgs = null!;

        public Item_PotionAspectProps()
        {
        }
    }
}
