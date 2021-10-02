using Mono.Cecil;
using OpenNefia.Core;
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

        public ModInfo? GetModFromAssemblyLocation(string location)
        {
            return LoadedMods.Find(mod => mod.Location == location);
        }

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

        public void Execute()
        {
            var path = Directory.GetCurrentDirectory();
            var modsDirectory = Path.GetFullPath(Path.Combine(path, MOD_PATH));

            if (!Directory.Exists(modsDirectory))
                Directory.CreateDirectory(modsDirectory);

            foreach (var dll in Directory.GetFiles(modsDirectory, "*.dll", SearchOption.AllDirectories))
            {
                using var stream = new MemoryStream(File.ReadAllBytes(dll));
                using var assembly = AssemblyDefinition.ReadAssembly(stream, AssemblyReaderParams);

                if (HasMod(assembly))
                {
                    var modInfos = assembly.MainModule.Types.Select(t => GetModInfo(t, dll)).WhereNonNull().ToList();
                    LoadedMods.AddRange(modInfos);
                }
            }

            foreach (var mod in LoadedMods)
            {
                if (!LoadedAssemblies.TryGetValue(mod.Location, out var ass))
                    LoadedAssemblies[mod.Location] = ass = Assembly.LoadFile(mod.Location);

                var inst = LoadMod(mod, ass);
                mod.Instance = inst;
            }

            HarmonyInstance.PatchAll();
        }

        private static BaseMod LoadMod(ModInfo info, Assembly assembly)
        {
            var type = assembly.GetType(info.TypeName)!;

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

        private ModInfo? GetModInfo(TypeDefinition type, string assemblyLocation)
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
                type.Module.AssemblyReferences.FirstOrDefault(reference => reference.Name == "HarmonyTest")?.Version ??
                new Version();

            return new ModInfo
            {
                Metadata = metadata,
                TypeName = type.FullName,
                TargetedCoreVersion = coreVersion,
                Location = assemblyLocation
            };
        }
    }
}
