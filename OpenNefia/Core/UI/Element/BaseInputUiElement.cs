using Love;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseInputUiElement : BaseUiElement, IUiInput
    {
        protected IKeyHandler KeyInput = new KeyHandler();

        public void OnKeyPressed(KeyConstant key, bool isRepeat) => KeyInput.OnKeyPressed(key, isRepeat);
        public void OnKeyReleased(KeyConstant key) => KeyInput.OnKeyReleased(key);
        public void OnTextInput(string text) => KeyInput.OnTextInput(text);

        public void BindKey(Keybind keybind, Func<KeyPressState, KeyActionResult?> func, bool trackReleased = false) => KeyInput.BindKey(keybind, func, trackReleased);
        public void UnbindKey(Keybind keybind) => KeyInput.UnbindKey(keybind);
        public void HaltInput() => KeyInput.HaltInput();
        public bool IsModifierHeld(Keys modifier) => KeyInput.IsModifierHeld(modifier);
        public void RunKeyActions(float dt) => KeyInput.RunKeyActions(dt);

        public virtual List<UiKeyHint> MakeKeyHints() => new List<UiKeyHint>();
    }
}
