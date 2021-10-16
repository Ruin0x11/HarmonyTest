using OpenNefia.Serial;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Serial
{
    public class Ref<T> : IDataExposable where T: Def
    {
        private T _Val;
        public T Val { get => _Val; }
        public int UpdatedCount { get; private set; }

        public Ref(T real) 
        { 
            _Val = real;
            UpdatedCount = 0;
        }

        public void SetReference(T real) 
        { 
            _Val = real;
            UpdatedCount++;
        }

        public void Expose(DataExposer data)
        {
            data.ExposeDef(ref _Val!, nameof(Val));
        }

        public static implicit operator T(Ref<T> r) => r.Val;
        public static implicit operator Ref<T>(T r) => new Ref<T>(r);
    }
}
