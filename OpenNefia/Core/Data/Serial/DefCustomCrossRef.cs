using System;
using System.Reflection;

namespace OpenNefia.Core.Data.Serial
{
    internal class DefCustomCrossRef<TRecv, TDef> : BaseDefCrossRef where TDef: Def
    {
        protected TRecv Target;
        protected Action<TRecv, TDef> ResolveAction;

        public DefCustomCrossRef(Type crossRefType, string crossRefId, TRecv target, Action<TRecv, TDef> onResolve) : base(crossRefType, crossRefId)
        {
            this.Target = target;
            this.ResolveAction = onResolve;
        }

        public override void OnResolve(Def def)
        {
            ResolveAction(Target, (def as TDef)!);
        }
    }
}