namespace OpenNefia.Core.UI.Element
{
    public interface IUiElement
    {
        int Width { get; }
        int Height { get; }
        int X { get; }
        int Y { get; }

        void SetSize(int width = 0, int height = 0);
        void SetPosition(int x, int y);

        void Update(float dt);
        void Draw();
    }
}
