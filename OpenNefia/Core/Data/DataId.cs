using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data
{
    public struct DataId<T> where T : IDataType
    {
        public DataId(string modId, string id) {
            this.Id = $"{modId}.{id}";
        }

        public string Id { get; private set; }

        public override int GetHashCode() => Id.GetHashCode();
    }
}
