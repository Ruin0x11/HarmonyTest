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
        public string AssemblyTypeName { get; internal set; }
        public ModEntry Metadata { get; internal set; }
        public Version TargetedCoreVersion { get; internal set; }
        public string AssemblyLocation { get; internal set; }
        public object? Instance { get; set; }

        public string WorkingDirectory { get => Directory.GetParent(AssemblyLocation)!.FullName; }

        public ModInfo(string assemblyTypeName, ModEntry metadata, Version targetedCoreVersion, string location, object? instance)
        {
            AssemblyTypeName = assemblyTypeName;
            Metadata = metadata;
            TargetedCoreVersion = targetedCoreVersion;
            AssemblyLocation = location;
            Instance = instance;
        }
    }
}
