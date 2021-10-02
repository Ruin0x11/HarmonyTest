using Love;
using OpenNefia.Core.Data.Types;
using System;

namespace OpenNefia.Core.UI
{
    /// <summary>
    /// Interface describing input handlers for either keypress or text input.
    /// </summary>
    public interface IKeyHandler : IKeyInput
    {
        /// <summary>
        /// Run key actions based on the current state of the key handler.
        /// </summary>
        /// <param name="dt">Frame delta time.</param>
        void RunKeyActions(float dt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool RunKeyAction(Keys key, KeyPressState state);
    }
}