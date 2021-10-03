using Love;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseInputUiElement : BaseUiElement, IUiInputElement
    {
        protected IKeyInput KeyInput = new KeyHandler();

        public void ReceiveKeyPressed(KeyConstant key, bool isRepeat) => KeyInput.ReceiveKeyPressed(key, isRepeat);
        public void ReceiveKeyReleased(KeyConstant key) => KeyInput.ReceiveKeyReleased(key);
        public void ReceieveTextInput(string text) => KeyInput.ReceieveTextInput(text);

        public void BindKey(Keybind keybind, Action<KeyInputEvent> func, bool trackReleased = false) => KeyInput.BindKey(keybind, func, trackReleased);
        public void UnbindKey(Keybind keybind) => KeyInput.UnbindKey(keybind);
        public void ForwardTo(IKeyInput keys, int? priority = null) => KeyInput.ForwardTo(keys, priority);
        public void HaltInput() => KeyInput.HaltInput();
        public bool IsModifierHeld(Keys modifier) => KeyInput.IsModifierHeld(modifier);
        public void RunKeyActions(float dt) => KeyInput.RunKeyActions(dt);
        public bool RunKeyAction(Keys key, KeyPressState state) => KeyInput.RunKeyAction(key, state);
        public void ReleaseKey(Keys key) => KeyInput.ReleaseKey(key);

        public virtual List<UiKeyHint> MakeKeyHints() => new List<UiKeyHint>();
    }
}
