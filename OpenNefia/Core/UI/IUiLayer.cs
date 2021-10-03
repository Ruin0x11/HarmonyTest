using OpenNefia.Core.UI.Element;

namespace OpenNefia.Core.UI
{
    public interface IUiLayer : IUiInput, IUiElement, ILoveEventReceiever
    {
        public IUiFocusManager FocusManager { get; }
    }
}
