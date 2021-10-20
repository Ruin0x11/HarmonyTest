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
    internal class TestEffect : BaseEffect
    {
        public override EffectResult Apply(EffectArguments args)
        {
            FieldLayer.Instance!.AsyncDrawables.Enqueue(new BasicAnimAsyncDrawable(BasicAnimDefOf.AnimCurse));

            return EffectResult.Succeeded;
        }
    }
}
