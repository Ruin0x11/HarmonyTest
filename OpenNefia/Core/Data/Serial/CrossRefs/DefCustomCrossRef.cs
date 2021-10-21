using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenNefia.Core.Data.Serial.CrossRefs
{
    public class DefCustomCrossRef<TRecv, TDef> : IDefCrossRef where TDef : Def
    {
        public delegate void OnResolveDelegate(TRecv receiver, IEnumerable<TDef> defs);

        protected TRecv Target;
        protected OnResolveDelegate ResolveAction;
        private List<DefIdentifier> DefIdentifiers;

        public DefCustomCrossRef(List<string> defIds, TRecv target, OnResolveDelegate onResolve)
        {
            Target = target;
            ResolveAction = onResolve;
            DefIdentifiers = defIds.Select(id => new DefIdentifier(typeof(TDef), id)).ToList();
        }

        public IEnumerable<DefIdentifier> GetWantedCrossRefs() => DefIdentifiers;

        public void Resolve(IEnumerable<Def> defs)
        {
            ResolveAction(Target, defs.Select(def => (def as TDef)!));
        }
    }
}