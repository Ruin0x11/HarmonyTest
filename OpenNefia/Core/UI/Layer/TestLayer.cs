using Love;
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
            if (Keyboard.IsPressed(KeyConstant.Escape))
            {
                return UiResult<int>.Finished(42);
            }

            return null;
        }

        public override void Draw()
        {
            Graphics.SetColor(255, 255, 255);
            Graphics.Rectangle(DrawMode.Fill, 100, 100, 100, 100);
            Graphics.SetColor(255, 0, 255);
            Graphics.Rectangle(DrawMode.Fill, 150 + X, 150, 100, 100);
        }
    }
}
