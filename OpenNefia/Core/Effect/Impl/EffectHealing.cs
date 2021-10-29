using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.Rendering.AsyncDrawable;
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
            var drawable = new ParticleAsyncDrawable(AssetDefOf.HealEffect, SoundDefOf.Heal1, 5f);
            FieldLayer.Instance!.AsyncDrawables.Enqueue(drawable, chara.GetTilePos());

            return EffectResult.Succeeded;
        }
    }
}