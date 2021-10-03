using Love;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Element
{
    public class UiWindow : BaseUiElement
    {
        public string? Title { get; }
        public bool HasShadow { get; }
        public List<UiKeyHint> KeyHints { get; }
        public int XOffset { get; }
        public int YOffset { get; }

        protected AssetDrawable AssetTipIcons;
        protected ColorAsset ColorWindowBottomLine1;
        protected ColorAsset ColorWindowBottomLine2;
        protected FontAsset FontWindowTitle;
        protected FontAsset FontWindowKeyHints;

        protected UiShadowedText? TitleText;
        protected UiText KeyHintText;
        protected UiWindowBacking Window;
        protected UiWindowBacking WindowShadow;
        protected UiTopicWindow TopicWindow;

        public UiWindow(string? title = null, bool hasShadow = true, List<UiKeyHint>? keyHints = null, int xOffset = 0, int yOffset = 0)
        {
            if (keyHints == null)
                keyHints = new List<UiKeyHint>();

            this.Title = title;
            this.HasShadow = hasShadow;
            this.KeyHints = keyHints;
            this.XOffset = xOffset;
            this.YOffset = yOffset;

            this.AssetTipIcons = new AssetDrawable(Asset.Entries.TipIcons);
            this.ColorWindowBottomLine1 = ColorAsset.Entries.WindowBottomLine1;
            this.ColorWindowBottomLine2 = ColorAsset.Entries.WindowBottomLine2;
            this.FontWindowTitle = FontAsset.Entries.WindowTitle;
            this.FontWindowKeyHints = FontAsset.Entries.WindowKeyHints;

            if (this.Title != null)
                this.TitleText = new UiShadowedText(this.Title!, this.FontWindowTitle);
            this.KeyHintText = new UiText(this.FontWindowKeyHints);
            this.Window = new UiWindowBacking();
            this.WindowShadow = new UiWindowBacking(UiWindowBacking.WindowBackingType.Shadow);
            this.TopicWindow = new UiTopicWindow();

            this.KeyHintText.Text = "hogepiyo";
        }

        public override void Relayout(int x, int y, int width, int height)
        {
            base.Relayout(x, y, width, height);

            if (this.HasShadow)
                this.WindowShadow.Relayout(x + 4, y + 4, width, height);

            this.Window.Relayout(x, y, width, height);

            if (this.TitleText != null)
            {
                this.TopicWindow.Relayout(x + 34, y - 4, 45 * width / 100 + Math.Clamp(this.TitleText.Width - 120, 0, 200), 32);
                this.TitleText.Relayout(x + 45 * width / 200 + 34 - this.TitleText.Width / 2, this.Y + 4);
            }

            this.KeyHintText.Relayout(x + 58 + this.XOffset, y + height - 43 - height % 8);
        }

        public override void Update(float dt)
        {
            this.Window.Update(dt);
            this.WindowShadow.Update(dt);
            this.TopicWindow.Update(dt);
            this.TitleText?.Update(dt);
            this.KeyHintText.Update(dt);
        }

        public override void Draw()
        {
            if (this.HasShadow)
            {
                GraphicsEx.SetColor(255, 255, 255, 80);
                Love.Graphics.SetBlendMode(BlendMode.Subtract);
                this.WindowShadow.Draw();
                Love.Graphics.SetBlendMode(BlendMode.Alpha);
            }
            
            GraphicsEx.SetColor(Color.White);
            this.Window.Draw();

            this.AssetTipIcons.DrawRegion("1", this.X + 30 + this.XOffset, this.Y + this.Height - 47 - this.Height % 8);

            if (this.TitleText != null)
            {
                this.TopicWindow.Draw();
                this.TitleText.Draw();
            }

            GraphicsEx.SetColor(this.ColorWindowBottomLine1);
            Love.Graphics.Line(
                this.X + 50 + this.XOffset,
                this.Y + this.Height - 48 - this.Height % 8,
                this.X + this.Width - 40, 
                this.Y + this.Height - 48 - this.Height % 8);

            GraphicsEx.SetColor(this.ColorWindowBottomLine2);
            Love.Graphics.Line(
                this.X + 50 + this.XOffset,
                this.Y + this.Height - 49 - this.Height % 8,
                this.X + this.Width - 40,
                this.Y + this.Height - 49 - this.Height % 8);

            this.KeyHintText.Draw();
        }
    }
}
