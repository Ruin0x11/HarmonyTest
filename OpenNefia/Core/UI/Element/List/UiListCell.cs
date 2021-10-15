using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class UiListCell<T> : BaseDrawable, IUiListCell<T>
    {
        public T Data { get; set; }

        public FontDef FontListText;

        private string? _Text;
        public string Text
        {
            get => _Text!;
            set
            {
                this._Text = value;
                this.UiText = this.MakeUIText(this.Data, this._Text);
            }
        }
        protected IUiText UiText;

        public int TextWidth { get => this.UiText.Width; }

        public int XOffset { get; set; }

        public UiListCell(T data, string text)
        {
            this.Data = data;

            this.FontListText = FontDefOf.ListText;

            this.Text = text;
            this.UiText = this.MakeUIText(data, this.Text);
        }

        protected virtual IUiText MakeUIText(T data, string rawText)
        {
            return new UiText(this.FontListText, rawText);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            this.UiText.SetPosition(x + 4 + this.XOffset, y + 1);
        }

        public override void SetSize(int width = -1, int height = -1)
        {
            this.UiText.SetSize(width, height);
            base.SetSize(Math.Max(width, this.UiText.Width), height);
        }

        public override void Draw()
        {
            this.UiText.Draw();
        }

        public override void Update(float dt)
        {
        }
    }
}
