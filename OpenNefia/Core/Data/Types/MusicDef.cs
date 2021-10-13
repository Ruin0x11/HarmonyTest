using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class MusicDef : Def
    {
        public IResourcePath Filepath;

        public MusicDef(string id) : base(id)
        {
            Filepath = new ModLocalPath(typeof(CoreMod), String.Empty);
        }
    }
}
