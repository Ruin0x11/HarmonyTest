using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public interface IPoolOwner
    {
        public IPoolOwner? ParentPoolOwner { get; }
        public void GetChildPoolOwners(List<IPoolOwner> outOwners);
        public Pool InnerPool { get; }
    }
}
