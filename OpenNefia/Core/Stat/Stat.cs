using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Stat
{
    public class Stat<T> : IRefreshable, IDataExposable where T: IDataExposable
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

        public void Expose(DataExposer data)
        {
            data.ExposeDeep(ref FinalValue, nameof(FinalValue));
            data.ExposeDeep(ref BaseValue, nameof(BaseValue));
        }

        public static implicit operator T(Stat<T> s) => s.BaseValue;
    }
}
