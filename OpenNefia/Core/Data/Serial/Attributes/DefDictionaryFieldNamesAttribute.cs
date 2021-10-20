using System;

namespace OpenNefia.Core.Data.Serial.Attributes
{
    internal class DefDictionaryFieldNamesAttribute : Attribute
    {
        public string Entry;
        public string Key;
        public string Value;
        public bool UseAttributes;

        public DefDictionaryFieldNamesAttribute(string entry, string key, string value, bool useAttributes = false)
        {
            Entry = entry;
            Key = key;
            Value = value;
            UseAttributes = useAttributes;
        }
    }
}