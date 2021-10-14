using OpenNefia.Core.Data.Serial;
using OpenNefia.Game;
using OpenNefia.Mod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data.Serial
{
    internal static class DefLoader
    {
        private static readonly Dictionary<Type, Dictionary<string, Def>> AllDefs = new Dictionary<Type, Dictionary<string, Def>>();
        private static readonly List<DefCrossRef> PendingCrossRefs = new List<DefCrossRef>();

        private static Type? GetDirectDefType(Type type)
        {
            if (type.BaseType == typeof(Def))
            {
                return type;
            }
            else if (type.BaseType == null || type.BaseType == typeof(object))
            {
                return null;
            }
            return GetDirectDefType(type.BaseType!);
        }

        internal static void Load(string filepath, ModInfo mod, DefDeserializer deserializer)
        {
            var defSet = new DefSet(filepath, mod, deserializer);
            foreach (var def in defSet.Defs)
            {
                var ty = GetDirectDefType(def.GetType());
                if (ty == null)
                {
                    throw new Exception($"Type {def.GetType()} is not a descendent of type that inherits from Def");
                }
                AllDefs[ty].Add(def.Id, def);
            }

            PendingCrossRefs.AddRange(defSet.CrossRefs);
        }

        private static Def? GetDef(Type defType, string defId)
        {
            var args = new object[] { defId };
            var store = typeof(DefStore<>)!.MakeGenericType(defType)!;

            var contains = store.GetMethod("ContainsDefId")!;
            if (!(bool)contains.Invoke(null, args)!)
                return null;

            var get = store.GetMethod("Get")!;
            return (Def?)get.Invoke(null, args);
        }

        private static void ResolveCrossRefs()
        {
            Logger.Info($"[DefLoader] ResolveCrossRefs");

            List<string> errors = new List<string>();

            foreach (var crossRef in PendingCrossRefs)
            {
                var defType = GetDirectDefType(crossRef.crossRefType);
                if (defType == null)
                {
                    errors.Add($"Type {crossRef.crossRefType} is not a descendent of type that inherits from Def");
                }
                else
                {
                    var def = GetDef(crossRef.crossRefType, crossRef.crossRefId);
                    if (def == null)
                    {
                        errors.Add($"{crossRef.target}: Could not find def crossreference '{crossRef.crossRefType}.{crossRef.crossRefId}'");
                    }
                    else
                    {
                        crossRef.targetProperty.SetValue(crossRef.target, def);
                    }
                }
            }

            foreach (var (defType, defs) in AllDefs)
            {
                foreach (var (defId, def) in defs)
                {
                    def.OnResolveReferences();
                    def.OnValidate(errors);
                }
            }

            CheckErrors(errors, $"Errors resolving crossreferences between defs");
        }

        internal static void LoadAll()
        {
            Logger.Info($"[DefLoader] Loading all defs...");

            DefTypes.ScanAllTypes();

            foreach (var ty in DefTypes.AllDefTypes)
            {
                AllDefs[ty] = new Dictionary<string, Def>();
            }

            var deserializer = new DefDeserializer();

            foreach (var modInfo in GameWrapper.Instance.ModLoader.LoadedMods)
            {
                var path = new ModLocalPath(modInfo, "Defs");
                var resolved = path.Resolve();
                if (Directory.Exists(resolved))
                {
                    foreach (var defSetFile in Directory.EnumerateFiles(resolved, "*.xml"))
                    {
                        Load(defSetFile, modInfo, deserializer);
                    }
                }
            }

            CheckErrors(deserializer.Errors, "Errors loading defs");

            AddDefs();
            ResolveCrossRefs();

            AllDefs.Clear();
            PendingCrossRefs.Clear();

            Logger.Info($"[DefLoader] Finished loading.");
        }

        private static void AddDefs()
        {
            Logger.Info($"[DefLoader] AddDefs");

            List<string> errors = new List<string>();

            foreach (var (defType, defs) in AllDefs)
            {
                var store = typeof(DefStore<>)!.MakeGenericType(defType)!;
                foreach (var (defId, def) in defs)
                {
                    var args = new object[] { def };
                    var contains = store.GetMethod("ContainsDefId")!;
                    if ((bool)contains.Invoke(null, new object[] { def.Id })!)
                    {
                        errors.Add($"Def with same ID already exists: {def}");
                    }
                    else
                    {
                        var add = store.GetMethod("AddDef", BindingFlags.NonPublic | BindingFlags.Static)!;
                        add.Invoke(store, new object[] { def });
                    }
                }
            }

            CheckErrors(errors, "Errors adding defs");
        }

        private static bool IsEntriesType(Type arg)
        {
            return arg.GetCustomAttribute<DefOfEntriesAttribute>() != null;
        }

        private static ModInfo? FindContainingMod(Assembly assembly, Type ty)
        {
            var attrib = ty.GetCustomAttribute<DefOfEntriesAttribute>()!;
            var containingModType = attrib.ContainingMod;
            if (containingModType != null)
                return GameWrapper.Instance.ModLoader.GetModFromType(containingModType);
            return GameWrapper.Instance.ModLoader.GetModFromAssembly(assembly);
        }

        internal static void PopulateStaticEntries()
        {
            Logger.Info($"[DefLoader] Populating DefOfEntries classes.");

            List<string> errors = new List<string>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var ty in assembly.GetTypes().Where(IsEntriesType))
                {
                    var containingMod = FindContainingMod(assembly, ty);
                    if (containingMod == null)
                    {
                        errors.Add($"Cannot determine containing mod for def entries class {ty}");
                    }
                    else
                    {
                        foreach (var field in ty.GetFields(BindingFlags.Static | BindingFlags.Public))
                        {
                            var defType = GetDirectDefType(field.FieldType);
                            if (defType == null)
                            {
                                errors.Add($"Type {defType} is not a descendent of type that inherits from Def");
                            }
                            else
                            {
                                var defId = $"{containingMod.Metadata.Name}.{field.Name}";

                                var def = GetDef(defType, defId);
                                if (def == null)
                                {
                                    errors.Add($"{ty.FullName}: Could not find def '{defType.Name}.{defId}'");
                                }
                                else
                                {
                                    field.SetValue(null, def);
                                }
                            }
                        }
                    }

                }
            }

            CheckErrors(errors, "Errors initializing DefEntriesOf classes");
        }

        private static void CheckErrors(List<string> errors, string message)
        {
            if (errors.Count > 0)
            {
                var i = 0;
                var errorMessage = "";
                foreach (var error in errors)
                {
                    errorMessage += $"{error}\n";
                    i++;
                    if (i > 10)
                    {
                        errorMessage += "...";
                        break;
                    }
                }
                throw new Exception($"{message}:\n{errorMessage}");
            }
        }
    }
}
