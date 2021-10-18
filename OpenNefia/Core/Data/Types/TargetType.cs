namespace OpenNefia.Core.Data.Types
{
    public enum TargetType
    {
        Self = 0,
        SelfOrAdjacentChara,
        AdjacentChara,
        Location,
        TargetedEnemy,
        TargetedChara,
        TargetedCharaOrLocation,
        Direction
    }
}