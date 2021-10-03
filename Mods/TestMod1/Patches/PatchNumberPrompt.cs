using HarmonyLib;
using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod1
{
    [HarmonyPatch]
    class PatchNumberPrompt
    {
        [HarmonyPatch(typeof(NumberPrompt), nameof(NumberPrompt.Query))]
        static bool Prefix(ref UiResult<int> __result)
        {
            Console.WriteLine("Running custom number prompt!");
            var myNumberPrompt = new MyNumberPrompt();
            __result = myNumberPrompt.Query();
            return true;
        }
    }
}
