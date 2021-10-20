using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Types
{
    public class SoundDef : Def
    {
        public IResourcePath Filepath;

        public float Volume = 1f;

        public SoundDef(string id) : base(id) 
        {
            Filepath = new ModLocalPath(typeof(CoreMod), String.Empty);
        }
    }
}
