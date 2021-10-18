using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public class UiResult<T> where T: class
    {
        public enum ResultType
        {
            Continuing,
            Finished,
            Cancelled,
            Error
        }

        public bool IsSuccess { get => this.Type == ResultType.Finished; }

        public T? Value;
        public UiResult<T>.ResultType Type;

        public UiResult(UiResult<T>.ResultType type, T? result)
        {
            this.Value = result;
            this.Type = type;
        }

        public static UiResult<T> Finished(T result) => new UiResult<T>(ResultType.Finished, result);
        public static UiResult<T> Cancelled() => new UiResult<T>(ResultType.Cancelled, null);

        public override string ToString() => $"{this.Type}({this.Value?.ToString()})";    
    }
}
