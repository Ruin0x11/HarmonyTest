namespace OpenNefia.Core.UI.Element
{
    public interface IUiInputElement : IUiElement, IUiInput
    {
        KeybindWrapper Keybinds { get; }
        KeyForwardsWrapper Forwards { get; }
    }
}