using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Serial
{
    internal abstract class BaseDefCrossRef : IDefCrossRef
    {
        protected Type CrossRefType;
        protected string CrossRefId;

        public BaseDefCrossRef(Type crossRefType, string crossRefId)
        {
            this.CrossRefType = crossRefType; 
            this.CrossRefId = crossRefId;
        }

        public void Resolve(List<string> errors)
        {
            var defType = DefLoader.GetDirectDefType(CrossRefType);
            if (defType == null)
            {
                errors.Add($"Type {CrossRefType} is not a descendent of type that inherits from Def");
            }
            else
            {
                var def = DefLoader.GetDef(CrossRefType, CrossRefId);
                if (def == null)
                {
                    errors.Add($"Could not find def crossreference '{CrossRefType}.{CrossRefId}'");
                }
                else
                {
                    OnResolve(def);
                }
            }
        }

        public abstract void OnResolve(Def def);
    }
}
