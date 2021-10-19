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
        public override EffectResult Apply(EffectArguments args)
        {
            var map = Chara.Player!.GetCurrentMap()!;
            for (int i = 0; i < 10; i++)
            {
                Chara.Create(map, Random.Rnd(map.Width), Random.Rnd(map.Height));
            }

            return EffectResult.Succeeded;
        }
    }
}
