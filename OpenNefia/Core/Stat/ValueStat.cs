using OpenNefia.Serial;
using System;

namespace OpenNefia.Core.Stat
{
    public class ValueStat<T> : IStat<T>, IComparable<ValueStat<T>>, IEquatable<ValueStat<T>> where T : struct, IComparable<T>, IEquatable<T>
    {
        private bool _IsBuffed = false;
        public bool IsBuffed { get => _IsBuffed; private set => _IsBuffed = value; }

        private T _FinalValue;
        public T FinalValue
        {
            get => _FinalValue;
            set
            {
                this._FinalValue = value;
                this.IsBuffed = true;
            }
        }

        private T _BaseValue;
        public T BaseValue
        {
            get => _BaseValue;
            set
            {
                this._BaseValue = value;
                if (!IsBuffed)
                    this.FinalValue = value;
            }
        }

        public ValueStat() : this(default(T)) { }

        public ValueStat(T Value)
        {
            this._FinalValue = Value;
            this._BaseValue = Value;
        }

        public void Refresh()
        {
            this._FinalValue = this._BaseValue;
            this.IsBuffed = false;
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
            data.ExposeValue(ref _IsBuffed, nameof(_IsBuffed));
            data.ExposeValue(ref _FinalValue, nameof(_FinalValue));
            data.ExposeValue(ref _BaseValue, nameof(_BaseValue));
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
