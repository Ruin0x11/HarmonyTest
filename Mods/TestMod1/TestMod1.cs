using OpenNefia;
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
        }
    }
}
