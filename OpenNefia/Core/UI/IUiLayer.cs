using OpenNefia.Core.UI.Element;

namespace OpenNefia.Core.UI
{
    public interface IUiLayer : IUiInput, IUiElement
    {
        public void OnKeyPressed(Love.KeyConstant key, bool is_repeat);
        public void OnKeyReleased(Love.KeyConstant key);
        public void OnTextInput(string text);

        public void HaltInput();
    }
}
