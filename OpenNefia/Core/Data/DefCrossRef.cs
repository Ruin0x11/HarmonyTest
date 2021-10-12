using System;
using System.Reflection;

namespace OpenNefia.Core.Data
{
    internal class DefCrossRef
    {
        internal object target;
        internal PropertyInfo targetProperty;
        internal Type crossRefType;
        internal string crossRefId;

        public DefCrossRef(object target, PropertyInfo targetProperty, Type crossRefType, string crossRefId)
        {
            this.target = target;
            this.targetProperty = targetProperty;
            this.crossRefType = crossRefType;
            this.crossRefId = crossRefId;
        }
    }
}