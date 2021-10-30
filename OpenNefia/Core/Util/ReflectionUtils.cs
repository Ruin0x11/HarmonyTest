using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Core.Util
{
    internal static class ReflectionUtils
    {
        public static IEnumerable<Type> AllTypes
        {
            get 
            {
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var type in assembly.GetTypes())
                    {
                        yield return type;
                    }
                }
            }
        }

        public static bool IsStatic(this Type type) => type.IsAbstract && type.IsSealed;

        internal static object? CreateFromPublicOrPrivateCtor(Type type, object[]? ctorParams = null)
        {
            return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ctorParams, null)!;
        }
    }
}
