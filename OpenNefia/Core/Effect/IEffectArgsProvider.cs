using OpenNefia.Core.Effect;

namespace OpenNefia.Core.Object.Aspect
{
    internal interface IEffectArgsProvider
    {
        EffectArguments GetEffectArgs(MapObject chara, TriggeredBy triggeredBy);
    }
}