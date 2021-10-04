using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data
{
    public class DefStore
    {
        private Dictionary<Type, Dictionary<string, Def>> AllDefs;
        private Dictionary<string, Type> AllDefTypes;

        public DefStore()
        {
            this.AllDefs = new Dictionary<Type, Dictionary<string, Def>>();
            this.AllDefTypes = new Dictionary<string, Type>();
        }

        public void Load(string filepath)
        {
            var defSet = new DefSet(filepath);
            foreach (var def in defSet.Defs)
            {
                var ty = def.GetType();
                this.AllDefTypes[ty.Name] = ty;
                this.AllDefs[ty]!.Add(def.Id, def);
            }
        }

        public Type? GetDefTypeFromName(string name)
        {
            return AllDefTypes.TryGetValue(name, out var type) ? type : null;
        }

        private void ScanTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var defTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Def)));

                foreach (var ty in defTypes)
                {
                    Console.WriteLine($"Load def type: {ty.FullName}");
                    this.AllDefs[ty] = new Dictionary<string, Def>();
                }
            }
        }

        public void LoadAll()
        {
            ScanTypes();

            foreach (var modInfo in GameWrapper.Instance.ModLoader.LoadedMods)
            {
                var path = new ModLocalPath(modInfo.Instance!.GetType(), "Defs");
                var resolved = path.Resolve();
                if (Directory.Exists(resolved))
                {
                    foreach (var defSetFile in Directory.EnumerateFiles(resolved, "*.xml"))
                    {
                        this.Load(defSetFile);
                    }
                }
            }
        }
    }
}
