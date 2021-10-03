using Love;
using OpenNefia.Core.Data.Types;
using System;

namespace OpenNefia.Core.UI
{
    public interface IKeyInput
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
        /// <param name="key"></param>
        /// <param name="func"></param>
        void BindKey(Keybind keybind, Action<KeyInputEvent> func, bool trackReleased = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        void UnbindKey(Keybind keybind);

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

    }
}