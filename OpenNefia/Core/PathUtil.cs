using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia
{
    class PathUtil
    {
        public static string ModLocalPath(string subpath)
        {
            var location = Assembly.GetExecutingAssembly().Location;
            var mod = GameWrapper.Instance.ModLoader.GetModFromAssemblyLocation(location)!;
            return ModLocalPath(mod, subpath);
        }
        public static string ModLocalPath(ModInfo mod, string subpath)
        {
            return Path.Combine(mod.WorkingDirectory, subpath);
        }
    }
}
