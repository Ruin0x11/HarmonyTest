using Love;
using OpenNefia.Core.Data.Types;
using System;

namespace OpenNefia.Core.UI
{
    public interface IInputHandler : IKeyBinder, IMouseBinder, IMouseMovedBinder, ITextInputBinder, IInputForwarder
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
        void ReceiveTextInput(string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="isTouch"></param>
        void ReceiveMouseMoved(float x, float y, float dx, float dy, bool isTouch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        /// <param name="isTouch"></param>
        void ReceiveMousePressed(float x, float y, int button, bool isTouch);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="button"></param>
        /// <param name="isTouch"></param>
        void ReceiveMouseReleased(float x, float y, int button, bool isTouch);

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
        /// <param name="text"></param>
        /// <returns></returns>
        bool RunTextInputAction(string text);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void ReleaseKey(Keys key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="press"></param>
        void ReleaseMouseButton(MouseButtonPress press);

        /// <summary>
        /// Run key actions based on the current state of the key handler.
        /// </summary>
        /// <param name="dt">Frame delta time.</param>
        void RunKeyActions(float dt);
        bool RunMouseMovedAction(int x, int y, int dx, int dy);
        bool RunMouseAction(MouseButtonPress press);
    }
}
