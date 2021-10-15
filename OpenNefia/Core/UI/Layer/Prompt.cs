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
    public class PromptChoice<T> where T : struct
    {
        public T Result;
        public string? Text = null;
        public uint? Index = null;
        public Keys Key = Keys.None;

        public PromptChoice(T result)
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

            public override string GetChoiceText(PromptChoice<T> choice, int index)
            {
                if (choice.Text != null)
                    return choice.Text;

                return $"{choice.Result}";
            }

            public override UiListChoiceKey GetChoiceKey(PromptChoice<T> choice, int index)
            {
                if (choice.Key != Keys.None)
                    return new UiListChoiceKey(choice.Key, useKeybind: false);

                return new UiListChoiceKey(Keys.A + index, useKeybind: true);
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

        private int DefaultWidth;

        public Prompt(List<PromptChoice<T>> choices, PromptOptions options)
        {
            this.List = new UiPromptList(choices);
            this.Window = new UiTopicWindow(UiTopicWindow.FrameStyle.Zero, UiTopicWindow.WindowStyle.Zero);
            this.Options = options;

            this.DefaultWidth = this.Options.Width;

            this.BindKeys();
        }

        public Prompt(List<PromptChoice<T>> choices) : this(choices, new PromptOptions()) { }

        protected virtual void BindKeys()
        {
            Action<KeyInputEvent> cancel = (_) => {
                if (this.Options.IsCancellable)
                    this.Cancel();
            };

            this.Keybinds[Keybind.Entries.Cancel] += cancel;
            this.Keybinds[Keybind.Entries.Escape] += cancel;

            this.Forwards += this.List;

            this.List.EventOnActivate += (o, e) =>
            {
                Gui.PlaySound(SoundDefOf.Ok1);
                this.Finish(e.SelectedChoice.Data);
            };
        }

        public override void OnQuery()
        {
            Gui.PlaySound(SoundDefOf.Pop2);
        }

        public override void SetDefaultSize()
        {
            this.SetSize(this.DefaultWidth, 0);

            var promptX = (Love.Graphics.GetWidth() - 10) / 2 + 3;
            var promptY = (Love.Graphics.GetHeight() - Constants.INF_VERH - 30) / 2 - 4;

            var x = promptX - this.Width / 2;
            var y = promptY - this.List.Height / 2;

            this.SetPosition(x, y);
        }

        public override void SetSize(int width = 0, int height = 0)
        {
            width = Math.Max(this.DefaultWidth, width);

            this.List.SetSize(width, height);

            foreach (var cell in this.List)
            {
                width = Math.Max(width, cell.Width + 26 + 33 + 44);
            }

            base.SetSize(Math.Max(width, this.List.Width), this.List.Count * this.List.ItemHeight + 42);

            this.Window.SetSize(this.Width - 16, this.Height - 16);
        }

        public override void SetPosition(int x, int y)
        {
            base.SetPosition(x, y);

            this.List.SetPosition(this.X + 30, this.Y + 24);
            this.Window.SetPosition(this.X + 8, this.Y + 8);
        }

        public override void Update(float dt)
        {
            this.Window.Update(dt);
            this.List.Update(dt);
        }

        public override void Draw()
        {
            this.Window.Draw();
            this.List.Draw();
        }
    }
}
