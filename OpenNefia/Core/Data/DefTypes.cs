using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var defTypes = assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(Def)));

                foreach (var ty in defTypes)
                {
                    Console.WriteLine($"Load def type: {ty.FullName}");
                    Storage[ty.Name] = ty;
                }
            }
        }
    }
}
