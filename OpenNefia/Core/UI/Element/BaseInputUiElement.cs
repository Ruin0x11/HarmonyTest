using Love;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseInputUiElement : BaseUiElement, IUiInput
    {
        protected IKeyHandler Keys = new KeyHandler();

        public void OnKeyPressed(KeyConstant key, bool isRepeat) => Keys.OnKeyPressed(key, isRepeat);
        public void OnKeyReleased(KeyConstant key) => Keys.OnKeyReleased(key);
        public void OnTextInput(string text) => Keys.OnTextInput(text);

        public void BindKey(Keybind keybind, Func<KeyPressState, KeyActionResult?> func, bool trackReleased = false) => Keys.BindKey(keybind, func, trackReleased);
        public void UnbindKey(Keybind keybind) => Keys.UnbindKey(keybind);
        public void HaltInput() => Keys.HaltInput();
        public bool IsModifierHeld(Keys modifier) => Keys.IsModifierHeld(modifier);
        public void RunKeyActions(float dt) => Keys.RunKeyActions(dt);

        public virtual List<UiKeyHint> MakeKeyHints() => new List<UiKeyHint>();
    }
}
