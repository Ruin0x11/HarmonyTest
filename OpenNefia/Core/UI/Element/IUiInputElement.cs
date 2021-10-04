﻿namespace OpenNefia.Core.UI.Element
{
    public interface IUiInputElement : IUiElement, IUiInput
    {
        KeybindWrapper Keybinds { get; }
        TextInputWrapper TextInput { get; }
        KeyForwardsWrapper Forwards { get; set; }
    }
}
