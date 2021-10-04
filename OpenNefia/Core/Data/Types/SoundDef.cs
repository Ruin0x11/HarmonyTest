using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class SoundDef : Def
    {
        public string Filepath { get; } = string.Empty;

        public SoundDef(string id) : base(id) { }
    }
}
