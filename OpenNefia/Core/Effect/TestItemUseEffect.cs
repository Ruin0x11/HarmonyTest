using OpenNefia.Core.Object;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Effect
{
    internal class TestItemUseEffect : IEffect<Item, Chara>
    {
        public EffectResult Apply(Item source, Chara target)
        {
            Console.WriteLine($"I've been used: {source} - on {target}");

            return EffectResult.Succeeded;
        }
    }
}
