using OpenNefia.Core.Data.Types;
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
        public uint Index;
        public Keys Key = Keys.None;

        public PromptChoice(T result) : this()
        {
        }
    }

    public class Prompt<T> : BaseUiLayer<PromptChoice<T>> where T: struct
    {
        public class UiPromptList : UiList<PromptChoice<T>>
        {
            public UiPromptList(List<PromptChoice<T>> choices) : base(choices)
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

        public UiPromptList List { get; }
        private bool Finished;

        public Prompt(List<PromptChoice<T>> choices)
        {
            this.List = new UiPromptList(choices);

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
            this.BindKey(Keybind.Entries.Escape, (_) => { this.Finished = true; return null; });
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
