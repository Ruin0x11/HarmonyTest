using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class TestLayer : BaseUiLayer<int>
    {
        private bool Finished = false;
        private UiWindowBacking WindowBacking;

        public TestLayer()
        {
            this.WindowBacking = new UiWindowBacking(UiWindowBacking.WindowBackingType.Normal);

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.BindKey(Keybind.Entries.Escape, (_) =>
            {
                this.Finished = true;
                return null;
            });
            this.BindKey(Keybind.Entries.Identify, (_) =>
            {
                var choices = new List<PromptChoice<int>>()
                {
                    new PromptChoice<int>(0),
                    new PromptChoice<int>(24),
                    new PromptChoice<int>(42)
                };
                var prompt = new Prompt<int>(choices);
                Console.WriteLine($"Prompt start");
                var result = prompt.Query();
                Console.WriteLine($"Prompt result: {result}");
            });
        }

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);

            this.WindowBacking.Relayout(x + width / 4, y + height / 4, width / 2, height / 2);
        }

        public override void Update(float dt)
        {
            X = X + 1;
            if (X > 200)
            {
                X = 100;
            }

            this.WindowBacking.Update(dt);
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
            Graphics.SetColor(1f, 1f, 1f);
            Graphics.Rectangle(DrawMode.Fill, 100, 100, 100, 100);
            Graphics.SetColor(1f, 0, 1f);
            Graphics.Rectangle(DrawMode.Fill, 50 + X, 50 + Y, 100, 100);

            Graphics.SetColor(1f, 1f, 1f);
            this.WindowBacking.Draw();
        }
    }
}
