using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect.Impl
{
    public class EffectHealing : BaseEffect
    {
        public override EffectResult Apply(Chara chara, EffectArguments args)
        {
            Messages.Print($"{chara.Def.Name} is (supposed to be) healed.", ColorDefOf.MesGreen);

            var drawable = new ParticleMapDrawable(AssetDefOf.HealEffect, SoundDefOf.Heal1, 5f);
            FieldLayer.Instance!.MapDrawables.Enqueue(drawable, chara.GetTilePos());

            return EffectResult.Succeeded;
        }
    }
}