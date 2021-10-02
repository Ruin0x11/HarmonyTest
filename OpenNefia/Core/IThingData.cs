using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    public interface IThingData
    {
        abstract string ID { get; }
        abstract IResourcePath Image { get; }
    }
}
