namespace OpenNefia.Core.UI.Element
{
    public abstract class BaseUiElement : BaseDrawable, IUiDefaultSizeable
    {
        public abstract void GetPreferredSize(out int width, out int height);

        public void SetPreferredSize()
        {
            this.GetPreferredSize(out int width, out int height);
            this.SetSize(width, height);
        }
    }
}