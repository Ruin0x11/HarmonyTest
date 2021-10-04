﻿using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;

namespace OpenNefia.Core.UI.Element
{
    public class UiText : BaseUiElement, IUiText
    {
        private Love.Text BakedText;

        private FontAsset _Font;
        public FontAsset Font
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
        
        public UiText(FontAsset font, string text = "")
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
            GraphicsEx.SetColor(this.Font.Color);
            Love.Graphics.Draw(this.BakedText, this.X, this.Y);
        }
    }
}
