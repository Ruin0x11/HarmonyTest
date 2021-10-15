using OpenNefia.Core.Data;
using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Stat
{
    public class DefStat<T> : IStat<T>, IEquatable<DefStat<T>> where T : Def
    {
        public T FinalValue;
        public T BaseValue;

        public DefStat(T Value)
        {
            this.FinalValue = Value;
            this.BaseValue = Value;
        }

        public void Refresh()
        {
            this.FinalValue = this.BaseValue;
        }

        public bool Equals(DefStat<T>? other)
        {
            if (this.BaseValue.Equals(other?.BaseValue))
            {
                return this.FinalValue.Equals(other?.FinalValue);
            }
            return this.BaseValue.Equals(other?.BaseValue);
        }

        public void Expose(DataExposer data)
        {
            data.ExposeDef(ref FinalValue, nameof(FinalValue));
            data.ExposeDef(ref BaseValue, nameof(BaseValue));
        }

        public static implicit operator T(DefStat<T> s) => s.BaseValue;
    }
}
