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

        public string Text
        {
            get => this.UiText.Text;
            set => this.UiText.Text = value;
        }
        protected IUiText UiText;

        public int TextWidth { get => this.UiText.Width; }

        public int XOffset { get; set; }

        public UiListCell(T data, IUiText text)
        {
            this.Data = data;
            this.UiText = text;
        }

        public UiListCell(T data, string text) : this(data, new UiText(FontDefOf.ListText, text)) {}

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
            this.UiText.Update(dt);
        }

        public override void Dispose()
        {
            this.UiText.Dispose();
        }
    }
}
