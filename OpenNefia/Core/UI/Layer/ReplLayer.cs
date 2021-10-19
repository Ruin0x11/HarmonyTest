using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpRepl.Services.Completion;
using Microsoft.CodeAnalysis.Completion;
using OpenNefia.Core.Data.Types;
using OpenNefia.Core.Extensions;
using OpenNefia.Core.Rendering;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Layer.Repl;
using OpenNefia.Core.Util.Collections;
using TextCopy;

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

        private float _HeightPercentage = 0.3f;
        public float HeightPercentage {
            get
            {
                if (this.IsFullscreen)
                    return 1.0f;
                return _HeightPercentage;
            }
            set => _HeightPercentage = value;
        }

        private bool _IsFullscreen = false;
        public bool IsFullscreen {
            get => _IsFullscreen;
            set
            {
                _IsFullscreen = value;
                this.SetDefaultSize();
            }
        }
        public bool UsePullDownAnimation { get; set; } = true;
        public int PullDownSpeed { get; set; } = 80;
        public bool HideDuringExecute { get; set; } = true;
        public string EditingLine
        {
            get => this.TextEditingLine.Text;
            set {
                this.TextEditingLine.Text = value;
                this.UpdateCompletions();
            }
        }
        public bool ShowCompletions { get; set; } = true;

        public int ScrollbackSize { get => this.ScrollbackBuffer.Size; }
        public int CursorDisplayX { get => this.X + 6 + this.TextCaret.Width + this.CursorX; }
        public int CursorDisplayY
        {
            get
            {
                var top = this.Height - this.PullDownY * this.FontReplText.GetHeight();
                return this.Y + top - this.FontReplText.GetHeight() - 5;
            }
        }

        public FontDef FontReplText { get; }
        public ColorDef ColorReplBackground { get; }
        public ColorDef ColorReplText { get; }
        public ColorDef ColorReplTextResult { get; }
        public ColorDef ColorReplTextError { get; }

        private IUiText TextCaret;
        private IUiText TextEditingLine;
        private IUiText TextScrollbackCounter;
        private IUiText[] TextScrollback;

        private IReplExecutor Executor;
        private CompletionsPane CompletionsPane;

        protected float Dt = 0f;
        protected bool IsPullingDown = false;
        protected int PullDownY = 0;
        public int MaxLines { get; private set; } = 0;
        protected int CursorX = 0;
        
        private int _CursorCharPos = 0;
        /// Stringwise width position of cursor. (not CJK width)
        public int CursorCharPos
        {
            get => _CursorCharPos;
            set
            {
                this._CursorCharPos = Math.Clamp(value, 0, this.EditingLine.Length);
                var prefixToCursor = this.EditingLine.Substring(0, this.CursorCharPos);
                var prefixWidth = this.FontReplText.GetWidth(prefixToCursor);
                this.CursorX = prefixWidth;
                this.Dt = 0f;
                this.UpdateCompletions();
            }
        }

        protected bool NeedsScrollbackRedraw = true;
        protected CircularBuffer<ReplTextLine> ScrollbackBuffer;
        protected int ScrollbackPos = 0;
        protected List<string> History = new List<string>();
        protected IReadOnlyCollection<CompletionItemWithDescription>? Completions;
        protected int HistoryPos = -1;
        private bool IsExecuting = false;

        public ReplLayer(int scrollbackSize = 15000, IReplExecutor? executor = null)
        {
            this.FontReplText = FontDefOf.ReplText;
            this.ColorReplBackground = ColorDefOf.ReplBackground;
            this.ColorReplTextResult = ColorDefOf.ReplTextResult;
            this.ColorReplTextError = ColorDefOf.ReplTextError;
            this.ColorReplText = ColorDefOf.ReplText;

            this.TextCaret = new UiText(this.FontReplText, "> ");
            this.TextEditingLine = new UiText(this.FontReplText, "");
            this.TextScrollbackCounter = new UiText(this.FontReplText, "0/0");
            this.TextScrollback = new IUiText[0];
            this.ScrollbackBuffer = new CircularBuffer<ReplTextLine>(scrollbackSize);

            this.Executor = executor ?? new CSharpReplExecutor(this);
            this.Executor.Init();

            this.CompletionsPane = new CompletionsPane();

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.TextInput.Enabled = true;
            this.TextInput.Callback += (evt) => this.InsertText(evt.Text);

            this.Keybinds[Keys.Up] += (_) => this.PreviousHistoryEntry();
            this.Keybinds[Keys.Down] += (_) => this.NextHistoryEntry();
            this.Keybinds[Keys.Left] += (_) => this.CursorCharPos -= 1;
            this.Keybinds[Keys.Right] += (_) => this.CursorCharPos += 1;
            this.Keybinds[Keys.Backspace] += (_) => this.DeleteCharAtCursor();
            this.Keybinds[Keys.PageUp] += (_) => this.SetScrollbackPos(this.ScrollbackPos + (this.MaxLines / 2));
            this.Keybinds[Keys.PageDown] += (_) => this.SetScrollbackPos(this.ScrollbackPos - (this.MaxLines / 2));
            this.Keybinds[Keys.Ctrl | Keys.A] += (_) => this.CursorCharPos = 0;
            this.Keybinds[Keys.Ctrl | Keys.F] += (_) => this.IsFullscreen = !this.IsFullscreen;
            this.Keybinds[Keys.Ctrl | Keys.E] += (_) => this.CursorCharPos = this.EditingLine.Length;
            this.Keybinds[Keys.Ctrl | Keys.X] += (_) => this.CutText();
            this.Keybinds[Keys.Ctrl | Keys.C] += (_) => this.CopyText();
            this.Keybinds[Keys.Ctrl | Keys.V] += (_) => this.PasteText();
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

        public void InsertText(string inserted)
        {
            if (inserted == string.Empty)
                return;

            var text = this.EditingLine;

            if (this.CursorCharPos == text.Length)
            {
                text += inserted;
            }
            else if (this.CursorCharPos == 0)
            {
                text = inserted + text;
            }
            else
            {
                var (left, right) = SplitStringAtPos(text, this.CursorCharPos);
                text = left + inserted + right;
            }

            this.EditingLine = text;

            this.CursorCharPos += inserted.Length;
        }

        public void DeleteCharAtCursor()
        {
            if (this.CursorCharPos == 0)
            {
                return;
            }

            var text = this.EditingLine;
            text = text.Remove(this.CursorCharPos - 1, 1);
            this.EditingLine = text;

            this.CursorCharPos -= 1;
        }

        public void SetScrollbackPos(int pos)
        {
            this.ScrollbackPos = Math.Clamp(pos, 0, Math.Max(this.ScrollbackBuffer.Size - this.MaxLines, 0));
            this.NeedsScrollbackRedraw = true;
        }

        public void NextHistoryEntry()
        {
            var search = false;

            if (search)
            {

            }
            else
            {
                if (this.HistoryPos - 1 < 0)
                {
                    this.EditingLine = "";
                    this.HistoryPos = -1;
                }
                else if (this.HistoryPos - 1 <= this.History.Count)
                {
                    this.HistoryPos -= 1;
                    this.EditingLine = this.History[this.HistoryPos];
                }
            }
        }

        public void PreviousHistoryEntry()
        {
            var search = false;

            if (search)
            {

            }
            else
            {
                if (this.HistoryPos + 1 > this.History.Count - 1)
                {
                    this.EditingLine = "";
                    this.HistoryPos = this.History.Count;
                }
                else if (this.HistoryPos + 1 <= this.History.Count)
                {
                    this.HistoryPos += 1;
                    this.EditingLine = this.History[this.HistoryPos];
                }
            }
        }

        public void CutText()
        {
            ClipboardService.SetText(this.EditingLine);
            this.EditingLine = "";
        }

        public void CopyText()
        {
            ClipboardService.SetText(this.EditingLine);
        }

        public void PasteText()
        {
            var text = ClipboardService.GetText() ?? "";
            this.InsertText(text);
        }

        private void UpdateCompletions()
        {
            if (!this.ShowCompletions)
            {
                this.CompletionsPane.Clear();
                return;
            }

            this.CompletionsPane.SetPosition(this.CursorDisplayX, this.CursorDisplayY + this.FontReplText.GetHeight());

            if (this.EditingLine != string.Empty)
            {
                if (this.Completions == null)
                {
                    this.Completions = this.Executor.Complete(this.EditingLine, this.CursorCharPos);
                    this.CompletionsPane.SetFromCompletions(this.Completions);
                }
                this.CompletionsPane.FilterCompletions(this.EditingLine, this.CursorCharPos);
            }
            else
            {
                this.CompletionsPane.Clear();
            }
        }

        public void Clear()
        {
            this.ScrollbackBuffer.Clear();
            this.ScrollbackPos = 0;
            this.UpdateCompletions();
            this.NeedsScrollbackRedraw = true;
        }

        public void SubmitText()
        {
            var code = this.TextEditingLine.Text;

            this.TextEditingLine.Text = string.Empty;
            this.ScrollbackPos = 0;
            this.HistoryPos = -1;
            this.CursorCharPos = 0;
            this.CursorX = 0;
            this.UpdateCompletions();

            this.PrintText($"{this.TextCaret.Text}{code}");

            if (code != string.Empty)
            {
                this.History.Insert(0, code);
            }

            var result = this.Executor.Execute(code);

            switch(result)
            {
                case ReplExecutionResult.Success success:
                    this.PrintText(success.Result, this.ColorReplTextResult);
                    break;
                case ReplExecutionResult.Error error:
                    var text = $"Error: {error.Exception.StackTrace}\n{error.Exception.Message}";
                    this.PrintError(text);
                    break;
                default:
                    break;
            }
        }

        public void PrintError(string text) => PrintText(text, this.ColorReplTextError);

        public void PrintText(string text, ColorDef? color = null)
        {
            if (color == null)
                color = this.ColorReplText;

            var (_, wrapped) = this.FontReplText.GetWrap(text, this.Width);

            foreach (var line in wrapped)
            {
                this.ScrollbackBuffer.PushFront(new ReplTextLine(line, color));
            }

            this.NeedsScrollbackRedraw = true;
        }

        public override void SetDefaultSize()
        {
            var width = Love.Graphics.GetWidth();
            var viewportHeight = Love.Graphics.GetHeight();
            var height = (int)Math.Clamp(viewportHeight * this.HeightPercentage, 0, viewportHeight - 1);
            this.SetSize(width, height);
            this.SetPosition(0, 0);
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
            this.IsPullingDown = this.UsePullDownAnimation;
            this.PullDownY = 0;

            if (this.UsePullDownAnimation)
            {
                this.PullDownY = this.MaxLines * this.FontReplText.GetHeight();
            }
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

            this.CompletionsPane.Update(dt);

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
                var x = this.CursorDisplayX;
                var y = this.CursorDisplayY;
                GraphicsEx.SetColor(this.FontReplText.Color);
                Love.Graphics.Line(x, y, x, y + this.FontReplText.GetHeight());
            }

            this.CompletionsPane.Draw();
        }

        public override void Dispose()
        {
            this.TextCaret.Dispose();
            this.TextEditingLine.Dispose();
            this.TextScrollbackCounter.Dispose();
            foreach (var text in this.TextScrollback)
            {
                text.Dispose();
            }
        }
    }

    public class CompletionsPane : BaseDrawable
    {
        public int Padding { get; set; } = 5;
        public int BorderPadding { get; set; } = 4;
        public int MaxDisplayedEntries { get; set; } = 10;

        private record CompletionPaneEntry(IUiText Text, CompletionItemWithDescription Completion);

        private List<CompletionPaneEntry> Entries;

        private SlidingArrayWindow<CompletionPaneEntry> FilteredView;

        public FontDef FontCompletion { get; }
        public ColorDef ColorCompletionBorder { get; }
        public ColorDef ColorCompletionBackground { get; }

        public CompletionsPane()
        {
            Entries = new List<CompletionPaneEntry>();
            FilteredView = new SlidingArrayWindow<CompletionPaneEntry>();
            FontCompletion = FontDefOf.ReplCompletion;
            ColorCompletionBorder = ColorDefOf.ReplCompletionBorder;
            ColorCompletionBackground = ColorDefOf.ReplCompletionBackground;
        }

        public void SetFromCompletions(IReadOnlyCollection<CompletionItemWithDescription> completions)
        {
            this.Clear();

            foreach (var completion in completions.OrderBy(c => c.Item).Take(10))
            {
                Entries.Add(new CompletionPaneEntry(new UiText(this.FontCompletion, completion.Item.DisplayText), completion));
            }

            this.SetSize(0, 0);
            this.SetPosition(this.X, this.Y);
        }

        public void Increment()
        {
            this.FilteredView.IncrementSelectedIndex();
            this.SetSize(0, 0);
            this.SetPosition(this.X, this.Y);
        }

        public void Decrement()
        {
            this.FilteredView.DecrementSelectedIndex();
            this.SetSize(0, 0);
            this.SetPosition(this.X, this.Y);
        }

        public void FilterCompletions(string input, int caret)
        {
            bool Matches(CompletionItemWithDescription completion, string input) =>
                completion.Item.DisplayText.StartsWith(input, StringComparison.CurrentCultureIgnoreCase);

            string prefix;
            if (caret == 0)
                prefix = string.Empty;
            else if (caret < input.Length)
                prefix = input.Substring(0, caret);
            else
                prefix = input;

            var filtered = new List<CompletionPaneEntry>();
            var previouslySelectedItem = this.FilteredView.SelectedItem;
            var selectedIndex = -1;
            for (var i = 0; i < Entries.Count; i++)
            {
                var entry = Entries[i];
                if (!Matches(entry.Completion, prefix)) continue;

                filtered.Add(entry);
                if (entry.Completion.Item.DisplayText == previouslySelectedItem?.Completion.Item.DisplayText)
                {
                    selectedIndex = filtered.Count - 1;
                }
            }
            if (selectedIndex == -1 || previouslySelectedItem == null || !Matches(previouslySelectedItem!.Completion, input))
            {
                selectedIndex = 0;
            }
            FilteredView = new SlidingArrayWindow<CompletionPaneEntry>(
                filtered.ToArray(),
                MaxDisplayedEntries,
                selectedIndex
            );

            this.SetSize(0, 0);
            this.SetPosition(this.X, this.Y);
        }

        public override void SetSize(int width, int height)
        {
            width = 0;
            height = 0;
            foreach (var entry in this.FilteredView)
            {
                entry.Text.SetSize();
                width = Math.Max(entry.Text.Width + Padding * 2, width);
                height += entry.Text.Height;
            }

            width += Padding * 2;
            height += Padding * 2 + BorderPadding * 2;
            base.SetSize(width, height);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);
            foreach (var (entry, index) in this.FilteredView.WithIndex())
            {
                entry.Text.SetPosition(x + Padding + BorderPadding, y + Padding + BorderPadding + (index * this.FontCompletion.GetHeight()));
            }
        }

        public void Clear()
        {
            foreach (var item in this.Entries)
                item.Text.Dispose();
            this.Entries.Clear();
            this.FilteredView = new SlidingArrayWindow<CompletionPaneEntry>();
        }

        public override void Update(float dt)
        {
        }

        public override void Draw()
        {
            if (this.FilteredView.Count == 0)
                return;

            GraphicsEx.SetColor(this.ColorCompletionBackground);
            GraphicsEx.FilledRect(this.X, this.Y, this.Width, this.Height);

            GraphicsEx.SetColor(this.ColorCompletionBorder);
            GraphicsEx.LineRect(this.X + BorderPadding, this.Y + BorderPadding, this.Width - BorderPadding * 2, this.Height - BorderPadding * 2);

            foreach (var entry in this.FilteredView)
            {
                if (entry == this.FilteredView.SelectedItem)
                {
                    GraphicsEx.SetColor(255, 255, 255, 128);
                    GraphicsEx.FilledRect(entry.Text.X, entry.Text.Y, entry.Text.Width, entry.Text.Height);
                }
                entry.Text.Draw();
            }
        }
    }
}
