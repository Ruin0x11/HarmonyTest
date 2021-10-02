using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI
{
    internal class KeyHandler : IKeyHandler
    {
        private class KeyAction
        {
            private Func<KeyPressState, KeyActionResult?> Callback;
            public bool TrackReleased { get; private set; }

            public KeyAction(Func<KeyPressState, KeyActionResult?> func, bool trackReleased)
            {
                this.Callback = func;
                this.TrackReleased = trackReleased;
            }

            public KeyActionResult? Run(KeyPressState state) => this.Callback(state);
        }

        private HashSet<Love.KeyConstant> Pressed;
        private Dictionary<Love.KeyConstant, float> RepeatDelays;
        private HashSet<Love.KeyConstant> UnpressedThisFrame;
        private Keys Modifiers;
        private List<IKeyHandler> Forwards;
        private Dictionary<Keys, KeyAction> Actions;
        private bool Halted;
        private bool StopHalt;

        public int KeyHeldFrames { get; private set; }

        public KeyHandler()
        {
            this.Pressed = new HashSet<Love.KeyConstant>();
            this.RepeatDelays = new Dictionary<Love.KeyConstant, float>();  
            this.UnpressedThisFrame = new HashSet<Love.KeyConstant>();
            this.Modifiers = Keys.None;
            this.Forwards = new List<IKeyHandler>();
            this.Actions = new Dictionary<Keys, KeyAction>();
            this.Halted = false;
            this.StopHalt = false;
            this.KeyHeldFrames = 0;
        }

        /// <inheritdoc />
        public void OnKeyPressed(Love.KeyConstant key, bool is_repeat)
        {
            foreach (var forward in this.Forwards)
            {
                forward.OnKeyPressed(key, is_repeat);
            }

            if (this.Halted && is_repeat) {
                return;
            }

            var modifier = InputUtils.GetModifier(key);
            if (modifier.HasValue)
            {
                this.Modifiers |= modifier.Value;
            }

            this.Pressed.Add(key);

            //TEMP
            this.RepeatDelays[key] = 0.0f;
        }

        /// <inheritdoc />
        public void OnKeyReleased(Love.KeyConstant key)
        {
            foreach (var forward in this.Forwards)
            {
                forward.OnKeyReleased(key);
            }

            var modifier = InputUtils.GetModifier(key);
            if (modifier.HasValue)
            {
                this.Modifiers &= ~modifier.Value;
            }

            this.UnpressedThisFrame.Add(key);
        }

        /// <inheritdoc />
        public void OnTextInput(string text)
        {
            foreach (var forward in this.Forwards)
            {
                forward.OnTextInput(text);
            }
        }

        public void HaltInput()
        {
            this.RepeatDelays.Clear();
            this.Modifiers = Keys.None;
            this.Pressed.Clear();
            this.UnpressedThisFrame.Clear();
            this.Halted = true;
            this.StopHalt = false;
            this.KeyHeldFrames = 0;

            foreach (var forward in this.Forwards)
            {
                forward.HaltInput();
            }
        }

        public void ForwardTo(IKeyHandler handler, int priority)
        {
            this.Forwards.Add(handler);
        }

        public bool RunKeyAction(Love.KeyConstant key, KeyPressState state)
        {
            var keyAndModifiers = ((Keys)key | this.Modifiers);
            if (this.Actions.TryGetValue(keyAndModifiers, out KeyAction? action)) {
                if (state != KeyPressState.Released || (state == KeyPressState.Released && action.TrackReleased))
                {
                    var result = action.Run(state);
                    if (result == null || result == KeyActionResult.Complete)
                    {
                        return true;
                    }
                }
            }

            foreach (var forward in this.Forwards)
            {
                if (forward.RunKeyAction(key, state))
                {
                    return true;
                }
            }

            return false;
        }

        public void BindKey(Keys key, Func<KeyPressState, KeyActionResult?> func, bool trackReleased = false)
        {
            this.Actions[key] = new KeyAction(func, trackReleased);
        }

        public bool UnbindKey(Keys key)
        {
            return this.Actions.Remove(key);
        }

        public bool IsModifierHeld(Keys modifier)
        {
            return (this.Modifiers & modifier) == modifier;
        }

        public void ReleaseKey(Love.KeyConstant key)
        {
            this.Pressed.Remove(key);
            this.RepeatDelays.Remove(key);

            if (this.RunKeyAction(key, KeyPressState.Released))
            {
                return;
            }

            foreach (var forward in this.Forwards)
            {
                forward.ReleaseKey(key);
            }
        }
        
        public void UpdateRepeats(float dt)
        {
            foreach (var key in this.RepeatDelays.Keys)
            {
                this.RepeatDelays[key] += dt;
            }
        }

        public void RunActions(float dt)
        {
            foreach (var key in this.UnpressedThisFrame)
            {
                this.ReleaseKey(key);
            }

            var ran = false;

            foreach (var pair in this.RepeatDelays)
            {
                // TEMP
                var pressed = pair.Value == 0.0f;
                if (pressed) {
                    var key = pair.Key;
                    ran = this.RunKeyAction(key, KeyPressState.Pressed);
                    if (ran)
                    {
                        // Only run the first key action.
                        break;
                    }
                }
            }

            this.UnpressedThisFrame.Clear();

            this.UpdateRepeats(dt);

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