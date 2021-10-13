using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public class UidTracker
    {
        private ulong Current;

        public UidTracker()
        {
            Current = 0;
        }

        public ulong GetNextAndIncrement()
        {
            var current = Current;
            Current++;
            return current;
        }
    }
}
