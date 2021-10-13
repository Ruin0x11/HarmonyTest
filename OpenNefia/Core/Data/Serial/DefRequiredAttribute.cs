using System;

namespace OpenNefia.Core.Data.Serial
{
    internal class DefRequiredAttribute : Attribute
    {
        public object? DefaultValue { get; set; }
    }
}