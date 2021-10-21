using FluentResults;
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
        private static Dictionary<string, Type> ShortNameToDefDeserializableType = new Dictionary<string, Type>();
        private static Dictionary<string, List<Type>> AmbiguousShortNameDefDeserTypes = new Dictionary<string, List<Type>>();

        public static IEnumerable<Type> AllDefTypes { get => Storage.Values; }

        public static Type? GetDefTypeFromName(string name)
        {
            return Storage.TryGetValue(name, out var type) ? type : null;
        }

        /// <summary>
        /// Core.Elona122MapLoader -> OpenNefia.Core.Map.Generator.Elona122MapLoader
        /// </summary>
        /// <returns></returns>
        public static Result<Type> FindDefDeserializableTypeFromName(string name, Type baseType)
        {
            Type? type = null;
            if (AmbiguousShortNameDefDeserTypes.TryGetValue(name, out var types))
            {
                var ambiguous = types.Where(x => baseType.IsAssignableFrom(x)).ToList();

                if (ambiguous.Count == 0)
                {
                    type = null;
                }
                else if (ambiguous.Count == 1)
                {
                    type = ambiguous[0];
                }
                else
                {
                    var typeNames = String.Join(", ", ambiguous.Select(ty => ty.FullName));
                    throw new Exception($"Short class name '{name}' is ambiguous between the following types: {typeNames}");
                }
            }

            type = type ?? ShortNameToDefDeserializableType.GetValueOrDefault(name) ?? Type.GetType(name);

            if (type == null)
            {
                return Result.Fail($"Could not find fully qualified or shortened type with name '{name}'");
            }
            else if (!baseType.IsAssignableFrom(type))
            {
                return Result.Fail($"Type '{type}' is not convertable to base type '{baseType}");
            }

            return Result.Ok(type);
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
                    foreach (var ty in assembly.GetTypes())
                    {
                        if (ty.IsSubclassOf(typeof(Def)))
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

                        if (typeof(IDefDeserializable).IsAssignableFrom(ty))
                        {
                            var shortName = $"{containingMod.Metadata.Name}.{ty.Name}";

                            if (ShortNameToDefDeserializableType.ContainsKey(shortName))
                            {
                                if (AmbiguousShortNameDefDeserTypes.TryGetValue(shortName, out var types))
                                {
                                    types.Add(ty);
                                }
                                else
                                {
                                    AmbiguousShortNameDefDeserTypes[shortName] = new List<Type>() { ty };
                                }
                            }
                            else
                            {
                                ShortNameToDefDeserializableType.Add(shortName, ty);
                            }
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
