using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Serial
{
    public enum SerialStage
    {
        Invalid,
        Saving,
        LoadingDeep,
        ResolvingRefs,
        Finished
    }
}
