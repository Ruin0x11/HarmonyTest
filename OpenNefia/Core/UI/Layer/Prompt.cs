﻿using OpenNefia.Core.Data.Types;
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

        public Prompt(List<PromptChoice<T>> choices, PromptOptions options)
        {
            this.List = new UiPromptList(choices);
            this.Window = new UiTopicWindow(UiTopicWindow.FrameStyle.Zero, UiTopicWindow.WindowStyle.Zero);
            this.Options = options;

            this.Width = this.Options.Width;

            foreach (var cell in this.List)
            {
                this.Width = Math.Max(this.Width, cell.Width + 26 + 33 + 44);
            }

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

            this.List.EventOnActivate += (o, e) => this.Finish(e.SelectedChoice.Data);
            
        }

        public override void Relayout(int x = 0, int y = 0, int width = 0, int height = 0, RelayoutMode mode = RelayoutMode.Layout)
        {
            width = this.Width;
            height = this.List.Count * this.List.ItemHeight + 42;

            base.Relayout(x, y, width, height);

            this.List.Relayout(x + 30, y + 24, width, height);

            if (mode == RelayoutMode.Free)
            {
                var promptX = (Love.Graphics.GetWidth() - 10) / 2 + 3;
                x = promptX - width / 2;
                var promptY = (Love.Graphics.GetHeight() - Constants.INF_VERH - 30) / 2 - 4;
                y = promptY - this.List.Height / 2;
            }

            this.Width = Math.Max(this.Width, this.List.Width);

            this.Window.Relayout(x + 8, y + 8, this.Width - 16, this.Height - 16);
            this.List.Relayout(x + 30, y + 24, this.Width, this.Height);
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
