using OpenNefia.Core.UI;
using OpenNefia.Core.UI.Layer;
using System.Collections.Generic;

namespace OpenNefia.Core
{
    public static class Input
    {
        private enum YesNo
        {
            Yes,
            No
        }

        public static bool YesOrNo(bool canCancel = true, bool invert = false)
        {
            var items = new List<PromptChoice<YesNo>>()
            {
                new PromptChoice<YesNo>(YesNo.Yes, text: "Yes", key: Keys.Y),
                new PromptChoice<YesNo>(YesNo.No, text: "No..", key: Keys.N),
            };
            if (invert)
            {
                items.Reverse();
            }

            var result = new Prompt<YesNo>(items, new PromptOptions() { IsCancellable = canCancel }).Query();

            if (result.HasValue)
            {
                return result.Value.ChoiceData == YesNo.Yes;
            }

            return false;
        }
    }
}
