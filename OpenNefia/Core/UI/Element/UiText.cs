using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using static OpenNefia.Core.Rendering.GraphicsEx;

namespace OpenNefia.Core.UI.Element
{
    public class UiText : BaseDrawable, IUiText
    {
        private Love.Text BakedText;

        private FontDef _Font;
        public FontDef Font
        {
            get => _Font;
            set
            {
                this._Font = value;
                this.RebakeText();
            }
        }

        private string _Text;
        public string Text
        {
            get => _Text;
            set
            {
                this._Text = value;
                this.RebakeText();
            }
        }

#pragma warning disable CS8618
        
        public UiText(FontDef font, string text = "")
        {
            this._Text = text;
            this._Font = font;
            this.RebakeText();
        }

#pragma warning restore CS8618

        protected void RebakeText()
        {
            this.BakedText = Love.Graphics.NewText(this.Font, this.Text);
            this.SetSize(0, 0);
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            base.SetSize(this.Font.GetWidth(this.Text), this.Font.GetHeight());
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            switch(this.Font.Style)
            {
                case FontStyle.Normal:
                    GraphicsEx.SetColor(this.Font.Color);
                    Love.Graphics.Draw(this.BakedText, this.X, this.Y);
                    break;

                case FontStyle.Outlined:
                    GraphicsEx.SetColor(this.Font.ExtraColors[FontDef.ColorKinds.Background]);
                    for (int dx = -1; dx <= 1; dx++)
                        for (int dy = -1; dy <= 1; dy++)
                            Love.Graphics.Draw(this.BakedText, this.X + dx, this.Y + dy);

                    GraphicsEx.SetColor(this.Font.Color);
                    Love.Graphics.Draw(this.BakedText, this.X, this.Y);
                    break;

                case FontStyle.Shadowed:
                    GraphicsEx.SetColor(this.Font.ExtraColors[FontDef.ColorKinds.Background]);
                    Love.Graphics.Draw(this.BakedText, this.X + -1, this.Y + -1);

                    GraphicsEx.SetColor(this.Font.Color);
                    Love.Graphics.Draw(this.BakedText, this.X, this.Y);
                    break;
            }
        }
    }
}
