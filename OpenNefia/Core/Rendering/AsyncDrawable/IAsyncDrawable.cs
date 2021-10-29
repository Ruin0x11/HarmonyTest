using OpenNefia.Core.UI.Element;

namespace OpenNefia.Core.Rendering
{
    public interface IAsyncDrawable : IDrawable
    {
        public bool IsFinished { get; }
        public int ScreenLocalX { get; set; }
        public int ScreenLocalY { get; set; }

        public void OnEnqueue();
        public void OnThemeSwitched();
    }
}
