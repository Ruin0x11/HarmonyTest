using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element
{
    public class UiShadowedText : BaseUiElement, IUiText
    {
        private Love.Text BakedText;
        private ColorAsset FgColor;
        private ColorAsset BgColor;

        private Love.Font _Font;
        public Love.Font Font
        {
            get => _Font;
            set
            {
                this._Font = value;
                this.BakedText = Love.Graphics.NewText(value, this.Text);
            }
        }

        private string _Text;
        public string Text {
            get => _Text;
            set {
                this._Text = value;
                this.BakedText = Love.Graphics.NewText(this.Font, value);
            }
        }

        public UiShadowedText(string text, Love.Font font, ColorAsset? fgColor = null, ColorAsset? bgColor = null)
        {
            if (fgColor == null)
                fgColor = ColorAsset.Entries.TextForeground;
            if (bgColor == null)
                bgColor = ColorAsset.Entries.TextBackground;

            this._Text = text;
            this._Font = font;
            this.BakedText = Love.Graphics.NewText(this.Font, this.Text);
            this.FgColor = fgColor;
            this.BgColor = bgColor;
        }

        public override void Relayout(int x = 0, int y = 0, int width = 0, int height = 0, RelayoutMode mode = RelayoutMode.Layout)
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
            GraphicsEx.SetColor(this.BgColor);
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    Love.Graphics.Draw(this.BakedText, this.X + dx, this.Y + dy);

            GraphicsEx.SetColor(this.FgColor);
            Love.Graphics.Draw(this.BakedText, this.X, this.Y);
        }
    }
}
