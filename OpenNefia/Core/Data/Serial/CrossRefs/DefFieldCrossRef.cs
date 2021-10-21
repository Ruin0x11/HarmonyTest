using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenNefia.Core.Data.Serial.CrossRefs
{
    internal class DefFieldCrossRef : IDefCrossRef
    {
        protected DefIdentifier DefIdentifier;
        protected object Target;
        protected FieldInfo TargetField;

        public DefFieldCrossRef(DefIdentifier defIdentifier, object target, FieldInfo targetField)
        {
            DefIdentifier = defIdentifier;
            Target = target;
            TargetField = targetField;
        }

        public IEnumerable<DefIdentifier> GetWantedCrossRefs()
        {
            yield return this.DefIdentifier;
        }

        public void Resolve(IEnumerable<Def> defs)
        {
            TargetField.SetValue(Target, defs.First());
        }
    }
}