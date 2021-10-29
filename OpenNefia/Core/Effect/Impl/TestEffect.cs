using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Layer;

namespace OpenNefia.Core.Effect.Impl
{
    internal class TestEffect : BaseEffect
    {
        public override EffectResult Apply(Chara chara, EffectArguments args)
        {
            FieldLayer.Instance!.AsyncDrawables.Enqueue(new BasicAnimAsyncDrawable(BasicAnimDefOf.AnimCurse), chara.GetTilePos());

            return EffectResult.Succeeded;
        }
    }
}
