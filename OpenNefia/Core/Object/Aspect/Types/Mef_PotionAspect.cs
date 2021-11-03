using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Effect;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object.Aspect.Types
{
    public class Mef_PotionAspect : MapObjectAspect, ICanStepOnAspect
    {
        public List<EffectAndPower> Effects = new List<EffectAndPower>();

        public Mef_PotionAspect(MapObject owner) : base(owner)
        {
        }

        public void Event_OnSteppedOn(Chara chara)
        {
            Sounds.PlayOneShot(SoundDefOf.Water, chara.GetTilePos()!.Value);
            foreach (var effect in Effects)
            {
                effect.Apply(chara, this.Owner);
            }
            this.Owner.Destroy();
        }

        public override void AfterExposeData(DataExposer data)
        {
            data.ExposeCollection(ref Effects, nameof(Effects));
        }
    }

    [AspectClass(typeof(Mef_PotionAspect))]
    public class Mef_PotionAspectProps : AspectProperties
    {
    }
}
