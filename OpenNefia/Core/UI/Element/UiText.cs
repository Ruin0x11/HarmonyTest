using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;

namespace OpenNefia.Core.UI.Element
{
    public class UiText : BaseUiElement, IUiText
    {
        private Love.Text BakedText;
        private ColorAsset Color;

        private FontAsset _Font;
        public FontAsset Font
        {
            get => _Font;
            set
            {
                this._Font = value;
                this.BakedText = Love.Graphics.NewText(value, this.Text);
            }
        }

        private string _Text = string.Empty;
        public string Text
        {
            get => _Text;
            set
            {
                this._Text = value;
                this.BakedText = Love.Graphics.NewText(this.Font, value);
            }
        }

#pragma warning disable CS8618

        public UiText(string text, FontAsset font, ColorAsset? color = null)
        {
            if (color == null)
                color = ColorAsset.Entries.TextBackground;

            this._Text = text;
            this._Font = font;
            this.BakedText = Love.Graphics.NewText(this.Font, this.Text);
            this.Color = color;
        }

        public UiText(FontAsset font, ColorAsset? color = null) : this(string.Empty, font, color)
        {
        }

#pragma warning restore CS8618

        public override void Relayout(int x = 0, int y = 0, int width = 0, int height = 0)
        {
            base.Relayout(x, y, width, height);

            this.Width = this.Font.GetWidth(this.Text);
            this.Height = this.Font.GetHeight();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            GraphicsEx.SetColor(this.Color);
            Love.Graphics.Draw(this.BakedText, this.X, this.Y);
        }
    }
}