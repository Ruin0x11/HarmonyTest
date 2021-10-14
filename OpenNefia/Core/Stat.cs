using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public struct Stat<T>
    {
        public T Value { get; set; }
        public T OriginalValue { get; set; }

        public Stat(T Value)
        {
            this.Value = Value;
            this.OriginalValue = Value;
        }

        public void Refresh()
        {
            this.Value = this.OriginalValue;
        }

        public static implicit operator T(Stat<T> s) => s.OriginalValue;
    }
}
