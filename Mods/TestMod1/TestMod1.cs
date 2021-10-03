using HarmonyLib;
using OpenNefia;
using OpenNefia.Core.UI.Layer;
using OpenNefia.Mod;
using System;

namespace TestMod1
{
    [ModEntry("26feb706-4ed0-4250-a84e-d701cdf43942", "Test Mod 1", "0.1.0")]
    public class TestMod1 : BaseMod
    {
        public override void Load()
        {
            ThingRepo.Instance.Register(new PutitThing());
            Console.WriteLine("Loaded mod 1.");



            //var harmony = new Harmony("com.example.patch");

            ////
            //var mOriginal = AccessTools.Method(typeof(NumberPrompt), nameof(NumberPrompt.Query));
            //var mPrefix = SymbolExtensions.GetMethodInfo(() => new MyNumberPrompt().Query());
            //// in general, add null checks here (new HarmonyMethod() does it for you too)

            //harmony.Patch(mOriginal, new HarmonyMethod(mPrefix));
        }
    }
}
