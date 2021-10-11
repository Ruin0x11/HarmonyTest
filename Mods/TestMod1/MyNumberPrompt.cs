using OpenNefia.Core;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Layer;
using System;

namespace TestMod1
{
    internal class MyNumberPrompt : BaseUiLayer<NumberPromptResult>
    {
        private int _Value;
        public int Value
        {
            get => _Value;
            set
            {
                _Value = value;
                this.UpdateText();
            }
        }

        protected IUiText Text;
        protected ColorAsset ColorBackground;

        public MyNumberPrompt()
        {
            this._Value = 50;
            this.Text = new UiShadowedText(new FontAsset("TestMod1.MyNumberPromptFont", 60, fgColor: ColorAsset.Entries.TextWhite, bgColor: ColorAsset.Entries.TextBlack));
            this.ColorBackground = new ColorAsset("TestMod1.MyNumberPromptColor", 255, 255, 0);

            this.UpdateText();
            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.UILeft] += (_) => {
                this.Value--;
                // Gui.PlaySound("Core.Cursor1");
            };
            this.Keybinds[Keybind.Entries.UIRight] += (_) => {
                this.Value++;
                // Gui.PlaySound("Core.Cursor1");
            };
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Enter] += (_) => this.Finish(new NumberPromptResult(this.Value));
        }

        public override void OnQuery()
        {
            // Gui.PlaySound("Core.Pop2");
        }

        protected virtual void UpdateText()
        {
            this.Text.Text = $"{this.Value}({this.Value})";
        }

        public override void SetDefaultSize()
        {
            var size = UiUtils.GetCenteredParams(400, 400);
            this.SetSize(size.Width, size.Height);
            this.SetPosition(size.X, size.Y);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            this.Text.SetPosition(x, y);
        }

        public override void Update(float dt)
        {
            this.Text.Update(dt);
        }

        public override void Draw()
        {
            GraphicsEx.SetColor(this.ColorBackground);
            GraphicsEx.FilledRect(this.X, this.Y, this.Width, this.Height);

            this.Text.Draw();
        }
    }
}
