using System;

namespace OpenNefia.Core.Data.Serial
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DefSerialNameAttribute : Attribute
    {
        public string Name { get; set; }

        public DefSerialNameAttribute(string name)
        {
            this.Name = name;
        }
    }
}