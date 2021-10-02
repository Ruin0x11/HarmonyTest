using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenNefia.Mod
{
    public static class CecilExtensions
    {
        /// <summary>
        ///     Checks whether a given cecil type definition is a subtype of a provided type.
        /// </summary>
        /// <param name="self">Cecil type definition</param>
        /// <param name="td">Type to check against</param>
        /// <returns>Whether the given cecil type is a subtype of the type.</returns>
        public static bool IsSubtypeOf(this TypeDefinition self, Type td)
        {
            if (self.FullName == td.FullName)
                return true;
            return self.FullName != "System.Object" && (self.BaseType?.Resolve()?.IsSubtypeOf(td) ?? false);
        }
    }
}
