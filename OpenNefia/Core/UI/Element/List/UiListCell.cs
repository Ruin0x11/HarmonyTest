using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element.List
{
    public class UiListCell<T> : BaseUiElement, IUiListCell<T>
    {
        public T Data { get; set; }

        public FontAsset FontListText;

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

        public int XOffset { get; set; }

        public UiListCell(T data, string text)
        {
            this.Data = data;

            this.FontListText = FontAsset.Entries.ListText;

            this.Text = text;
            this.UiText = this.MakeUIText(data, this.Text);
        }

        protected virtual IUiText MakeUIText(T data, string rawText)
        {
            return new UiText(rawText, this.FontListText);
        }

        public override void Relayout(int x = -1, int y = -1, int width = -1, int height = -1)
        {
            this.UiText.Relayout(x + 4 + this.XOffset, y + 1, width, height);
            base.Relayout(x, y, this.UiText.Width, height);
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
