using HarmonyLib;
using Love;
using System;
using System.Collections.Generic;
using OpenNefia.Game;
using Mono.Cecil;
using System.Reflection;
using System.Threading.Tasks;

namespace OpenNefia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Cli.CliEntryPoint.Run(args).GetAwaiter().GetResult();
                return;
            }

            var bootConfig = new BootConfig()
            {
                WindowTitle = "OpenNefia.NET",
                WindowDisplay = 0,
                WindowMinWidth = 800,
                WindowMinHeight = 600,
                WindowVsync = true,
                WindowResizable = true,
                DefaultRandomSeed = 0
            };

            Boot.Init(bootConfig);
            Timer.Step();

            var iconData = Love.Image.NewImageData("Assets/Icon/icon.png");
            Love.Window.SetIcon(iconData);

            Engine.MainCode(args);
        }
    }
}