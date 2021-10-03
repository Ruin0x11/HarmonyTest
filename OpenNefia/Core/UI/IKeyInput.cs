using Love;
using OpenNefia.Core.Data.Types;
using System;

namespace OpenNefia.Core.UI
{
    public interface IKeyInput : IKeyBinder, IKeyForwarder
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="is_repeat"></param>
        void ReceiveKeyPressed(KeyConstant key, bool is_repeat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void ReceiveKeyReleased(KeyConstant key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        void ReceieveTextInput(string text);

        /// <summary>
        /// 
        /// </summary>
        void HaltInput();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        bool IsModifierHeld(Keys modifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>
        public void UpdateKeyRepeats(float dt);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        bool RunKeyAction(Keys key, KeyPressState state);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void ReleaseKey(Keys key);

        /// <summary>
        /// Run key actions based on the current state of the key handler.
        /// </summary>
        /// <param name="dt">Frame delta time.</param>
        void RunKeyActions(float dt);
    }
}