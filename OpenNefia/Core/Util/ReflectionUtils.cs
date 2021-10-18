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
        internal static object? CreateFromPublicOrPrivateCtor(Type type, object[]? ctorParams = null)
        {
            return Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, ctorParams, null)!;
        }

        internal static T CreateFromPublicOrPrivateCtor<T>(object[]? ctorParams = null)
        {
            return (T)CreateFromPublicOrPrivateCtor(typeof(T), ctorParams)!;
        }
    }
}
