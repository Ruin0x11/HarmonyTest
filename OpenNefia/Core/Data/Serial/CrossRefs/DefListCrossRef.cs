using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Serial.CrossRefs
{
    internal class DefListCrossRef<T> : IDefCrossRef where T : Def
    {
        private List<T> Target;
        private List<DefIdentifier> DefIdentifiers;

        public DefListCrossRef(List<T> target, List<DefIdentifier> defIdentifiers)
        {
            Target = target;
            DefIdentifiers = defIdentifiers;
        }

        public IEnumerable<DefIdentifier> GetWantedCrossRefs() => DefIdentifiers;

        public void Resolve(IEnumerable<Def> defs)
        {
            Target.AddRange(defs.Select(x => (x as T)!));
        }
    }
}
