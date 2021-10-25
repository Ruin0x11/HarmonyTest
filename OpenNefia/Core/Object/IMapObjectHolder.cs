using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Object
{
    public interface IMapObjectHolder
    {
        public IMapObjectHolder? ParentHolder { get; }
        public void GetChildPoolOwners(List<IMapObjectHolder> outOwners);
        public Pool? InnerPool { get; }
    }
}
