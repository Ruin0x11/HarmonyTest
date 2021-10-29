using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object.Aspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public class Mef_Potion : Mef
    {
        public Mef_Potion(MefDef def) : base(def)
        {
        }

        public override void OnSteppedOn(Chara chara)
        {
            Sounds.PlayOneShot(SoundDefOf.Water, chara.GetTilePos()!.Value);
            foreach (var aspect in this.GetAspects<ICanDrinkAspect>())
            {
                aspect.OnDrink(chara);
            }
            this.Destroy();
        }
    }
}
