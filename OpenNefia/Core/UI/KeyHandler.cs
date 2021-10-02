﻿using OpenNefia.Core.Data.Types;
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

        private HashSet<Keys> Pressed;
        private Dictionary<Keys, float> RepeatDelays;
        private HashSet<Keys> UnpressedThisFrame;
        private Keys Modifiers;
        private List<IKeyHandler> Forwards;
        private Dictionary<Keybind, KeyAction> Actions;
        private KeybindTranslator Keybinds;
        private bool Halted;
        private bool StopHalt;

        public int KeyHeldFrames { get; private set; }

        public KeyHandler()
        {
            this.Pressed = new HashSet<Keys>();
            this.RepeatDelays = new Dictionary<Keys, float>();  
            this.UnpressedThisFrame = new HashSet<Keys>();
            this.Modifiers = Keys.None;
            this.Forwards = new List<IKeyHandler>();
            this.Actions = new Dictionary<Keybind, KeyAction>();
            this.Keybinds = new KeybindTranslator();
            this.Halted = false;
            this.StopHalt = false;
            this.KeyHeldFrames = 0;
        }

        /// <inheritdoc />
        public void OnKeyPressed(Love.KeyConstant loveKey, bool is_repeat)
        {
            foreach (var forward in this.Forwards)
            {
                forward.OnKeyPressed(loveKey, is_repeat);
            }

            if (this.Halted && is_repeat) {
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

            //TEMP
            this.RepeatDelays[key] = 0.0f;
        }

        /// <inheritdoc />
        public void OnKeyReleased(Love.KeyConstant loveKey)
        {
            foreach (var forward in this.Forwards)
            {
                forward.OnKeyReleased(loveKey);
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
        public void OnTextInput(string text)
        {
            foreach (var forward in this.Forwards)
            {
                forward.OnTextInput(text);
            }
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

            foreach (var forward in this.Forwards)
            {
                forward.HaltInput();
            }
        }

        public void ForwardTo(IKeyHandler handler, int priority)
        {
            this.Forwards.Add(handler);
        }

        public bool RunKeyAction(Keys keyAndModifiers, KeyPressState state)
        {
            var keybind = this.Keybinds.KeyToKeybind(keyAndModifiers);

            if (keybind != null)
            {
                if (this.Actions.TryGetValue(keybind, out KeyAction? action))
                {
                    // BUG: Release events won't be counted properly for modifiers,
                    // since they only look at the modifier state when the key is
                    // released, not when any modifier is released.
                    if (state != KeyPressState.Released || (state == KeyPressState.Released && action.TrackReleased))
                    {
                        var result = action.Run(state);
                        if (result == null || result == KeyActionResult.Complete)
                        {
                            return true;
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

        public void BindKey(Keybind keybind, Func<KeyPressState, KeyActionResult?> func, bool trackReleased = false)
        {
            this.Actions[keybind] = new KeyAction(func, trackReleased);
            this.Keybinds.Enable(keybind);
        }

        public void UnbindKey(Keybind keybind)
        {
            this.Actions.Remove(keybind);
            this.Keybinds.Disable(keybind);
        }

        public bool IsModifierHeld(Keys modifier)
        {
            return (this.Modifiers & modifier) == modifier;
        }

        public void ReleaseKey(Keys key)
        {
            this.Pressed.Remove(key);
            this.RepeatDelays.Remove(key);

            if (this.RunKeyAction(key | this.Modifiers, KeyPressState.Released))
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

        public void RunKeyActions(float dt)
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
                    ran = this.RunKeyAction(key | this.Modifiers, KeyPressState.Pressed);
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