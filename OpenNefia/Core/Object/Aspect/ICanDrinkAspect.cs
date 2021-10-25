using OpenNefia.Core.Logic;

namespace OpenNefia.Core.Object.Aspect
{
    internal interface ICanDrinkAspect
    {
        bool ShouldConsumeOnDrink { get; }

        bool CanDrink(Chara chara);
        TurnResult OnDrink(Chara chara);
    }
}