using OpenNefia.Core.Data.Serial;
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
                var defTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Def)));

                foreach (var ty in defTypes)
                {
                    Console.WriteLine($"Load def type: {ty.FullName}");
                    
                    Storage[ty.Name] = ty;
                }

                var defSerializableTypes = assembly.GetTypes().Where(x => typeof(IDefSerializable).IsAssignableFrom(x));

                foreach (var ty in defSerializableTypes)
                {
                    foreach (var property in ty.GetProperties())
                    {
                        if (property.GetCustomAttribute<DefRequiredAttribute>() != null || property.GetCustomAttribute<DefUseAttributesAttribute>() != null)
                        {
                            errors.Add($"Def property {property.Name} of type {ty.Name} must be a field instead of a property to be marked with attributes and serialized");
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
        }
    }
}
