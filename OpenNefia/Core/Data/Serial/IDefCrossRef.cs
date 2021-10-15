using System.Collections.Generic;

namespace OpenNefia.Core.Data.Serial
{
    internal interface IDefCrossRef
    {
        void Resolve(List<string> errors);
    }
}