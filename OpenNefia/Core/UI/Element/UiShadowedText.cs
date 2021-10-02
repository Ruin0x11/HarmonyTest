using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OpenNefia.Core.Rendering.Drawing;

namespace OpenNefia.Core.UI.Element
{
    public class UiShadowedText : BaseUiElement
    {
        private Love.Text BakedText;
        private Love.Color FgColor;
        private Love.Color BgColor;

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

        private string _Text = string.Empty;
        public string Text {
            get => _Text;
            set {
                this._Text = value;
                this.BakedText = Love.Graphics.NewText(this.Font, value);
            }
        }

#pragma warning disable CS8618

        public UiShadowedText(string text, Love.Font font, Love.Color? fgColor = null, Love.Color? bgColor = null)
        {
            if (fgColor == null)
                fgColor = ColorAsset.Entries.TextForeground;
            if (bgColor == null)
                bgColor = ColorAsset.Entries.TextBackground;

            this.Text = text;
            this.Font = font;
            this.BakedText = Love.Graphics.NewText(this.Font, this.Text);
            this.FgColor = fgColor.Value;
            this.BgColor = bgColor.Value;
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
            Drawing.SetColor(this.BgColor);
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    Love.Graphics.Draw(this.BakedText, this.X + dx, this.Y + dy);

            Drawing.SetColor(this.FgColor);
            Love.Graphics.Draw(this.BakedText, this.X, this.Y);
        }
    }
}
