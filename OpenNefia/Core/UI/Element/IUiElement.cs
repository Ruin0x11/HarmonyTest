namespace OpenNefia.Core.UI.Element
{
    public interface IUiElement
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public void Relayout(int x, int y, int width, int height, RelayoutMode mode = RelayoutMode.Layout);
        public void Update(float dt);
        public void Draw();
    }
}
