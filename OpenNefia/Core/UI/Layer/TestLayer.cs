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
    public class TestLayer : BaseUiLayer<string>
    {
        private UiWindowBacking WindowBacking;

        public TestLayer()
        {
            this.WindowBacking = new UiWindowBacking(UiWindowBacking.WindowBackingType.Normal);

            this.BindKeys();
        }

        private int SquareX = 0;

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();

            this.Keybinds[Keybind.Entries.Identify] += (_) =>
            {
                var choices = new List<PromptChoice<int>>()
                {
                    new PromptChoice<int>(0),
                    new PromptChoice<int>(24),
                    new PromptChoice<int>(42142132)
                };
                var prompt = new Prompt<int>(choices);
                Console.WriteLine($"Prompt start");
                var result = prompt.Query();
                Console.WriteLine($"Prompt result: {result}");
            };

            this.Keybinds[Keybind.Entries.Mode] += (_) =>
            {
                var numberPrompt = new NumberPrompt(minValue: 2, maxValue: 100, initialValue: 50);
                var result = numberPrompt.Query();
                Console.WriteLine($"Number prompt result: {result}");
            };

            this.Keybinds[Keys.Ctrl | Keys.C] += (_) =>
            {
                new ListTestLayer().Query();
            };

            this.Keybinds[Keys.Ctrl | Keys.D] += (_) =>
            {
                var text = new TextPrompt().Query();
                Console.WriteLine($"Get: {text}");
            };
        }

        public override void SetDefaultSize()
        {
            var rect = UiUtils.GetCenteredParams(400, 300);
            this.SetSizeAndPosition(rect);
        }

        public override void SetSize(int width, int height)
        {
            base.SetSize(width, height);

            this.WindowBacking.SetSize(width, height);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);

            this.WindowBacking.SetPosition(this.X, this.Y);
        }

        public override void Update(float dt)
        {
            if (this.SquareX > 200)
            {
                this.SquareX = 0;
            }
            else
            {
                this.SquareX++;
            }

            this.WindowBacking.Update(dt);
        }

        public override void Draw()
        {
            Graphics.SetColor(1f, 1f, 1f);
            Graphics.Rectangle(DrawMode.Fill, 100, 100, 100, 100);
            Graphics.SetColor(1f, 0, 1f);
            Graphics.Rectangle(DrawMode.Fill, 50 + this.SquareX, 50, 100, 100);

            Graphics.SetColor(1f, 1f, 1f);
            this.WindowBacking.Draw();
        }

        public override void Dispose()
        {
            this.WindowBacking.Dispose();
        }
    }
}
