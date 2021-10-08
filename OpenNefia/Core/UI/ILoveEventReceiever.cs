using Love;

namespace OpenNefia.Core.UI
{
    public interface ILoveEventReceiever
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="is_repeat"></param>
        void OnLoveKeyPressed(KeyConstant key, bool is_repeat);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        void OnLoveKeyReleased(KeyConstant key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        void OnLoveTextInput(string text);

        void OnLoveMouseMoved(float x, float y, float dx, float dy, bool isTouch);

        void OnLoveMousePressed(float x, float y, int button, bool isTouch);

        void OnLoveMouseReleased(float x, float y, int button, bool isTouch);
    }
}