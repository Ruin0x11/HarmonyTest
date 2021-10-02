using HarmonyLib;
using Love;
using System;
using System.Collections.Generic;
using OpenNefia.Game;

namespace OpenNefia
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var bootConfig = new BootConfig();

            Boot.Init(bootConfig);
            Timer.Step();

            GameWrapper.Instance.MainCode();
        }
    }
}