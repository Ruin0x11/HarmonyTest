using System;

namespace OpenNefia.Core.Data
{
    public class DefOfEntriesAttribute : Attribute
    {
        public Type? ContainingMod { get; }
    }
}