using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data
{
    internal static class DefLoader
    {
        private static readonly Dictionary<Type, Dictionary<string, Def>> AllDefs = new Dictionary<Type, Dictionary<string, Def>>();

        internal static void Load(string filepath, Type modType)
        {
            var defSet = new DefSet(filepath, modType);
            foreach (var def in defSet.Defs)
            {
                var ty = def.GetType();
                AllDefs[ty].Add(def.Id, def);
            }
        }

        private static object? GetDef(Type defType, string defId)
        {
            var args = new object[] { defId };
            var store = typeof(DefStore<>)!.MakeGenericType(defType)!;

            var contains = store.GetMethod("ContainsDefId")!;
            if (!(bool)contains.Invoke(null, args)!)
                return null;

            var get = store.GetMethod("Get")!;
            return get.Invoke(null, args);
        }

        internal static void LoadAll()
        {
            DefTypes.ScanAllTypes();

            foreach (var ty in DefTypes.AllDefTypes)
            {
                AllDefs[ty] = new Dictionary<string, Def>();
            }

            foreach (var modInfo in GameWrapper.Instance.ModLoader.LoadedMods)
            {
                var modType = modInfo.Instance!.GetType();
                var path = new ModLocalPath(modType, "Defs");
                var resolved = path.Resolve();
                if (Directory.Exists(resolved))
                {
                    foreach (var defSetFile in Directory.EnumerateFiles(resolved, "*.xml"))
                    {
                        Load(defSetFile, modType);
                    }
                }
            }

            foreach (var (defType, defs) in AllDefs)
            {
                var store = typeof(DefStore<>)!.MakeGenericType(defType)!;
                var field = store.GetField("AllDefs", BindingFlags.Static | BindingFlags.NonPublic)!;
                Type newDictType = typeof(Dictionary<,>).MakeGenericType(field.FieldType.GetGenericArguments());
                var newDict = Activator.CreateInstance(newDictType);
                foreach (var (defId, def) in defs)
                {
                    var add = newDictType.GetMethod("Add", newDictType.GetGenericArguments())!;
                    add.Invoke(newDict, new object[] { defId, def });
                }
                field.SetValue(null, newDict);
            }

            AllDefs.Clear();
        }

        private static bool IsEntriesType(Type arg)
        {
            return arg.GetCustomAttribute<DefEntriesOfAttribute>() != null;
        }

        internal static void PopulateStaticEntries()
        {
            List<string> errors = new List<string>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var ty in assembly.GetTypes().Where(IsEntriesType))
                {
                    foreach (var field in ty.GetFields(BindingFlags.Static | BindingFlags.Public))
                    {
                        var defType = field.FieldType;
                        var defId = field.Name;

                        var def = GetDef(defType, defId);
                        if (def == null)
                        {
                            errors.Add($"{ty.FullName}: Could not find def of type '{defType.Name}.{defId}'");
                        }
                        else
                        {
                            field.SetValue(null, def);
                        }
                    }
                }
            }

            if (errors.Count > 0)
            {
                var errorMessage = "";
                foreach (var error in errors)
                {
                    errorMessage += $"{error}\n";
                }
                throw new Exception($"Errors initializing DefEntriesOf classes:\n{errorMessage}");
            }
        }
    }
}
