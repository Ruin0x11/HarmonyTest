using System;

namespace OpenNefia.Core.UI
{
    public interface ITextInputBinder
    {
        bool TextInputEnabled { get; set; }

        void BindTextInput(Action<TextInputEvent> evt);
        void UnbindTextInput();
    }
}
