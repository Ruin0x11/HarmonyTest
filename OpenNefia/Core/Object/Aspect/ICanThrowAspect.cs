using OpenNefia.Core.Map;

namespace OpenNefia.Core.Object.Aspect
{
    public interface ICanThrowAspect
    {
        bool CanThrow(Chara chara);
        bool OnThrownImpact(TilePos pos);
    }
}