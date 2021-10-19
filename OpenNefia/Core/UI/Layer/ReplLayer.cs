using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Layer.Repl;
using OpenNefia.Core.Util.Collections;

namespace OpenNefia.Core.UI.Layer
{
    public class ReplLayer : BaseUiLayer<UiNoResult>
    {
        protected class ReplTextLine
        {
            public string Text;
            public ColorDef Color;

            public ReplTextLine(string line, ColorDef color)
            {
                Text = line;
                Color = color;
            }
        }

        public float HeightPercentage { get; set; } = 0.3f;
        public bool UsePullDownAnimation { get; set; } = true;
        public int PullDownSpeed { get; set; } = 80;
        public bool HideDuringExecute { get; set; } = true;

        private FontDef FontReplText;
        private ColorDef ColorReplBackground;
        private ColorDef ColorReplText;
        private ColorDef ColorReplTextError;

        private IUiText TextCaret;
        private IUiText TextEditingLine;
        private IUiText TextScrollbackCounter;
        private IUiText[] TextScrollback;

        private IReplExecutor Executor;

        protected float Dt = 0f;
        protected bool IsPullingDown;
        protected int PullDownY = 0;
        protected int MaxLines = 0;
        protected int CursorX = 0;
        /// Stringwise width position of cursor. (not CJK-width)
        public int CursorCharPos { get; protected set; } = 0;
        protected bool NeedsScrollbackRedraw = true;
        protected CircularBuffer<ReplTextLine> ScrollbackBuffer;
        protected int ScrollbackPos = 0;
        private bool IsExecuting = false;

        public ReplLayer(int scrollbackSize = 15000)
        {
            this.FontReplText = FontDefOf.ReplText;
            this.ColorReplBackground = ColorDefOf.ReplBackground;
            this.ColorReplTextError = ColorDefOf.ReplTextError;
            this.ColorReplText = ColorDefOf.ReplText;

            this.TextCaret = new UiText(this.FontReplText, "> ");
            this.TextEditingLine = new UiText(this.FontReplText, "");
            this.TextScrollbackCounter = new UiText(this.FontReplText, "0/0");
            this.TextScrollback = new IUiText[0];
            this.ScrollbackBuffer = new CircularBuffer<ReplTextLine>(scrollbackSize);

            this.Executor = new CSharpReplExecutor();

            this.IsPullingDown = this.UsePullDownAnimation;

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.TextInput.Enabled = true;
            this.TextInput.Callback += (evt) => this.InsertText(evt);

            this.Keybinds[Keys.Left] += (_) => this.SetCursorPos(this.CursorCharPos - 1);
            this.Keybinds[Keys.Right] += (_) => this.SetCursorPos(this.CursorCharPos + 1);
            this.Keybinds[Keys.Backspace] += (_) => this.DeleteCharAtCursor();
            this.Keybinds[Keys.PageUp] += (_) => this.SetScrollbackPos(this.ScrollbackPos + (this.MaxLines / 2));
            this.Keybinds[Keys.PageDown] += (_) => this.SetScrollbackPos(this.ScrollbackPos - (this.MaxLines / 2));
            this.Keybinds[Keys.Enter] += (_) => this.SubmitText();
            this.Keybinds[Keys.Cancel] += (_) => this.Cancel();
            this.Keybinds[Keys.Escape] += (_) => this.Cancel();
        }

        /// <summary>
        /// Splits on CJK width position.
        /// </summary>
        private static (string, string) SplitStringAtPos(string str, int pos)
        {
            var left = str.Substring(0, pos);
            var right = str.Substring(pos);
            return (left, right);
        }

        public void InsertText(TextInputEvent evt)
        {
            var text = this.TextEditingLine.Text;

            if (this.CursorCharPos == text.Length)
            {
                text += evt.Text;
            }
            else if (this.CursorCharPos == 0)
            {
                text = evt.Text + text;
            }
            else
            {
                var (left, right) = SplitStringAtPos(text, this.CursorCharPos);
                text = left + evt.Text + right;
            }

            this.TextEditingLine.Text = text;

            this.SetCursorPos(this.CursorCharPos + evt.Text.Length);
        }

        public void DeleteCharAtCursor()
        {
            if (this.CursorCharPos == 0)
            {
                return;
            }

            var text = this.TextEditingLine.Text;
            text = text.Remove(this.CursorCharPos - 1, 1);
            this.TextEditingLine.Text = text;

            this.SetCursorPos(this.CursorCharPos - 1);
        }

        public void SetScrollbackPos(int pos)
        {
            this.ScrollbackPos = Math.Clamp(pos, 0, Math.Max(this.ScrollbackBuffer.Size - this.MaxLines, 0));
            this.NeedsScrollbackRedraw = true;
        }

        public void Clear()
        {
            this.ScrollbackBuffer.Clear();
            this.ScrollbackPos = 0;
            this.NeedsScrollbackRedraw = true;
        }

        public void SubmitText()
        {
            var code = this.TextEditingLine.Text;

            this.TextEditingLine.Text = string.Empty;
            this.ScrollbackPos = 0;
            this.CursorCharPos = 0;
            this.CursorX = 0;

            var result = this.Executor.Execute(code);

            switch(result)
            {
                case ReplExecutionResult.Success success:
                    this.PrintText(success.Result);
                    break;
                case ReplExecutionResult.Error error:
                    var text = $"Error: {error.Exception.StackTrace}\n{error.Exception.Message}";
                    this.PrintText(text, this.ColorReplTextError);
                    break;
                default:
                    break;
            }
        }

        private void PrintText(string text, ColorDef? color = null)
        {
            if (color == null)
                color = this.ColorReplText;

            var (remain, wrapped) = this.FontReplText.GetWrap(text, this.Width);

            foreach (var line in wrapped)
            {
                this.ScrollbackBuffer.PushFront(new ReplTextLine(line, color));
            }

            this.NeedsScrollbackRedraw = true;
        }

        private void SetCursorPos(int pos)
        {
            this.CursorCharPos = Math.Clamp(pos, 0, this.TextEditingLine.Text.Length);

            var prefixToCursor = this.TextEditingLine.Text.Substring(0, this.CursorCharPos);
            var prefixWidth = this.FontReplText.GetWidth(prefixToCursor);
            this.CursorX = prefixWidth;
            this.Dt = 0f;
        }

        public override void SetDefaultSize()
        {
            this.IsPullingDown = false;
            this.PullDownY = 0;

            var width = Love.Graphics.GetWidth();
            var viewportWidth = Love.Graphics.GetHeight();
            var height = (int)Math.Clamp(viewportWidth * this.HeightPercentage, viewportWidth / 3, viewportWidth - 1);
            this.SetSize(width, height);
            this.SetPosition(0, 0);

            if (this.UsePullDownAnimation)
            {
                this.PullDownY = this.MaxLines * this.FontReplText.GetHeight();
            }
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            base.SetSize(width, height);
            this.MaxLines = (this.Height - 5) / this.FontReplText.GetHeight();

            foreach (var text in this.TextScrollback)
                text.Dispose();
            this.TextScrollback = new IUiText[this.MaxLines];
            for (int i = 0; i < this.MaxLines; i++)
                this.TextScrollback[i] = new UiText(this.FontReplText);

            this.NeedsScrollbackRedraw = true;
        }

        public override void SetPosition(int x = 0, int y = 0)
        {
            base.SetPosition(x, y);
        }

        public override void OnQuery()
        {
        }

        public override void Update(float dt)
        {
            this.Dt += dt;
            
            this.TextCaret.Update(dt);
            this.TextEditingLine.Update(dt);
            this.TextScrollbackCounter.Update(dt);
            foreach (var text in this.TextScrollback)
            {
                text.Update(dt);
            }

            if (this.UsePullDownAnimation)
            {
                if (this.WasFinished || this.WasCancelled)
                {
                    this.PullDownY = Math.Min(this.PullDownY + this.PullDownSpeed, this.MaxLines * this.FontReplText.GetHeight());
                }
                else if (this.PullDownY > 0)
                {
                    this.PullDownY = Math.Max(this.PullDownY - this.PullDownSpeed, 0);
                }
            }
        }

        public override UiResult<UiNoResult>? GetResult()
        {
            if (!this.UsePullDownAnimation)
                return base.GetResult();

            if (this.WasFinished || this.WasCancelled)
            {
                if (this.PullDownY >= this.MaxLines * this.FontReplText.GetHeight())
                {
                    return base.GetResult();
                }
            }

            return null;
        }

        public override void Draw()
        {
            if (this.IsExecuting && this.HideDuringExecute)
            {
                return;
            }

            // Background
            GraphicsEx.SetColor(this.ColorReplBackground);
            GraphicsEx.FilledRect(this.X, this.Y, this.Width, this.Height);

            var yPos = this.Y + this.Height - this.FontReplText.GetHeight() - 5;

            // Caret
            GraphicsEx.SetFont(this.FontReplText);
            this.TextCaret.SetPosition(this.X + 5, yPos);
            this.TextCaret.Draw();

            // Current line
            this.TextEditingLine.SetPosition(this.X + 5 + this.FontReplText.GetWidth(this.TextCaret.Text), yPos);
            this.TextEditingLine.Draw();

            // Scrollback Display
            if (this.NeedsScrollbackRedraw)
            {
                if (this.ScrollbackPos > 0)
                {
                    this.TextScrollbackCounter.Text = $"{this.ScrollbackPos}/{this.ScrollbackBuffer.Size}";
                    this.TextScrollbackCounter.SetPosition(this.X + this.Width - this.TextScrollbackCounter.Width - 5, yPos);
                }

                for (int i = 0; i < this.MaxLines; i++)
                {
                    var index = this.ScrollbackPos + i;
                    if (index >= this.ScrollbackBuffer.Size)
                    {
                        break;
                    }

                    var uiText = this.TextScrollback[i];
                    var line = this.ScrollbackBuffer[index];
                    uiText.Text = line.Text;
                    uiText.Color = line.Color;
                    uiText.SetPosition(this.X + 5, this.Y + this.Height - this.FontReplText.GetHeight() * (i + 2) - 5);
                }
                this.NeedsScrollbackRedraw = false;
            }

            // Scrollback counter
            if (this.ScrollbackPos > 0)
            {
                this.TextScrollbackCounter.Draw();
            }

            foreach (var text in this.TextScrollback)
            {
                text.Draw();
            }

            if (Math.Floor(this.Dt * 2) % 2 == 0)
            {
                var top = this.Height - this.PullDownY * this.FontReplText.GetHeight();
                var x = this.X + 6 + this.TextCaret.Width + this.CursorX;
                var y = this.Y + top - this.FontReplText.GetHeight() - 5;
                GraphicsEx.SetColor(this.FontReplText.Color);
                Love.Graphics.Line(x, y, x, y + this.FontReplText.GetHeight());
            }
        }

        public override void Dispose()
        {
            this.TextEditingLine.Dispose();
            this.TextScrollbackCounter.Dispose();
            foreach (var text in this.TextScrollback)
            {
                text.Dispose();
            }
        }
    }
}
