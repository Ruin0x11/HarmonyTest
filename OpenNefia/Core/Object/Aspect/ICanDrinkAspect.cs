using OpenNefia.Core.Logic;

namespace OpenNefia.Core.Object.Aspect
{
    internal interface ICanDrinkAspect
    {
        bool ShouldConsumeOnDrink { get; }

        bool Event_CanDrink(Chara chara);
        TurnResult Event_OnDrink(Chara chara);
    }
}