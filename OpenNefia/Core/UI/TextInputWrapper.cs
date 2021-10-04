using OpenNefia.Core.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    /// <summary>
    /// Provides some convenient syntax for defining text input handlers on UI classes that support them.
    /// </summary>
    public class TextInputWrapper
    {
        public class TextInputDelegateWrapper : ITextInputBinder
        {
            public TextInputWrapper Parent { get; }

            public TextInputDelegateWrapper(TextInputWrapper parent)
            {
                this.Parent = parent;
            }

            public static TextInputDelegateWrapper operator +(TextInputDelegateWrapper forwardsWrapper, Action<TextInputEvent> handler)
            {
                forwardsWrapper.BindTextInput(handler);
                return forwardsWrapper;
            }

            public void Clear() => this.UnbindTextInput();

            public void BindTextInput(Action<TextInputEvent> handler) => this.Parent.KeyInput.BindTextInput(handler);
            public void UnbindTextInput() => this.Parent.KeyInput.UnbindTextInput();

            public bool TextInputEnabled
            {
                get => this.Parent.KeyInput.TextInputEnabled;
                set => this.Parent.KeyInput.TextInputEnabled = value;
            }
        }

        public bool Enabled { get => this.KeyInput.TextInputEnabled; set => this.KeyInput.TextInputEnabled = value; }
        public IKeyInput KeyInput { get; }

        // The empty setter is for supporting += syntax.

        private TextInputDelegateWrapper _Callback;
        public TextInputDelegateWrapper Callback
        {
            get => _Callback;
            set {}
        }

        public TextInputWrapper(IKeyInput parent)
        {
            this.KeyInput = parent;
            this._Callback = new TextInputDelegateWrapper(this);
        }
    }
}
