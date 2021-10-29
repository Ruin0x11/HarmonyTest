using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core
{
    public interface ILocalizable
    {
        void Localize(LocaleKey key);
    }
}
