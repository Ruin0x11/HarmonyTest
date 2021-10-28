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
        public T FinalValue;
        public T BaseValue;

        public Stat() : this(default(T)!) { }

        public Stat(T Value)
        {
            this.FinalValue = Value;
            this.BaseValue = Value;
        }

        public void Refresh()
        {
            this.FinalValue = this.BaseValue;
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
            data.ExposeDeep(ref FinalValue, nameof(FinalValue));
            data.ExposeDeep(ref BaseValue, nameof(BaseValue));
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
