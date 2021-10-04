using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI
{
    internal class KeyHandler : IKeyInput
    {
        private class KeyAction
        {
            private Action<KeyInputEvent> Callback;
            public bool TrackReleased { get; private set; }

            public KeyAction(Action<KeyInputEvent> func, bool trackReleased)
            {
                this.Callback = func;
                this.TrackReleased = trackReleased;
            }

            public void Run(KeyInputEvent evt) => this.Callback(evt);
        }

        private class KeyRepeatDelay
        {
            public bool IsActive = false;
            public int WaitRemain = 0;
            public float Delay = 0.0f;
            public bool IsPressed = false;
            public bool IsRepeating = false;
            public bool IsFast = false;
            public bool FirstPress = true;
            public HashSet<IKeybind> ActiveKeybinds = new HashSet<IKeybind>();

            public void Reset()
            {
                this.IsActive = false;
                this.WaitRemain = 0;
                this.Delay = 0.0f;
                this.IsPressed = false;
                this.IsRepeating = false;
                this.IsFast = false;
                this.FirstPress = true;
                this.ActiveKeybinds = new HashSet<IKeybind>();
            }
        }

        private HashSet<Keys> Pressed;
        private Dictionary<Keys, KeyRepeatDelay> RepeatDelays;
        private HashSet<Keys> UnpressedThisFrame;
        private Keys Modifiers;
        private List<IKeyInput> Forwards;
        private Dictionary<IKeybind, KeyAction> Actions;
        private KeybindTranslator Keybinds;
        private bool Halted;
        private bool StopHalt;
        private string? TextInputThisFrame;
        private Action<TextInputEvent>? TextInputHandler;

        public bool NoShiftDelay { get; set; }
        public int KeyHeldFrames { get; private set; }
        public bool TextInputEnabled { get; set; }

        public KeyHandler()
        {
            this.Pressed = new HashSet<Keys>();
            this.RepeatDelays = new Dictionary<Keys, KeyRepeatDelay>();  
            this.UnpressedThisFrame = new HashSet<Keys>();
            this.Modifiers = Keys.None;
            this.Forwards = new List<IKeyInput>();
            this.Actions = new Dictionary<IKeybind, KeyAction>();
            this.Keybinds = new KeybindTranslator();
            this.Halted = false;
            this.StopHalt = false;
            this.NoShiftDelay = false;
            this.KeyHeldFrames = 0;
            this.TextInputEnabled = false;
            this.TextInputThisFrame = null;
            this.TextInputHandler = null;
        }

        /// <inheritdoc />
        public void ReceiveKeyPressed(Love.KeyConstant loveKey, bool isRepeat)
        {
            foreach (var forward in this.Forwards)
            {
                forward.ReceiveKeyPressed(loveKey, isRepeat);
            }

            if (this.Halted && isRepeat) {
                return;
            }

            var key = (Keys)loveKey;

            var modifier = InputUtils.GetModifier(loveKey);
            if (modifier.HasValue)
            {
                this.Modifiers |= modifier.Value;

                // Treat LShift and RShift as Shift, etc.
                key = modifier.Value;
            }

            this.Pressed.Add(key);

            if (!this.RepeatDelays.ContainsKey(key))
                this.RepeatDelays[key] = new KeyRepeatDelay();
            this.RepeatDelays[key].IsActive = true;
        }

        /// <inheritdoc />
        public void ReceiveKeyReleased(Love.KeyConstant loveKey)
        {
            foreach (var forward in this.Forwards)
            {
                forward.ReceiveKeyReleased(loveKey);
            }

            var key = (Keys)loveKey;

            var modifier = InputUtils.GetModifier(loveKey);
            if (modifier.HasValue)
            {
                this.Modifiers &= ~modifier.Value;

                // Treat LShift and RShift as Shift, etc.
                key = modifier.Value;
            }

            this.UnpressedThisFrame.Add(key);
        }

        /// <inheritdoc />
        public void ReceieveTextInput(string text)
        {
            foreach (var forward in this.Forwards)
            {
                forward.ReceieveTextInput(text);
            }

            if (!this.TextInputEnabled)
                return;

            this.TextInputThisFrame = text;
        }

        public void HaltInput()
        {
            foreach (var key in this.Pressed)
            {
                this.ReleaseKey(key);
            }

            this.RepeatDelays.Clear();
            this.Modifiers = Keys.None;
            this.Pressed.Clear();
            this.UnpressedThisFrame.Clear();
            this.Halted = true;
            this.StopHalt = false;
            this.KeyHeldFrames = 0;
            this.TextInputThisFrame = null;

            foreach (var forward in this.Forwards)
            {
                forward.HaltInput();
            }
        }

        public void UpdateKeyRepeats(float dt)
        {
            foreach (var key in this.Pressed)
            {
                var keyRepeat = this.RepeatDelays[key];
                var keybind = this.Keybinds.KeyToKeybind(key | this.Modifiers);
                var isShiftDelayed = keybind != null && keybind.IsShiftDelayed;

                if (keyRepeat.FirstPress)
                {
                    keyRepeat.FirstPress = false;

                    if (isShiftDelayed)
                    {
                        if (this.NoShiftDelay)
                        {
                            keyRepeat.WaitRemain = 0;
                            keyRepeat.Delay = 40;
                        }
                        else
                        {
                            keyRepeat.WaitRemain = 3;
                            keyRepeat.Delay = 200;
                        }
                    }
                    else
                    {
                        keyRepeat.WaitRemain = 0;
                        keyRepeat.Delay = 600;
                    }
                    keyRepeat.IsPressed = true;
                }

                keyRepeat.Delay -= dt * 1000f;
                if (keyRepeat.Delay <= 0)
                {
                    keyRepeat.IsPressed = true;
                }

                if (isShiftDelayed && (this.Modifiers & Keys.Shift) == Keys.Shift)
                {
                    keyRepeat.Delay = 10;
                }
            }

            foreach (var forward in this.Forwards)
            {
                forward.UpdateKeyRepeats(dt);
            }
        }

        public void ForwardTo(IKeyInput keys, int? priority = null)
        {
            if (this == keys)
                throw new ArgumentException("Cannot forward key handler to itself");

            this.Forwards.Add(keys);
        }

        public void UnforwardTo(IKeyInput keys)
        {
            this.Forwards.Remove(keys);
        }
        
        public void ClearAllForwards()
        {
            this.Forwards.Clear();
        }

        private bool AddKeyDelay(Keys keyWithoutModifiers, bool isShiftDelayed)
        {
            if (!this.RepeatDelays.ContainsKey(keyWithoutModifiers))
                this.RepeatDelays[keyWithoutModifiers] = new KeyRepeatDelay();
            var keyRepeat = this.RepeatDelays[keyWithoutModifiers]!;

            keyRepeat!.WaitRemain--;
            if (keyRepeat.WaitRemain <= 0)
            {
                if (isShiftDelayed)
                {
                    if (this.NoShiftDelay)
                    {
                        keyRepeat.Delay = 100;
                    }
                    else
                    {
                        keyRepeat.Delay = 20;
                    }
                }
                if (keyRepeat.IsFast)
                {
                    keyRepeat.IsRepeating = true;
                }
                keyRepeat.IsFast = true;
            }
            else if (keyRepeat.IsFast)
            {
                if (isShiftDelayed)
                {
                    // TODO
                    if (this.NoShiftDelay)
                    {
                        keyRepeat.Delay = 100;
                    }
                    else
                    {
                        keyRepeat.Delay = 20;
                    }
                }
                else
                {
                    keyRepeat.Delay = 10;
                }
            }
            else
            {
                keyRepeat.Delay = 200;
            }
            keyRepeat.IsPressed = false;

            return keyRepeat.IsRepeating;
        }

        public bool RunKeyAction(Keys keyAndModifiers, KeyPressState state)
        {
            var keyWithoutModifiers = keyAndModifiers & (~Keys.AllModifiers);

            if (this.RepeatDelays.TryGetValue(keyWithoutModifiers, out KeyRepeatDelay? repeatDelay))
            {
                if (repeatDelay.IsPressed)
                {
                    var keybind = this.Keybinds.KeyToKeybind(keyAndModifiers);
                    var isShiftDelayed = keybind != null && keybind.IsShiftDelayed;

                    var isRepeating = this.AddKeyDelay(keyWithoutModifiers, isShiftDelayed);

                    if (keybind != null)
                    {
                        if (this.Actions.TryGetValue(keybind, out KeyAction? action))
                        {
                            if (state != KeyPressState.Released || (state == KeyPressState.Released && action.TrackReleased))
                            {
                                if (isRepeating && state == KeyPressState.Pressed)
                                    state = KeyPressState.Repeated;

                                var evt = new KeyInputEvent(state);
                                action.Run(evt);
                                if (!evt.Vetoed)
                                {
                                    repeatDelay.ActiveKeybinds.Add(keybind);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var forward in this.Forwards)
            {
                if (forward.RunKeyAction(keyAndModifiers, state))
                {
                    return true;
                }
            }

            return false;
        }

        public bool RunTextInputAction(string text)
        {
            if (this.TextInputHandler != null)
            {
                var evt = new TextInputEvent(text);
                this.TextInputHandler(evt);
                if (!evt.Vetoed)
                {
                    return true;
                }
            }

            foreach (var forward in this.Forwards)
            {
                if (forward.RunTextInputAction(text))
                {
                    return true;
                }
            }

            return false;
        }

        public void BindKey(IKeybind keybind, Action<KeyInputEvent> handler, bool trackReleased = false)
        {
            this.Actions[keybind] = new KeyAction(handler, trackReleased);
            this.Keybinds.Enable(keybind);
        }

        public void UnbindKey(IKeybind keybind)
        {
            this.Actions.Remove(keybind);
            this.Keybinds.Disable(keybind);
        }

        public void BindTextInput(Action<TextInputEvent> handler)
        {
            this.TextInputHandler = handler;
        }

        public void UnbindTextInput()
        {
            this.TextInputHandler = null;
        }

        public bool IsModifierHeld(Keys modifier)
        {
            return (this.Modifiers & modifier) == modifier;
        }

        public void ReleaseKey(Keys key)
        {
            this.Pressed.Remove(key);

            if (this.RepeatDelays.TryGetValue(key & (~Keys.AllModifiers), out var repeatDelay))
            {
                foreach (var activeKeybind in repeatDelay.ActiveKeybinds)
                {
                    if (this.Actions.TryGetValue(activeKeybind, out KeyAction? action))
                    {
                        if (action.TrackReleased)
                        {
                            var evt = new KeyInputEvent(KeyPressState.Released);
                            action.Run(evt);
                        }
                    }
                }

                repeatDelay.Reset();
            }

            foreach (var forward in this.Forwards)
            {
                forward.ReleaseKey(key);
            }
        }

        public void RunKeyActions(float dt)
        {
            foreach (var key in this.UnpressedThisFrame)
            {
                this.ReleaseKey(key);
            }

            var ran = false;

            foreach (var (key, repeatDelay) in this.RepeatDelays)
            {
                if (repeatDelay.IsActive)
                {
                    ran = this.RunKeyAction(key | this.Modifiers, KeyPressState.Pressed);
                    if (ran)
                    {
                        // Only run the first key action.
                        break;
                    }
                }
            }

            if (this.TextInputThisFrame != null)
            {
                if (this.TextInputEnabled)
                {
                    this.RunTextInputAction(this.TextInputThisFrame);
                }

                this.TextInputThisFrame = null;
            }

            this.UnpressedThisFrame.Clear();

            this.UpdateKeyRepeats(dt);

            this.Halted = this.Halted && !this.StopHalt;

            if (this.Pressed.Count > 0)
            {
                if (ran)
                {
                    this.KeyHeldFrames++;
                }
            }
            else
            {
                this.KeyHeldFrames = 0;
            }
        }
    }
}
