using System;

namespace OpenNefia.Core.Object.Aspect
{
    internal class AspectClassAttribute : Attribute
    {
        public Type AspectType { get; }

        public AspectClassAttribute(Type aspectType)
        {
            this.AspectType = aspectType;
        }
    }
}