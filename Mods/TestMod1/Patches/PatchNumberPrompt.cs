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
    [HarmonyPatch(typeof(NumberPrompt), nameof(NumberPrompt.Query))]
    class PatchNumberPrompt
    {
        static bool Prefix(NumberPrompt __instance, ref UiResult<int> __result)
        {
            Console.WriteLine("Running custom number prompt!");
            var myNumberPrompt = new MyNumberPrompt();
            __result = myNumberPrompt.Query();
            return false;
        }
    }
}
