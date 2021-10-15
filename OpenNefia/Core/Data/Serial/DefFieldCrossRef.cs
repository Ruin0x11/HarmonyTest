using System;
using System.Collections.Generic;
using System.Reflection;

namespace OpenNefia.Core.Data.Serial
{
    internal class DefFieldCrossRef : BaseDefCrossRef
    {
        protected object Target;
        protected FieldInfo TargetField;

        public DefFieldCrossRef(Type crossRefType, string crossRefId, object target, FieldInfo targetField) : base(crossRefType, crossRefId)
        {
            this.Target = target;
            this.TargetField = targetField;
        }

        public override void OnResolve(Def def)
        {
            TargetField.SetValue(Target, def);
        }
    }
}