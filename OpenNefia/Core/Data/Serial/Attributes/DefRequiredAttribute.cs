using System;

namespace OpenNefia.Core.Data.Serial
{
    [AttributeUsage(AttributeTargets.Field)]
    internal class DefRequiredAttribute : Attribute
    {
        public object? DefaultValue { get; set; }
    }
}