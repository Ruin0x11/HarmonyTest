using HarmonyLib;
using Love;
using System;
using System.Collections.Generic;
using OpenNefia.Game;
using Mono.Cecil;
using System.Reflection;

namespace OpenNefia
{
    public class Program
    {

        public static void Main(string[] args)
        {
            var bootConfig = new BootConfig()
            {
                WindowTitle = "OpenNefia.NET",
                WindowDisplay = 0,
                WindowMinWidth = 800,
                WindowMinHeight = 600,
                WindowVsync = true,
                WindowResizable = true,
            };

            Boot.Init(bootConfig);
            Timer.Step();
            GameWrapper.Instance.SystemStep();

            foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            {
                Console.WriteLine(type.FullName);
                Console.WriteLine(type.BaseType?.FullName);
            }
            return;

            GameWrapper.Instance.MainCode(args);
        }
    }
}