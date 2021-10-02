using Love;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class TestLayer : BaseUiLayer<int>
    {
        private int X = 100;
        private int Y = 100;
        private bool Finished = false;

        public TestLayer()
        {
            this.Keys.BindKey(Keybind.Entries.Escape, (_) =>
            {
                this.Finished = true;
                return null;
            });
        }

        public override void Update(float dt)
        {
            X = X + 1;
            if (X > 200)
            {
                X = 100;
            }
        }

        public override UiResult<int>? GetResult()
        {
            if (this.Finished)
            {
                this.Finished = false;
                return UiResult<int>.Finished(42);
            }

            return null;
        }

        public override void Draw()
        {
            Graphics.SetColor(255, 255, 255);
            Graphics.Rectangle(DrawMode.Fill, 100, 100, 100, 100);
            Graphics.SetColor(255, 0, 255);
            Graphics.Rectangle(DrawMode.Fill, 50 + X, 50 + Y, 100, 100);
        }
    }
}
