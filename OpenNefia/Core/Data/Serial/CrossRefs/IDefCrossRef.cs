using System;
using System.Collections.Generic;

namespace OpenNefia.Core.Data.Serial.CrossRefs
{
    public interface IDefCrossRef
    {
        IEnumerable<DefIdentifier> GetWantedCrossRefs();

        void Resolve(IEnumerable<Def> defs);
    }
}