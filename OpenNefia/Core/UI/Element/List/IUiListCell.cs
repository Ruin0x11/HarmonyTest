namespace OpenNefia.Core.UI.Element.List
{
    public interface IUiListCell<T> : IDrawable
    {
        public int TextWidth { get; }
        public T Data { get; set; }
        public int XOffset { get; set; }
    }
}