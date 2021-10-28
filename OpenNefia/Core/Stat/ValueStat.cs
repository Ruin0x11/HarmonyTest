using OpenNefia.Serial;
using System;

namespace OpenNefia.Core.Stat
{
    public class ValueStat<T> : IStat<T>, IComparable<ValueStat<T>>, IEquatable<ValueStat<T>> where T : struct, IComparable<T>, IEquatable<T>
    {
        public T FinalValue;
        public T BaseValue;

        public ValueStat() : this(default(T)) { }

        public ValueStat(T Value)
        {
            this.FinalValue = Value;
            this.BaseValue = Value;
        }

        public void Refresh()
        {
            this.FinalValue = this.BaseValue;
        }

        public int CompareTo(ValueStat<T>? other) => this.FinalValue.CompareTo(other!.FinalValue);

        public bool Equals(ValueStat<T>? other)
        {
            if (this.BaseValue.Equals(other!.BaseValue))
            {
                return this.FinalValue.Equals(other.FinalValue);
            }
            return false;
        }

        public override bool Equals(object? obj) => Equals(obj as ValueStat<T>);
        public override int GetHashCode() => HashCode.Combine(this.FinalValue, this.BaseValue);

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref FinalValue, nameof(FinalValue));
            data.ExposeValue(ref BaseValue, nameof(BaseValue));
        }

        public static implicit operator T(ValueStat<T> s) => s.BaseValue;

        public static bool operator ==(ValueStat<T> left, ValueStat<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ValueStat<T> left, ValueStat<T> right)
        {
            return !(left == right);
        }
    }
}
