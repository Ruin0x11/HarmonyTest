using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI.Layer
{
    public struct PromptChoice<T> where T : struct
    {
        public uint Index;
        public T Result;
    }

    public class Prompt<T> : BaseUiLayer<PromptChoice<T>> where T: struct
    {
        public List<PromptChoice<T>> Choices { get; }

        public Prompt(List<PromptChoice<T>> choices)
        {
            this.Choices = choices;

            this.BindKeys();
        }

        protected virtual void BindKeys()
        {
        }

        public override void Update(float dt)
        {
        }

        public override UiResult<PromptChoice<T>>? GetResult()
        {
        }

        public override void Draw()
        {
        }
    }
}
