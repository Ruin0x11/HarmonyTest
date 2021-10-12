using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data
{
    public class Def
    {
        public string Id { get; }

        public BaseMod? Mod { get; private set; }

        public Def(string id)
        {
            this.Id = id;
        }

        public override string ToString() => $"<Def {this.GetType().Name}.{this.Id}>";
    }
}
