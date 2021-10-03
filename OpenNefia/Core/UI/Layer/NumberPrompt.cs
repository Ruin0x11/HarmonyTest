using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public class NumberPrompt : BaseUiLayer<int>
    {
        private int _MinValue;
        public int MinValue
        {
            get => _MinValue;
            set
            {
                _MinValue = value;
                this.UpdateText();
            }
        }

        private int _MaxValue;
        public int MaxValue { 
            get => _MaxValue; 
            set
            {
                _MaxValue = value;
                this.UpdateText();
            }
        }

        private int _Value;

        public int Value
        {
            get => _Value;
            set
            {
                _Value = value;
                this.UpdateText();
            }
        }

        public bool IsCancellable { get; set; }

        protected UiTopicWindow TopicWindow;
        protected IUiText Text;

        protected AssetDrawable AssetLabelInput;
        protected AssetDrawable AssetArrowLeft;
        protected AssetDrawable AssetArrowRight;
        protected ColorAsset ColorPromptBackground;
        protected FontAsset FontPromptText;

        public NumberPrompt(int max = 1, int min = 1, int? initial = null, bool isCancellable = true)
        {
            this._MinValue = min;
            this._MaxValue = max;
            if (initial == null)
                initial = this.MaxValue;

            initial = Math.Clamp(initial.Value, this.MinValue, this.MaxValue);

            this._Value = initial.Value;
            this.IsCancellable = isCancellable;

            this.AssetLabelInput = new AssetDrawable(Asset.Entries.LabelInput);
            this.AssetArrowLeft = new AssetDrawable(Asset.Entries.ArrowLeft);
            this.AssetArrowRight = new AssetDrawable(Asset.Entries.ArrowRight);
            this.ColorPromptBackground = ColorAsset.Entries.PromptBackground;
            this.FontPromptText = FontAsset.Entries.PromptText;

            this.TopicWindow = new UiTopicWindow(UiTopicWindow.FrameStyle.Zero, UiTopicWindow.WindowStyle.Two);
            this.Text = new UiText(this.FontPromptText);
            
            this.UpdateText();

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.Keybinds[Keybind.Entries.UIUp] += (_) => {
                this.Value = this.MaxValue;
                Gui.PlaySound("Core.Cursor1");
            };
            this.Keybinds[Keybind.Entries.UIDown] += (_) => {
                this.Value = this.MinValue;
                Gui.PlaySound("Core.Cursor1");
            };
            this.Keybinds[Keybind.Entries.UILeft] += (_) => {
                this.Value = Math.Max(this.Value - 1, this.MinValue);
                Gui.PlaySound("Core.Cursor1");
            };
            this.Keybinds[Keybind.Entries.UIRight] += (_) => {
                this.Value = Math.Min(this.Value + 1, this.MaxValue);
                Gui.PlaySound("Core.Cursor1");
            };
            this.Keybinds[Keybind.Entries.Cancel] += (_) => { if (this.IsCancellable) this.Cancel(); };
            this.Keybinds[Keybind.Entries.Escape] += (_) => { if (this.IsCancellable) this.Cancel(); };
            this.Keybinds[Keybind.Entries.Enter] += (_) => this.Finish(this.Value);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override UiResult<int> Query() => base.Query();

        public override void OnQuery()
        {
            Gui.PlaySound("Core.Pop2");
        }

        protected virtual void UpdateText()
        {
            this.Text.Text = $"{this.Value}({this.Value})";
            this.Text.Relayout(this.X + this.Width - 70 - Text.Width + 8, this.Y + 11);
        }

        public override void Relayout(int x = 0, int y = 0, int width = 0, int height = 0, RelayoutMode mode = RelayoutMode.Layout)
        {
            if (mode == RelayoutMode.Free)
            {
                width = 8 * 16 + 60;
                height = 36;
                var rect = UiUtils.GetCenteredParams(width, height);
                base.Relayout(rect.X, rect.Y, rect.Width, rect.Height);
            }
            else
            {
                base.Relayout(x, y, width, height);
            }

            this.TopicWindow.Relayout(this.X + 20, this.Y, this.Width - 40, this.Height);
            this.UpdateText();
        }

        public override void Update(float dt)
        {
            this.TopicWindow.Update(dt);
            this.Text.Update(dt);
        }

        public override void Draw()
        {
            GraphicsEx.SetColor(this.ColorPromptBackground);
            GraphicsEx.DrawFilledRect(this.X + 24, this.Y + 4, this.Width - 42, this.Height - 1);
            
            this.TopicWindow.Draw();

            GraphicsEx.SetColor(Love.Color.White);
            this.AssetLabelInput.Draw(this.X + this.Width / 2 - 56, this.Y - 32);
            this.AssetArrowLeft.Draw(this.X + 28, this.Y + 4);
            this.AssetArrowRight.Draw(this.X + this.Width - 51, this.Y + 4);

            this.Text.Draw();
        }
    }
}
