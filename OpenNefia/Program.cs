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
                DefaultRandomSeed = 0
            };

            Boot.Init(bootConfig);
            Timer.Step();
            GameWrapper.Instance.SystemStep();

            var iconData = Love.Image.NewImageData("Assets/Icon/icon.png");
            Love.Window.SetIcon(iconData);

            GameWrapper.Instance.MainCode(args);
        }
    }
}