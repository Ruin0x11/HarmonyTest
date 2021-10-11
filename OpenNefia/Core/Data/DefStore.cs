﻿using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data
{
    public static class DefStore<T> where T: Def
    {
        internal static Dictionary<string, T> AllDefs = new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

        public static bool ContainsDefId(string id) => AllDefs.ContainsKey(id);
        public static T Get(string id) => AllDefs[id];

        public static Dictionary<string, T>.Enumerator Enumerate() => AllDefs.GetEnumerator();
    }
}
