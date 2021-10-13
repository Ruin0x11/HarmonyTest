using OpenNefia.Core.Rendering;

namespace OpenNefia.Core.UI.Element
{
    public interface IUiElement : IDrawable
    {
        int Width { get; }
        int Height { get; }
        int X { get; }
        int Y { get; }

        void SetSize(int width = 0, int height = 0);
        void SetPosition(int x, int y);
        bool ContainsPoint(int x, int y);
    }
}
