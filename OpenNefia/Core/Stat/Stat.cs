using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Stat
{
    public class Stat<T> : IStat<T>, IComparable<Stat<T>>, IEquatable<Stat<T>> where T: IDataExposable, IComparable<T>, IEquatable<T>
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

        public Stat() : this(default(T)!) { }

        public Stat(T Value)
        {
            this._FinalValue = Value;
            this._BaseValue = Value;
        }

        public void Refresh()
        {
            this._FinalValue = this._BaseValue;
            this.IsBuffed = false;
        }

        public int CompareTo(Stat<T>? other) => this.FinalValue.CompareTo(other!.FinalValue);

        public bool Equals(Stat<T>? other)
        {
            if (this.BaseValue.Equals(other!.BaseValue))
            {
                return this.FinalValue.Equals(other.FinalValue);
            }
            return false;
        }

        public override bool Equals(object? obj) => Equals(obj as Stat<T>);
        public override int GetHashCode() => HashCode.Combine(this.FinalValue, this.BaseValue);

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _IsBuffed, nameof(_IsBuffed));
            data.ExposeDeep(ref _FinalValue, nameof(_FinalValue));
            data.ExposeDeep(ref _BaseValue, nameof(_BaseValue));
        }

        public static implicit operator T(Stat<T> s) => s.BaseValue;

        public static bool operator ==(Stat<T> left, Stat<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Stat<T> left, Stat<T> right)
        {
            return !(left == right);
        }
    }
}
