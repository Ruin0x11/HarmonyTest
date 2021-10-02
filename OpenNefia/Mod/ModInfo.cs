using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Mod
{
    public class ModInfo
    {
        public string TypeName { get; internal set; }
        public object Metadata { get; internal set; }
        public Version TargetedCoreVersion { get; internal set; }
        public string Location { get; internal set; }
        public object Instance { get; set; }

        public string WorkingDirectory { get => Directory.GetParent(Location).FullName; }
    }
}
