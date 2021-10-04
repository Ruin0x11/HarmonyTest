using Love;
using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseInputUiElement : BaseUiElement, IUiInputElement
    {
        protected IKeyInput KeyInput;
        public KeybindWrapper Keybinds { get; }

        // The empty setters are to support += syntax.

        public TextInputWrapper TextInput { get; }

        private KeyForwardsWrapper _Forwards;
        public KeyForwardsWrapper Forwards { 
            get => _Forwards; 
            set {}
        }

        public bool TextInputEnabled {
            get => this.KeyInput.TextInputEnabled;
            set => this.KeyInput.TextInputEnabled = value;
        }

        public BaseInputUiElement()
        {
            this.KeyInput = new KeyHandler();
            this.Keybinds = new KeybindWrapper(this.KeyInput);
            this.TextInput = new TextInputWrapper(this.KeyInput);
            this._Forwards = new KeyForwardsWrapper(this.KeyInput);
        }

        public void ReceiveKeyPressed(KeyConstant key, bool isRepeat) => KeyInput.ReceiveKeyPressed(key, isRepeat);
        public void ReceiveKeyReleased(KeyConstant key) => KeyInput.ReceiveKeyReleased(key);
        public void ReceieveTextInput(string text) => KeyInput.ReceieveTextInput(text);

        public void BindKey(IKeybind keybind, Action<KeyInputEvent> func, bool trackReleased = false) => KeyInput.BindKey(keybind, func, trackReleased);
        public void UnbindKey(IKeybind keybind) => KeyInput.UnbindKey(keybind);
        public void ForwardTo(IKeyInput keys, int? priority = null) => KeyInput.ForwardTo(keys, priority);
        public void UnforwardTo(IKeyInput keys) => KeyInput.UnforwardTo(keys);
        public void ClearAllForwards() => KeyInput.ClearAllForwards();
        public void HaltInput() => KeyInput.HaltInput();
        public bool IsModifierHeld(Keys modifier) => KeyInput.IsModifierHeld(modifier);
        public void UpdateKeyRepeats(float dt) => KeyInput.UpdateKeyRepeats(dt);
        public void RunKeyActions(float dt) => KeyInput.RunKeyActions(dt);
        public bool RunKeyAction(Keys key, KeyPressState state) => KeyInput.RunKeyAction(key, state);
        public void ReleaseKey(Keys key) => KeyInput.ReleaseKey(key);
        public bool RunTextInputAction(string text) => KeyInput.RunTextInputAction(text);
        public void BindTextInput(Action<TextInputEvent> evt) => KeyInput.BindTextInput(evt);
        public void UnbindTextInput() => KeyInput.UnbindTextInput();

        public virtual List<UiKeyHint> MakeKeyHints() => new List<UiKeyHint>();
    }
}
