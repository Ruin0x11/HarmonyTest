using OpenNefia;
using OpenNefia.Mod;
using System;

namespace TestMod2
{
    [ModEntry("edeb46c1-1864-4bdf-a53f-6f8ac84f02b6", "Test Mod 2", "0.1.0")]
    public class TestMod2 : BaseMod
    {
        public override void Load()
        {
            ThingRepo.Instance.Register(new Putit2Thing());
            Console.WriteLine("Loaded mod 2, dood.");
        }
    }
}
