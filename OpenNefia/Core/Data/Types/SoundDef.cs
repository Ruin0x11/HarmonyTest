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
        public IResourcePath Filepath { get; }

        public SoundDef(string id, IResourcePath filepath) : base(id) { this.Filepath = filepath; }

        public static class Entries
        {
            public static SoundDef Pop3 = new SoundDef($"Core.{nameof(Pop3)}", new ModLocalPath(typeof(CoreMod), "Graphic/pop3.wav"));
        }
    }
}
