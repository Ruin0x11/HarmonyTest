namespace OpenNefia.Core
{
    public interface IEventResult<T>
    {
        T Result { get; set; }
        bool IsVetoed { get; }

        void Veto();
    }
}