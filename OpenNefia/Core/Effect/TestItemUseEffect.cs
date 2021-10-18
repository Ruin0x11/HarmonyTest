using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect
{
    internal class TestItemUseEffect : IEffect
    {
        public EffectResult Apply()
        {
            return EffectResult.Succeeded;
        }
    }
}
