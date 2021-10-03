namespace OpenNefia.Core.UI.Element.List
{
    public interface IUiListCell<T> : IUiElement
    {
        public T Data { get; set; }
        public int XOffset { get; set; }
    }
}