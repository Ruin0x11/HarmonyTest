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
            return this.BaseValue == other?.BaseValue && this.FinalValue == other?.FinalValue;
        }

        public override bool Equals(object? obj) => Equals(obj as DefStat<T>);
        public override int GetHashCode() => HashCode.Combine(this.FinalValue, this.BaseValue);

        public void Expose(DataExposer data)
        {
            data.ExposeDef(ref FinalValue, nameof(FinalValue));
            data.ExposeDef(ref BaseValue, nameof(BaseValue));
        }

        public static implicit operator T(DefStat<T> s) => s.BaseValue;

        public static bool operator ==(DefStat<T> left, DefStat<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DefStat<T> left, DefStat<T> right)
        {
            return !(left == right);
        }
    }
}
