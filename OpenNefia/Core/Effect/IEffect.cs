using OpenNefia.Core.Data.Serial;
using OpenNefia.Core.Object;

namespace OpenNefia.Core.Effect
{
    public interface IEffect : IDefDeserializable
    {
        EffectResult Apply(Chara chara, EffectArguments args);
    }
}