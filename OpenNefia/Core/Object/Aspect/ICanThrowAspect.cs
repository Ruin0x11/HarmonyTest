using OpenNefia.Core.Map;

namespace OpenNefia.Core.Object.Aspect
{
    public interface ICanThrowAspect
    {
        bool Event_CanThrow(Chara chara);
        bool Event_OnThrownImpact(TilePos pos);
    }
}