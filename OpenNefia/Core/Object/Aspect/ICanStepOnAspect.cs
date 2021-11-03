using OpenNefia.Core.Logic;

namespace OpenNefia.Core.Object.Aspect.Types
{
    public interface ICanStepOnAspect
    {
        void Event_OnSteppedOn(Chara chara);
    }
}