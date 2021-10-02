using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;

namespace OpenNefia.Core.UI
{
    public class KeybindTranslator
    {
        private Dictionary<Keys, Keybind> Translations;
        private HashSet<Keybind> AcceptedKeybinds;
        private bool Dirty;

        public KeybindTranslator()
        {
            this.Translations = new Dictionary<Keys, Keybind>();
            this.AcceptedKeybinds = new HashSet<Keybind>();
            this.Dirty = true;
        }

        internal void Enable(Keybind keybind)
        {
            this.AcceptedKeybinds.Add(keybind);
            this.Dirty = true;
        }

        internal void Disable(Keybind keybind)
        {
            this.AcceptedKeybinds.Remove(keybind);
            this.Dirty = true;
        }

        public void Reload()
        {
            this.BindKey(Keys.Return, Keybind.Entries.Enter);
            this.BindKey(Keys.Shift, Keybind.Entries.Cancel);
            this.BindKey(Keys.Escape, Keybind.Entries.Escape);
            this.BindKey(Keys.Escape, Keybind.Entries.Quit);

            this.BindKey(Keys.Up, Keybind.Entries.UIUp);
            this.BindKey(Keys.Down, Keybind.Entries.UIDown);
            this.BindKey(Keys.Left, Keybind.Entries.UILeft);
            this.BindKey(Keys.Right, Keybind.Entries.UIRight);

            this.BindKey(Keys.Up, Keybind.Entries.North);
            this.BindKey(Keys.Down, Keybind.Entries.South);
            this.BindKey(Keys.Left, Keybind.Entries.West);
            this.BindKey(Keys.Right, Keybind.Entries.East);

            this.BindKey(Keys.Period, Keybind.Entries.Wait);
            this.BindKey(Keys.X, Keybind.Entries.Identify);
            this.BindKey(Keys.Z, Keybind.Entries.Mode);
            this.BindKey(Keys.KeypadMultiply, Keybind.Entries.Mode2);

            this.Dirty = false;
        }

        public void BindKey(Keys keyAndModifiers, Keybind keybind)
        {
            if (this.AcceptedKeybinds.Contains(keybind))
            {
                this.Translations[keyAndModifiers] = keybind;
            }
        }

        public Keybind? KeyToKeybind(Keys keyAndModifiers)
        {
            if (this.Dirty)
            {
                // Get the list of all key bindings and bind only the ones that have actions set.
                this.Reload();
            }

            if (this.Translations.TryGetValue(keyAndModifiers, out Keybind? keybind))
            {
                return keybind;
            }

            return null;
        }
    }
}
