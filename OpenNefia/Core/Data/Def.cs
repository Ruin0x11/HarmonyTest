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

        public int? ElonaId { get; internal set; }

        public BaseMod? Mod { get; internal set; }

        public Def(string id)
        {
            this.Id = id;
        }

        public virtual void OnResolveReferences()
        {

        }

        public virtual void OnValidate(List<string> errors)
        {

        }

        public override string ToString() => $"<Def {this.GetType().Name}.{this.Id}>";
    }
}
