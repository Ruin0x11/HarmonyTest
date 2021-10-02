using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Love;
using OpenNefia.Core.UI;
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

        public GameScene Scene { get; private set; }

        public List<IUiLayer> Layers { get; private set; }

        public ModLoader ModLoader = new ModLoader();

        private Love.Canvas TargetCanvas;

        public GameWrapper()
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
                layer.Relayout(0, 0, width, height);
            }
        }

        internal void OnQuit()
        {
            Console.WriteLine("Quitting game.");
            Environment.Exit(0);
        }

        internal bool IsQuerying(IUiLayer layer)
        {
            return Layers.Count() != 0 && Layers.Last() == layer;
        }

        internal void PushLayer(IUiLayer layer)
        {
            layer.Relayout(0, 0, Love.Graphics.GetWidth(), Love.Graphics.GetHeight());
            Layers.Add(layer);
        }

        internal void PopLayer(IUiLayer layer)
        {
            Layers.Remove(layer);
        }

        public IUiLayer? CurrentLayer {
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
            this.ModLoader.Execute();

            var layer = new Core.UI.Layer.FieldLayer();
            layer.Query();
        }
    }
}
