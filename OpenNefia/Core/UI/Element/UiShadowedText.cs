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

        private string _Text;
        public string Text {
            get => _Text;
            set {
                this._Text = value;
                this.BakedText = Love.Graphics.NewText(this.Font, value);
            }
        }

        public UiShadowedText(FontAsset font, string text = "")
        {
            this._Text = text;
            this._Font = font;
            this.BakedText = Love.Graphics.NewText(this.Font, this.Text);
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
            GraphicsEx.SetColor(this.Font.ExtraColors[FontAsset.ColorKinds.Background]);
            for (int dx = -1; dx <= 1; dx++)
                for (int dy = -1; dy <= 1; dy++)
                    Love.Graphics.Draw(this.BakedText, this.X + dx, this.Y + dy);

            GraphicsEx.SetColor(this.Font.Color);
            Love.Graphics.Draw(this.BakedText, this.X, this.Y);
        }
    }
}
