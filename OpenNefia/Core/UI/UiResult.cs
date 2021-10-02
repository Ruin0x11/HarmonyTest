using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public class UiResult<T>
    {
        public enum ResultType
        {
            Continuing,
            Finished,
            Cancelled,
            Error
        }

        public T Result;
        public UiResult<T>.ResultType Type;

        public UiResult(UiResult<T>.ResultType type, T result)
        {
            this.Result = result;
            this.Type = type;
        }

        public static UiResult<T> Finished(T result) => new UiResult<T>(ResultType.Finished, result);
        public static UiResult<T> Cancelled(T result) => new UiResult<T>(ResultType.Cancelled, result);
    }
}
