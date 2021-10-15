using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Stat
{
    public class ValueStat<T> : IRefreshable, IDataExposable where T : struct
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

        public void Expose(DataExposer data)
        {
            data.ExposeValue(ref FinalValue, nameof(FinalValue));
            data.ExposeValue(ref BaseValue, nameof(BaseValue));
        }

        public static implicit operator T(ValueStat<T> s) => s.BaseValue;
    }
}
