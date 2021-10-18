using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Util
{
    public static class PathUtils
    {
        public static string? GetFullPathWithoutExtension(string path)
        {
            var directory = Path.GetDirectoryName(path);
            if (directory == null)
                return null;

            return Path.Combine(directory, Path.GetFileNameWithoutExtension(path));
        }
    }
}
