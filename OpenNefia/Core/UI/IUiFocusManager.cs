using OpenNefia.Core.UI.Element;

namespace OpenNefia.Core.UI
{
    public interface IUiFocusManager
    {
        public IUiInputElement? FocusedElement { get; set; }
    }
}