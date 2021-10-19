using OpenNefia.Core.Data.Serial;
using OpenNefia.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Data
{
    public static class DefTypes
    {
        private static Dictionary<string, Type> Storage = new Dictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        public static IEnumerable<Type> AllDefTypes { get => Storage.Values; }

        public static Type? GetDefTypeFromName(string name)
        {
            return Storage.TryGetValue(name, out var type) ? type : null;
        }

        internal static void ScanAllTypes()
        {
            Storage.Clear();

            var errors = new List<string>();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var containingMod = Engine.ModLoader.GetModFromAssembly(assembly);

                if (containingMod != null)
                {
                    var defTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Def)));

                    foreach (var ty in defTypes)
                    {
                        var fullTypeName = $"{containingMod.Metadata.Name}.{ty.Name}";
                        
                        Storage[fullTypeName] = ty;

                        // Allow omitting the namespace if this Def comes from the Core mod
                        // (<Core.AssetDef/> -> <AssetDef/>)
                        // TODO: Maybe allow this for external mods, too.
                        if (containingMod.Instance?.GetType() == typeof(CoreMod))
                        {
                            Storage[ty.Name] = ty;
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
                throw new Exception($"Errors validating def types:\n{errorMessage}");
            }

            Console.WriteLine($"Loaded {Storage.Count} def types.");
        }

        private static bool HasDefSerialAttribute(PropertyInfo property)
        {
            return property.GetCustomAttribute<DefRequiredAttribute>() != null
                    || property.GetCustomAttribute<DefUseAttributesAttribute>() != null
                    || property.GetCustomAttribute<DefSerialNameAttribute>() != null;
        }
    }
}
