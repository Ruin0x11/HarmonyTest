using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;

using DotNetCustomAttributeData = System.Reflection.CustomAttributeData;

namespace OpenNefia.Mod
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ModEntry : Attribute
    {
        public ModEntry(string Guid, string Name, string Version)
        {
            this.Guid = Guid;
            this.Name = Name;
            this.Version = TryParseLongVersion(Version)!;
        }

        public string Guid { get; protected set; }
        public string Name { get; protected set; }
        public Version Version { get; protected set; }

        private static Version? TryParseLongVersion(string version)
        {
            if (Version.TryParse(version, out var v))
                return v;

            // no System.Version.TryParse() on .NET 3.5
            try
            {
                var longVersion = new System.Version(version);

                return new Version(longVersion.Major, longVersion.Minor,
                                   longVersion.Build != -1 ? longVersion.Build : 0);
            }
            catch { }

            return null;
        }

        internal static ModEntry? FromCecilType(TypeDefinition td)
        {
            var attr = MetadataHelper.GetCustomAttributes<ModEntry>(td, false).FirstOrDefault();

            if (attr == null)
                return null;

            return new ModEntry((string)attr.ConstructorArguments[0].Value,
                                   (string)attr.ConstructorArguments[1].Value,
                                   (string)attr.ConstructorArguments[2].Value);
        }

        internal static ModEntry? FromDotNetType(Type td)
        {
            var attr = MetadataHelper.GetCustomAttributes<ModEntry>(td, false).FirstOrDefault();

            if (attr == null)
                return null;

            return new ModEntry((string)attr.ConstructorArguments[0].Value!,
                                   (string)attr.ConstructorArguments[1].Value!,
                                   (string)attr.ConstructorArguments[2].Value!);
        }

        public static class MetadataHelper
        {
            internal static IEnumerable<CustomAttribute> GetCustomAttributes<T>(TypeDefinition td, bool inherit)
                where T : Attribute
            {
                var result = new List<CustomAttribute>();
                var type = typeof(T);
                var currentType = td;

                do
                {
                    result.AddRange(currentType!.CustomAttributes.Where(ca => ca.AttributeType.FullName == type.FullName));
                    currentType = currentType.BaseType?.Resolve();
                } while (inherit && currentType?.FullName != "System.Object");


                return result;
            }

            internal static IEnumerable<DotNetCustomAttributeData> GetCustomAttributes<T>(Type type, bool inherit)
                where T : Attribute
            {
                var result = new List<DotNetCustomAttributeData>();
                var attributeType = typeof(T);
                var currentType = type;

                do
                {
                    result.AddRange(currentType!.CustomAttributes.Where(ca => ca.AttributeType.FullName == attributeType.FullName));
                    currentType = currentType.BaseType;
                } while (inherit && currentType?.FullName != "System.Object");


                return result;
            }
        }
    }
}