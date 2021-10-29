namespace OpenNefia.Core.Object.Aspect
{
    public abstract class MapObjectAspect
    {
        public AspectProperties Props = null!;
        public MapObject Owner;

        public MapObjectAspect(MapObject owner)
        {
            Owner = owner;
        }

        public virtual bool CanStackWith(MapObject other)
        {
            return true;
        }

        public virtual void AfterStacked()
        {

        }

        public virtual void Initialize(AspectProperties props)
        {
            Props = props;
        }
    }
}