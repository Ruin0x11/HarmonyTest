﻿using HarmonyLib;
using OpenNefia.Core.UI.Layer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestMod1
{
    [HarmonyPatch(typeof(FieldLayer), nameof(FieldLayer.PrintMessage))]
    class TestPatchProgram
    {
        static bool Prefix(ref string __result, string dood)
        {
            Console.WriteLine($"I'm in yr callback {dood} doooooooooooooood");
            __result = "Asdfg!";
            return true;
        }
    }
}
