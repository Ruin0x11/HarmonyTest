using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Core;
using OpenNefia.Core.Data;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Object;
using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Layer;
using OpenNefia.Mod;

namespace OpenNefia.Game
{
    /// <summary>
    /// This wraps Love.Scene with drawing methods unraveled from inside the standard game loop, so that they may be called deep into a nested call stack.
    /// This is necessary in order to directly port Elona's architecture without the use of stackful coroutines.
    /// </summary>
    public class Engine
    {
        public const string NameBase = "OpenNefia.NET";
        public static Version Version { get => Assembly.GetExecutingAssembly().GetName().Version!; }

        public static Engine Instance { get; private set; } = null!;
        public static ModLoader ModLoader = null!;

        private GameScene Scene;
        public List<IUiLayer> Layers { get; private set; }
        private Love.Canvas? TargetCanvas;

        public Engine()
        {
            Scene = new GameScene(this);
            Layers = new List<IUiLayer>();
            TargetCanvas = Love.Graphics.NewCanvas(Love.Graphics.GetWidth(), Love.Graphics.GetHeight());
        }

        public void Draw()
        {
            if (Graphics.IsActive())
            {
                Vector4 backgroundColor = Graphics.GetBackgroundColor();
                Graphics.Clear(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
                Graphics.Origin();
                DoDraw();
                Graphics.Present();
            }

            if (Timer.IsLimitMaxFPS())
            {
                // Timer.SleepByMaxFPS();
            }
            else
            {
                Timer.Sleep(0.001f);
            }
        }

        internal bool IsInActiveLayerList(IUiLayer layer) => this.Layers.Contains(layer);

        public void Update(float dt)
        {
            UpdateLayers(dt);
        }

        private void UpdateLayers(float dt)
        {
            for (int i = 0; i < this.Layers.Count; i++)
            {
                this.Layers[i].Update(dt);
            }
        }

        private void DoDraw()
        {
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha);
            Love.Graphics.SetCanvas(this.TargetCanvas);
            Love.Graphics.Clear();

            DrawLayers();

            Love.Graphics.SetCanvas();
            Love.Graphics.SetColor(Love.Color.White);
            Love.Graphics.SetBlendMode(Love.BlendMode.Alpha, Love.BlendAlphaMode.PreMultiplied);

            Love.Graphics.Draw(this.TargetCanvas);
        }

        private void DrawLayers()
        {
            for (int i = 0; i < this.Layers.Count; i++)
            {
                this.Layers[i].Draw();
            }
        }

        internal void SystemStep()
        {
            if (Boot.QuitFlag)
            {
                OnQuit();
            }

            Boot.SystemStep(this.Scene);
        }

        public void OnWindowResize(int width, int height)
        {
            this.TargetCanvas = Love.Graphics.NewCanvas(width, height);
            foreach (var layer in this.Layers)
            {
                layer.GetPreferredBounds(out var x, out var y, out var w, out var h);
                layer.SetSize(w, h);
                layer.SetPosition(x, y);
            }
        }

        internal void OnQuit()
        {
            Music.StopMusic();
            Console.WriteLine("Quitting game.");
            Environment.Exit(0);
        }

        internal bool IsQuerying(IUiLayer layer)
        {
            return Layers.Count() != 0 && Layers.Last() == layer;
        }

        internal void PushLayer(IUiLayer layer)
        {
            layer.GetPreferredBounds(out var x, out var y, out var width, out var height);
            layer.SetSize(width, height);
            layer.SetPosition(x, y);
            Layers.Add(layer);
        }

        internal void PopLayer(IUiLayer layer)
        {
            Layers.Remove(layer);
        }

        public IUiLayer? CurrentLayer 
        {
            get
            {
                if (Layers.Count == 0)
                {
                    return null;
                }
                return Layers.Last();
            }
        }

        internal static void InitStaticGlobals()
        {
            Instance = new Engine();
            ModLoader = new ModLoader();

            Current.InitStaticGlobals();
        }

        private static void RunTitleScreen()
        {
            var action = TitleScreenAction.ReturnToTitle;

            while (action != TitleScreenAction.Quit)
            {
                using (ITitleScreenLayer titleScreen = new TitleScreenLayer())
                {
                    var result = titleScreen.Query();
                    Console.WriteLine(result);

                    if (result.HasValue)
                    {
                        action = result.Value.Action;
                        switch (action)
                        {
                            case TitleScreenAction.ReturnToTitle:
                                break;
                            case TitleScreenAction.StartGame:
                                var map = InitMap();
                                Current.Game.ActiveMap = map;
                                using (var field = new FieldLayer(map))
                                {
                                    FieldLayer.Instance = field;
                                    var fieldResult = FieldLayer.Instance.Query();
                                    FieldLayer.Instance = null;
                                }
                                break;
                            case TitleScreenAction.Quit:
                                break;
                        }
                    }
                    else
                    {
                        action = TitleScreenAction.ReturnToTitle;
                    }
                }
            }
        }

        private static InstancedMap InitMap()
        {
            var map = MapDefOf.Vernis_TestSite.GenerateMap(new InstancedArea(AreaDefOf.Vernis), 5).Value;

            var player = CharaGen.Create(CharaDefOf.Chicken, map.AtPos(2, 2)).Value;
            Current.Player = player;
            map.ClearMemory(TileDefOf.WallForestFog);
            
            for (int i = 0; i< 10; i++)
            {
                var item = ItemGen.Create(ItemDefOf.PotionCureMinorWound, map.AtPos(3 + i, 3)).Value;
                item.Amount = 100;
            }

            return map;
        }

        public static void MainCode(string[] args)
        {
            InitStaticGlobals();

            Instance.SystemStep();

            Startup.Run();

            RunTitleScreen();

            Instance.OnQuit();
        }
    }
}
