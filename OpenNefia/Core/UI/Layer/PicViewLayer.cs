using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    internal partial class PicViewLayer : BaseUiLayer<UiNoResult>
    {
        public Image Image { get; private set; }
        public bool DrawBorder { get; set; }

        private UiScroller Scroller;

        public PicViewLayer(Love.Image image)
        {
            Image = image;
            DrawBorder = true;

            Scroller = new UiScroller();
            
            Scroller.BindKeys(this);
            this.Keybinds[Keybind.Entries.Escape] += (_) => this.Cancel();
            this.Keybinds[Keybind.Entries.Cancel] += (_) => this.Cancel();
        }

        public override void SetDefaultSize()
        {
            var rect = UiUtils.GetCenteredParams(this.Image.GetWidth(), this.Image.GetHeight());
            this.SetSize(rect.Width, rect.Height);
            this.SetPosition(rect.X, rect.Y);
        }

        public override void Update(float dt)
        {
            Scroller.UpdateParentPosition(this, dt);
        }

        public override void Draw()
        {
            Love.Graphics.SetColor(Love.Color.Black);
            GraphicsEx.FilledRect(this.X, this.Y, this.Width, this.Height);

            Love.Graphics.SetColor(Love.Color.White);
            Love.Graphics.Draw(this.Image, this.X, this.Y);

            if (this.DrawBorder)
            {
                GraphicsEx.LineRect(this.X, this.Y, this.Width, this.Height);
            }
        }
    }
}
