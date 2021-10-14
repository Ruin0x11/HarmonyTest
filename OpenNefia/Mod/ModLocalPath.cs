using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Mod
{
    public class ModLocalPath : IResourcePath
    {
        ModInfo ModInfo;
        string Subpath;

        public Type ModType { get => this.ModInfo.Instance!.GetType(); }

        public ModLocalPath(Type modType, string subpath)
        {
            var location = modType.Assembly.Location;
            var modInfo = GameWrapper.Instance.ModLoader.GetModFromAssemblyLocation(location);
            if (modInfo == null)
            {
                throw new ArgumentException($"Mod of type {modType} is not loaded.");
            }
            ModInfo = modInfo;
            Subpath = subpath;
        }

        public ModLocalPath(BaseMod mod, string subpath) : this(mod.GetType(), subpath) { }
        public ModLocalPath(ModInfo mod, string subpath) : this((BaseMod)mod.Instance!, subpath) { }

        public string Resolve()
        {
            return Path.Combine(ModInfo.WorkingDirectory, Subpath).Replace("/", "\\");
        }

        public ModLocalPath Join(string other)
        {
            return new ModLocalPath(this.ModType, this.Subpath + "/" + other);
        }
    }
}
