using OpenNefia.Core.Data.Types;
using OpenNefia.Core.UI.Element;
using OpenNefia.Core.UI.Element.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public struct PromptChoice<T> where T : struct
    {
        public T Result;
        public string? Text = null;
        public uint? Index = null;
        public Keys Key = Keys.None;

        public PromptChoice(T result) : this()
        {
            this.Result = result;
        }
    }

    public class Prompt<T> : BaseUiLayer<PromptChoice<T>> where T: struct
    {
        public class UiPromptList : UiList<PromptChoice<T>>
        {
            public UiPromptList(List<PromptChoice<T>> choices) : base(choices, itemHeight: 20)
            {
            }

            public override string GetChoiceText(int index)
            {
                var choice = this[index];
                if (choice.Text != null)
                    return choice.Text;

                return choice.Result.ToString()!;
            }

            public override Keys GetChoiceKey(int index)
            {
                var choice = this[index];
                if (choice.Key != Keys.None)
                    return choice.Key;

                return Keys.A + index;
            }
        }

        public struct PromptOptions
        {
            public int Width = 160;
            public bool IsCancellable = true;
        }

        private PromptOptions Options;

        public UiPromptList List { get; }
        public UiTopicWindow Window { get; }

        private bool Finished;
        private bool Cancelled;

        public Prompt(List<PromptChoice<T>> choices, PromptOptions options)
        {
            this.List = new UiPromptList(choices);
            this.Window = new UiTopicWindow(UiTopicWindow.FrameStyle.Zero, UiTopicWindow.WindowStyle.Zero);
            this.Options = options;

            this.Width = this.Options.Width;

            var font = List.FontListText;
            for (int i = 0; i < this.List.Count; i++)
            {
                this.Width = Math.Max(this.Width, font.GetWidth(List.GetChoiceText(i)) + 26 + 33 + 44);
            }

            this.BindKeys();
        }

        public Prompt(List<PromptChoice<T>> choices) : this(choices, new PromptOptions()) { }

        protected virtual void BindKeys()
        {
            Func<KeyPressState, KeyActionResult?> cancel = (_) => {
                if (this.Options.IsCancellable)
                    this.Cancelled = true;
                return null;
            };

            this.BindKey(Keybind.Entries.Cancel, cancel);
            this.BindKey(Keybind.Entries.Escape, cancel);
        }

        public override void Relayout(int x = -1, int y = -1, int width = -1, int height = -1)
        {
            width = this.Width;
            height = this.List.Count * this.List.ItemHeight + 42;

            base.Relayout(x, y, width, height);

            this.Window.Relayout(x + 8, y + 8, width - 16, height - 16);
            this.List.Relayout(x + 30, y + 24, width, height);
        }

        public override void Update(float dt)
        {
            this.List.Update(dt);
        }

        public override UiResult<PromptChoice<T>>? GetResult()
        {
            if (this.Finished)
            {
                this.Finished = false;
                return UiResult<PromptChoice<T>>.Finished(this.List.SelectedChoice);
            }

            return null;
        }

        public override void Draw()
        {
            this.List.Draw();
        }
    }
}
