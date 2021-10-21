using System.Collections.Generic;

namespace OpenNefia.Core.Data.Serial.CrossRefs
{
    internal class DefHashSetCrossRef : IDefCrossRef
    {
        private HashSet<object> Target;
        private HashSet<DefIdentifier> DefIdentifiers;

        public DefHashSetCrossRef(HashSet<object> target, HashSet<DefIdentifier> defIdentifiers)
        {
            Target = target;
            DefIdentifiers = defIdentifiers;
        }

        public IEnumerable<DefIdentifier> GetWantedCrossRefs() => DefIdentifiers;

        public void Resolve(IEnumerable<Def> defs)
        {
            foreach (var def in defs)
            {
                Target.Add(def);
            }
        }
    }
}
