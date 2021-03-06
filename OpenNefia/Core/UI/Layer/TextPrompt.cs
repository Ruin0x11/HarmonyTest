using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
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
    public class TextPrompt : BaseUiLayer<string>
    {
        private string _Value;
        public string Value
        {
            get => _Value;
            set
            {
                _Value = value;
                this.UpdateText();
            }
        }

        public bool IsCancellable { get; set; }
        public bool LimitLength { get; set; }
        public int? MaxLength { get; set; }
        public bool HasShadow { get; set; }

        protected bool IsCutOff = false;
        protected float Dt = 0f;
        protected double CaretAlpha = 2;

        protected UiTopicWindow TopicWindow;
        protected IUiText Text;

        protected AssetDrawable AssetLabelInput;
        protected AssetDrawable AssetImeStatusJapanese;
        protected AssetDrawable AssetImeStatusEnglish;
        protected AssetDrawable AssetImeStatusNone;
        protected AssetDrawable AssetInputCaret;
        protected ColorDef ColorPromptBackground;
        protected FontDef FontPromptText;

        public TextPrompt(int? maxLength = 16, bool limitLength = false, string? initialValue = null, bool isCancellable = true, bool hasShadow = true)
        {
            if (initialValue == null)
                initialValue = string.Empty;

            this.MaxLength = maxLength;
            this.LimitLength = limitLength;
            this._Value = initialValue;
            this.IsCancellable = isCancellable;
            this.HasShadow = hasShadow;

            this.AssetLabelInput = new AssetDrawable(AssetDefOf.LabelInput);
            this.AssetImeStatusJapanese = new AssetDrawable(AssetDefOf.ImeStatusJapanese);
            this.AssetImeStatusEnglish = new AssetDrawable(AssetDefOf.ImeStatusEnglish);
            this.AssetImeStatusNone = new AssetDrawable(AssetDefOf.ImeStatusNone);
            this.AssetInputCaret = new AssetDrawable(AssetDefOf.InputCaret);
            this.ColorPromptBackground = ColorDefOf.PromptBackground;
            this.FontPromptText = FontDefOf.PromptText;

            this.TopicWindow = new UiTopicWindow(UiTopicWindow.FrameStyle.Zero, UiTopicWindow.WindowStyle.Two);
            this.Text = new UiText(this.FontPromptText);

            this.UpdateText();

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.TextInput.Enabled = true;
            this.TextInput.Callback += (evt) => {
                this.Value = this.Value + evt.Text;
                this.UpdateText();
            };
            this.Keybinds[Keybind.Entries.Enter] += (_) => this.Finish(this.Value);
            this.Keybinds[Keybind.Entries.Escape] += (_) => {
                if (this.IsCancellable)
                    this.Cancel();
            };
            this.Keybinds[Keys.Backspace] += (_) => {
                if (this.Value.Length > 0)
                {
                    this.Value = this.Value.Remove(this.Value.Length - 1, 1);
                    this.UpdateText();
                }
            };
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public override UiResult<string> Query() => base.Query();

        public override void OnQuery()
        {
            Sounds.PlayOneShot(SoundDefOf.Pop2);
        }

        protected virtual void UpdateText()
        {
            this.IsCutOff = false;
            string displayText = string.Empty;
            var wideLength = this.Value.GetWideLength();

            if (this.MaxLength.HasValue)
            {
                if (this.MaxLength.HasValue && wideLength > this.MaxLength.Value - 2)
                {
                    var dots = "...";
                    if (I18N.IsFullwidth())
                    {
                        dots = "…";
                    }
                    displayText = this.Value.WideSubstring(0, this.MaxLength.Value - 2) + dots;
                }
                else
                {
                    displayText = this.Value;
                }
            }
            else
            {
                displayText = this.Value;
            }

            this.Text.Text = displayText;

            if (this.MaxLength.HasValue && wideLength > this.MaxLength)
            {
                this._Value = this.Value.WideSubstring(0, this.MaxLength);
                this.IsCutOff = true;
            }
        }

        public const int DEFAULT_WIDTH = 16 * 16 + 60;
        public const int DEFAULT_HEIGHT = 36;
        
        public override void GetPreferredBounds(out int x, out int y, out int width, out int height)
        {
            var rect = UiUtils.GetCenteredParams(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            x = rect.X;
            y = rect.Y;
            width = rect.Width;
            height = rect.Height;
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            width = Math.Max(width, this.FontPromptText.GetWidth(this.Value));

            base.SetSize(width, height);

            this.TopicWindow.SetSize(this.Width, this.Height);
            this.Text.SetPreferredSize();
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);

            this.TopicWindow.SetPosition(this.X, this.Y);
            this.Text.SetPosition(this.X + 36, this.Y + 9);
        }

        public override void Update(float dt)
        {
            this.Dt += (dt / Constants.SCREEN_REFRESH) * 4;
            this.CaretAlpha = Math.Sin(this.Dt) * 255f * 1f;
            this.TopicWindow.Update(dt);
            this.Text.Update(dt);
        }

        public override void Draw()
        {
            if (this.HasShadow)
            {
                GraphicsEx.SetColor(this.ColorPromptBackground);
                GraphicsEx.FilledRect(this.X + 4, this.Y + 4, this.Width - 1, this.Height - 1);
            }

            this.TopicWindow.Draw();

            GraphicsEx.SetColor(Love.Color.White);
            this.AssetLabelInput.Draw(this.X + this.Width / 2 - 60, this.Y - 32);

            if (this.IsCutOff)
            {
                this.AssetImeStatusNone.Draw(this.X + 8, this.Y + 4);
            }
            else
            {
                this.AssetImeStatusEnglish.Draw(this.X + 8, this.Y + 4);
            }

            this.Text.Draw();

            GraphicsEx.SetColor(255, 255, 255, (int)this.CaretAlpha);
            this.AssetInputCaret.Draw(this.X + this.Text.Width + 34, this.Y + 5);
        }

        public override void Dispose()
        {
            this.AssetLabelInput.Dispose();
            this.AssetImeStatusEnglish.Dispose();
            this.AssetImeStatusJapanese.Dispose();
            this.AssetImeStatusNone.Dispose();
            this.AssetInputCaret.Dispose();
            this.Text.Dispose();
            this.TopicWindow.Dispose();
        }
    }
}
