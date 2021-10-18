using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Core;
using OpenNefia.Core.Data;
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
    public class GameWrapper
    {
        public static GameWrapper Instance { get; private set; } = new GameWrapper();
        public GameState State { get; private set; }
        public GameScene Scene { get; private set; }

        public List<IUiLayer> Layers { get; private set; }

        public ModLoader ModLoader = new ModLoader();

        private Love.Canvas? TargetCanvas;

        public GameWrapper()
        {
            Scene = new GameScene(this);
            State = new GameState();
            Layers = new List<IUiLayer>();
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
                layer.SetDefaultSize();
            }
        }

        internal void OnQuit()
        {
            Gui.StopMusic();
            Console.WriteLine("Quitting game.");
            Environment.Exit(0);
        }

        internal bool IsQuerying(IUiLayer layer)
        {
            return Layers.Count() != 0 && Layers.Last() == layer;
        }

        internal void PushLayer(IUiLayer layer)
        {
            layer.SetDefaultSize();
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

        public void MainCode(string[] args)
        {
            TargetCanvas = Love.Graphics.NewCanvas(Love.Graphics.GetWidth(), Love.Graphics.GetHeight());

            Startup.Run();

            FieldLayer.Instance = new FieldLayer();
            FieldLayer.Instance.Query();
        }
    }
}
