using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Mod
{
    public abstract class BaseMod
    {
        protected BaseMod()
        {
        }

        public abstract void Load();

        public virtual bool Unload() => false;
    }
}
