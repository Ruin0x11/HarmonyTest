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
    internal class TestEffect2 : BaseEffect
    {
        public override EffectResult Apply(Chara chara, EffectArguments args)
        {
            var map = chara.GetContainingMap()!;
            for (int i = 0; i < 10; i++)
            {
                CharaGen.Create(CharaDefOf.Putit, map.AtPos(Rand.NextInt(map.Width), Rand.NextInt(map.Height)));
            }

            return EffectResult.Succeeded;
        }
    }
}
