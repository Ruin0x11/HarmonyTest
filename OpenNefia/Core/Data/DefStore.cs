using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace OpenNefia.Core.Data
{
    public static class DefStore<T> where T: Def
    {
        internal static Dictionary<string, T> AllDefs = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);
        internal static List<T> AllDefsList = new List<T>();

        internal static void AddDef(T def)
        {
            if (def.OriginalXml == null)
            {
                throw new Exception("Tracked defs must have original XML (for theme merging purposes)");
            }

            AllDefs.Add(def.Id, def);
            AllDefsList.Add(def);
        }

        public static bool ContainsDefId(string id) => AllDefs.ContainsKey(id);
        public static T Get(string id) => AllDefs[id];
        public static T? GetOrNull(string id) => AllDefs.GetValueOrDefault(id);

        public static IEnumerable<T> Enumerate() => AllDefsList.AsEnumerable();
    }
}
