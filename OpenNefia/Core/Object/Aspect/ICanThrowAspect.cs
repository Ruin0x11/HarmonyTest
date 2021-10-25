namespace OpenNefia.Core.Object.Aspect
{
    internal interface ICanThrowAspect
    {
        bool ShouldDestroyOnThrow { get; }

        bool CanThrow(Chara chara);
        void OnThrownImpact(InstancedMap map, int x, int y);
    }
}