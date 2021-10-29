using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;

namespace OpenNefia.Core
{
    internal class LocalizationEnv : IDisposable
    {
        private Lua Lua;
        internal Dictionary<string, string> Store = new Dictionary<string, string>();

        private LuaTable _FinalizedKeys => (LuaTable)Lua["_FinalizedKeys"];

        public LocalizationEnv()
        {
            Lua = SetupLua();
        }

        public void Clear()
        {
            Lua.Dispose();
            Lua = SetupLua();
            Store.Clear();
        }

        private static Lua SetupLua()
        {
            var lua = new Lua();
            lua.State.Encoding = Encoding.UTF8;
            lua.DoFile("Assets/Core/Lua/LocaleEnv.lua");
            return lua;
        }

        public void LoadAll(string language)
        {
            var opts = new EnumerationOptions() { RecurseSubdirectories = true };
            foreach (var file in Directory.EnumerateFiles($"Assets/Elona/Locale/{language}", "*.lua", opts))
            {
                Lua.DoFile(file);
            }
            Lua.DoString("_Finalize()");
            foreach (KeyValuePair<object, object> pair in _FinalizedKeys)
            {
                var key = pair.Key;
                var value = pair.Value;
                Store[key.ToString()!] = value!.ToString()!;
            }
        }

        public void Dispose()
        {
            this.Lua.Dispose();
        }
    }
}
