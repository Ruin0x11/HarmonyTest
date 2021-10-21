using System;

namespace OpenNefia.Core.Data.Serial.Attributes
{
    internal class DefDictionaryFieldNamesAttribute : Attribute
    {
        public const string DEFAULT_ENTRY_NAME = nameof(Entry);
        public const string DEFAULT_KEY_NAME = nameof(Key);
        public const string DEFAULT_VALUE_NAME = nameof(Value);

        public string Entry;
        public string Key;
        public string Value;
        public bool UseAttributes;

        public DefDictionaryFieldNamesAttribute(string Entry, string Key, string Value, bool UseAttributes = false)
        {
            this.Entry = Entry;
            this.Key = Key;
            this.Value = Value;
            this.UseAttributes = UseAttributes;
        }
    }
}