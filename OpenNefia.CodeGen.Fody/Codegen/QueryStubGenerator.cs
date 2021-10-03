extern alias fodyhelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using fodyhelpers::Fody;
using fodyhelpers::Mono.Cecil;
using fodyhelpers::Mono.Cecil.Rocks;

namespace OpenNefia.CodeGen
{
    public class QueryStubGenerator : BaseModuleWeaver
    {
        public const string attributeName = "EqualsAttribute";
        public const string ignoreDuringEqualsAttributeName = "IgnoreDuringEqualsAttribute";
        public const string customEqualsInternalAttribute = "CustomEqualsInternalAttribute";

        public const string DoNotAddEqualityOperators = "DoNotAddEqualityOperators";
        public const string DoNotAddGetHashCode = "DoNotAddGetHashCode";
        public const string DoNotAddEquals = "DoNotAddEquals";
        public const string IgnoreBaseClassProperties = "IgnoreBaseClassProperties";

        public IEnumerable<TypeDefinition> GetMatchingTypes()
        {
            return ModuleDefinition.GetTypes()
                .Where(x =>
                {
                    return x.IsSubtypeOf("OpenNefia.Core.UI.BaseUiLayer`1");
                });
        }

        TypeReference GetGenericType(TypeReference type)
        {
            if (type.HasGenericParameters)
            {
                var parameters = type.GenericParameters.Select(x => (TypeReference)x).ToArray();
                return type.MakeGenericInstanceType(parameters);
            }

            return type;
        }

        public override void Execute()
        {
            var matchingTypes = GetMatchingTypes().ToArray();
            foreach (var type in matchingTypes)
            {
                Console.WriteLine($"Found type: {type}");
            }
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "mscorlib";
            yield return "System";
            yield return "netstandard";
            yield return "System.Diagnostics.Tools";
            yield return "System.Diagnostics.Debug";
            yield return "System.Runtime";
        }

        public override bool ShouldCleanReference => true;
    }
}
