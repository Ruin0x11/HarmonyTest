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
        public ModInfo ModInfo { get; }
        public string Subpath { get; }
        private string _path;

        public Type ModType { get => this.ModInfo.Instance!.GetType(); }

        public ModLocalPath(Type modType, string subpath)
        {
            var location = modType.Assembly.Location;
            var modInfo = Engine.ModLoader.GetModFromAssemblyLocation(location);
            if (modInfo == null)
            {
                throw new ArgumentException($"Mod of type {modType} is not loaded.");
            }
            ModInfo = modInfo;
            Subpath = subpath;
            _path = Path.Combine(ModInfo.WorkingDirectory, Subpath).Replace("/", "\\");
        }

        public ModLocalPath(BaseMod mod, string subpath) : this(mod.GetType(), subpath) { }
        public ModLocalPath(ModInfo mod, string subpath) : this((BaseMod)mod.Instance!, subpath) { }

        public string Resolve()
        {
            return _path;
        }

        public ModLocalPath Join(string other)
        {
            return new ModLocalPath(this.ModType, this.Subpath + "/" + other);
        }
    }
}
