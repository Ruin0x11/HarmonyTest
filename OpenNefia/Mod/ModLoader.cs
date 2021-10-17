using Mono.Cecil;
using OpenNefia.Core;
using OpenNefia.Core.Data;
using OpenNefia.Core.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OpenNefia.Mod
{
    public class ModLoader
    {
        protected static readonly string CurrentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name!;

        protected static readonly Version CurrentAssemblyVersion = Assembly.GetExecutingAssembly().GetName().Version!;
        private static Regex allowedGuidRegex { get; } = new(@"^[a-zA-Z0-9\._\-]+$");

        public DefaultAssemblyResolver CecilResolver { get; }

        HarmonyLib.Harmony HarmonyInstance;

        public ReaderParameters AssemblyReaderParams { get; }
        public List<ModInfo> LoadedMods { get; }
        public Dictionary<string, Assembly> LoadedAssemblies { get; }

        const string MOD_PATH = "Mods";

        public ModLoader()
        {
            CecilResolver = new DefaultAssemblyResolver();
            HarmonyInstance = new HarmonyLib.Harmony("xyz.ruin.harmony");
            AssemblyReaderParams = new ReaderParameters { AssemblyResolver = CecilResolver };
            LoadedMods = new List<ModInfo>();
            LoadedAssemblies = new Dictionary<String, Assembly>();
        }

        public ModInfo? GetModFromAssemblyLocation(string location)
        {
            return LoadedMods.Find(mod => mod.AssemblyLocation == location);
        }

        public ModInfo? GetModFromAssembly(Assembly assembly)
        {
            if (assembly.IsDynamic)
            {
                // Can't invoke assembly.Location on dynamic assemblies.
                return null;
            }

            return LoadedMods.Find(mod => mod.AssemblyLocation == assembly.Location);
        }

        public ModInfo? GetModFromType(Type modType)
        {
            return LoadedMods.Find(mod => mod.Instance?.GetType() == modType);
        }

        public void Execute()
        {
            HarmonyLib.Harmony.DEBUG = true;

            var path = AppDomain.CurrentDomain.BaseDirectory;
            var modsDirectory = Path.GetFullPath(Path.Combine(path, MOD_PATH));
            var scannedMods = new List<ModInfo>();

            if (!Directory.Exists(modsDirectory))
                Directory.CreateDirectory(modsDirectory);

            Action<string> ScanDllAtPath = (dllPath) =>
            {
                using var stream = new MemoryStream(File.ReadAllBytes(dllPath));
                using var assembly = AssemblyDefinition.ReadAssembly(stream, AssemblyReaderParams);

                if (HasMod(assembly))
                {
                    var modInfos = assembly.MainModule.Types.Select(t => GetModInfo(t, dllPath)).WhereNotNull().ToList();
                    scannedMods.AddRange(modInfos);
                }
            };

            foreach (var dir in Directory.GetFiles(modsDirectory, "*.dll", SearchOption.AllDirectories))
            {
                ScanDllAtPath(dir);
            }

            // The Core mod is special since it's a part of our own assembly, not one loaded by Cecil.
            // Trying to load it with Cecil ends up creating two separate instances of every type in the assembly, and they won't be equivalent to one another.
            var coreModInfo = LoadCoreMod();
            LoadedMods.Add(coreModInfo);

            foreach (var mod in scannedMods)
            {
                if (!LoadedAssemblies.TryGetValue(mod.AssemblyLocation, out var ass))
                    LoadedAssemblies[mod.AssemblyLocation] = ass = Assembly.LoadFile(mod.AssemblyLocation);

                var inst = LoadMod(mod, ass);
                mod.Instance = inst;

                Logger.Info($"Running patches in {mod.Metadata.Name}.");
                HarmonyInstance.PatchAll(LoadedAssemblies[mod.AssemblyLocation]);

                LoadedMods.Add(mod);
            }
        }

        private static ModInfo LoadCoreMod()
        {
            var modInfo = GetModInfo(typeof(CoreMod), Assembly.GetExecutingAssembly().Location);

            var core = new CoreMod();
            core.Load();

            modInfo!.Instance = core;
            return modInfo;
        }

        private static BaseMod LoadMod(ModInfo info, Assembly assembly)
        {
            var type = assembly.GetType(info.AssemblyTypeName)!;

            var modInstance = (BaseMod) Activator.CreateInstance(type)!;

            modInstance.Load();

            return modInstance;
        }

        private static bool HasMod(AssemblyDefinition assembly)
        {
            if (assembly.MainModule.AssemblyReferences.All(r => r.Name != CurrentAssemblyName))
                return false;
            if (assembly.MainModule.GetTypeReferences().All(r => r.FullName != typeof(ModEntry).FullName))
                return false;

            return true;
        }

        private static ModInfo? GetModInfo(TypeDefinition type, string assemblyLocation)
        {
            if (type.IsInterface || type.IsAbstract)
                return null;

            try
            {
                if (!type.IsSubtypeOf(typeof(BaseMod)))
                    return null;
            }
            catch (AssemblyResolutionException)
            {
                // Can happen if this type inherits a type from an assembly that can't be found. Safe to assume it's not a plugin.
                return null;
            }

            var metadata = ModEntry.FromCecilType(type);

            // Perform checks that will prevent the plugin from being loaded in ALL cases
            if (metadata == null)
            {
                Console.WriteLine($"Skipping over type [{type.FullName}] as no metadata attribute is specified");
                return null;
            }

            if (string.IsNullOrEmpty(metadata.Guid) || !allowedGuidRegex.IsMatch(metadata.Guid))
            {
                Console.WriteLine($"Skipping type [{type.FullName}] because its GUID [{metadata.Guid}] is of an illegal format.");
                return null;
            }

            if (metadata.Version == null)
            {
                Console.WriteLine($"Skipping type [{type.FullName}] because its version is invalid.");
                return null;
            }

            if (metadata.Name == null)
            {
                Console.WriteLine($"Skipping type [{type.FullName}] because its name is null.");
                return null;
            }

            var coreVersion =
                type.Module.AssemblyReferences.FirstOrDefault(reference => reference.Name == CurrentAssemblyName)?.Version ??
                new Version();

            return new ModInfo(type.FullName, metadata, coreVersion, assemblyLocation, null);
        }

        private static ModInfo? GetModInfo(Type type, string assemblyLocation)
        {
            if (type.IsInterface || type.IsAbstract)
                return null;

            try
            {
                if (!type.IsSubclassOf(typeof(BaseMod)))
                    return null;
            }
            catch (AssemblyResolutionException)
            {
                // Can happen if this type inherits a type from an assembly that can't be found. Safe to assume it's not a plugin.
                return null;
            }

            var metadata = ModEntry.FromDotNetType(type);

            // Perform checks that will prevent the plugin from being loaded in ALL cases
            if (metadata == null)
            {
                Console.WriteLine($"Skipping over type [{type.FullName}] as no metadata attribute is specified");
                return null;
            }

            if (string.IsNullOrEmpty(metadata.Guid) || !allowedGuidRegex.IsMatch(metadata.Guid))
            {
                Console.WriteLine($"Skipping type [{type.FullName}] because its GUID [{metadata.Guid}] is of an illegal format.");
                return null;
            }

            if (metadata.Version == null)
            {
                Console.WriteLine($"Skipping type [{type.FullName}] because its version is invalid.");
                return null;
            }

            if (metadata.Name == null)
            {
                Console.WriteLine($"Skipping type [{type.FullName}] because its name is null.");
                return null;
            }

            return new ModInfo(type.FullName!, metadata, CurrentAssemblyVersion, assemblyLocation, null);
        }
    }
}
