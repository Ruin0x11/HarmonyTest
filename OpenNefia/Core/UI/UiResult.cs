using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.UI
{
    public abstract record UiResult<T> where T: class
    {
        public bool HasValue { get => this is Finished; }

        public T Value
        {
            get
            {
                if (this is Finished)
                {
                    return (this as Finished)!.InnerValue;
                }
                else
                {
                    throw new Exception($"Tried to unwrap Value on non-resultful UiResult: {this}");
                }
            }
        }

        public sealed record Finished(T InnerValue) : UiResult<T>;
        public sealed record Cancelled() : UiResult<T>;
        public sealed record Error(Exception Exception) : UiResult<T>;

        public override string ToString()
        {
            switch (this)
            {
                case UiResult<T>.Finished success:
                    return $"Finished({success.InnerValue})";
                case UiResult<T>.Cancelled:
                    return $"Cancelled()";
                case UiResult<T>.Error error:
                    return $"Error({error.Exception.Message})";
                default:
                    return $"Unknown()";
            }
        }
    }
}
