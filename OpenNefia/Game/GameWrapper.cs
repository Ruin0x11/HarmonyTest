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
        public static GameWrapper Instance { get; private set; }

        public GameScene Scene { get; private set; }

        public List<IUiLayer> Layers { get; private set; }

        public ModLoader ModLoader = new ModLoader();

        public GameWrapper()
        {
            Scene = new GameScene();
        }

        public void Draw()
        {
            if (Graphics.IsActive())
            {
                Vector4 backgroundColor = Graphics.GetBackgroundColor();
                Graphics.Clear(backgroundColor.r, backgroundColor.g, backgroundColor.b, backgroundColor.a);
                Graphics.Origin();
                DrawLayers();
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

        internal bool IsQuerying(IUiLayer layer)
        {
            return Layers.Count() != 0 && Layers.Last() == layer;
        }

        internal void PushLayer(IUiLayer layer)
        {
            Layers.Add(layer);
        }

        internal void PopLayer(IUiLayer layer)
        {
            Layers.Remove(layer);
        }

        public void MainCode()
        {
            var layer = new Core.UI.Layer.FieldLayer();
            layer.Query();
        }

        public void Update()
        {
            var dt = Timer.GetDelta();
            for (int i = 0; i < this.Layers.Count; i++)
            {
                this.Layers[i].Update(dt);
            }
        }

        private void DrawLayers()
        {
            for (int i = 0; i < this.Layers.Count; i++)
            {
                this.Layers[i].Draw();
            }
        }
    }
}
