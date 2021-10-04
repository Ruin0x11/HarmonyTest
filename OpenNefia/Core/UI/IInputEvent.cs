namespace OpenNefia.Core.UI
{
    public interface IInputEvent
    {
        public bool Vetoed { get; }

        public void Veto();
    }
}
