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
        public T BaseValue {
            get => _BaseValue;
            set
            {
                this._BaseValue = value;
                if (!IsBuffed)
                    this.FinalValue = value;
            } 
        }

        public DefStat(T Value)
        {
            this._FinalValue = Value;
            this._BaseValue = Value;
        }

        public void Refresh()
        {
            this._FinalValue = this._BaseValue;
            this.IsBuffed = false;
        }

        public bool Equals(DefStat<T>? other)
        {
            return this.BaseValue == other?.BaseValue && this.FinalValue == other?.FinalValue;
        }

        public override bool Equals(object? obj) => Equals(obj as DefStat<T>);
        public override int GetHashCode() => HashCode.Combine(this.FinalValue, this.BaseValue);

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref _IsBuffed, nameof(_IsBuffed));
            data.ExposeDef(ref _FinalValue, nameof(_FinalValue));
            data.ExposeDef(ref _BaseValue, nameof(_BaseValue));
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
