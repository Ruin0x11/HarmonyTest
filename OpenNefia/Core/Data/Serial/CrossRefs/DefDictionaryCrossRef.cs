using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace OpenNefia.Core.Data.Serial.CrossRefs
{
    internal class DefDictionaryCrossRef<K, V> : IDefCrossRef where K : notnull
    {
        private Dictionary<K, V> Target;
        private List<object> TargetKeys;
        private List<object> TargetValues;

        public DefDictionaryCrossRef(Dictionary<K, V> target, IList keys, IList values)
        {
            Target = target;
            TargetKeys = keys.Cast<object>().ToList();
            TargetValues = values.Cast<object>().ToList();

            if (keys.Count != values.Count)
            {
                throw new ArgumentException($"Keys and values lists must have same length: {keys.Count} != {values.Count}");
            }

        }

        private static bool KeysAreDefs() => typeof(Def).IsAssignableFrom(typeof(K));
        private static bool ValuesAreDefs() => typeof(Def).IsAssignableFrom(typeof(V));

        public IEnumerable<DefIdentifier> GetWantedCrossRefs()
        {
            if (KeysAreDefs())
            {
                foreach (var key in TargetKeys)
                {
                    if (key is DefIdentifier)
                    {
                        yield return (DefIdentifier)key;
                    }
                    else
                    {
                        throw new Exception($"Expected DefIdentifier, got {key.GetType()}");
                    }
                }
            }
            if (ValuesAreDefs())
            {
                foreach (var value in TargetValues)
                {
                    if (value is DefIdentifier)
                    {
                        yield return (DefIdentifier)value;
                    }
                    else
                    {
                        throw new Exception($"Expected DefIdentifier, got {value.GetType()}");
                    }
                }
            }
        }

        public void Resolve(IEnumerable<Def> defs)
        {
            var defList = defs.ToList();
            List<K> finalKeys = new List<K>();
            List<V> finalValues = new List<V>();

            var ind = 0;
            if (KeysAreDefs())
            {
                for (var i = 0; i < TargetKeys.Count; i++)
                {
                    finalKeys.Add((K)(object)defList[ind]);
                    ind += 1;
                }
            }
            else
            {
                foreach (var key in TargetKeys)
                {
                    finalKeys.Add((K)key);
                }
            }
            if (ValuesAreDefs())
            {
                for (var i = 0; i < TargetValues.Count; i++)
                {
                    finalValues.Add((V)(object)defList[ind]);
                    ind += 1;
                }
            }
            else
            {
                foreach (var value in TargetValues)
                {
                    finalValues.Add((V)value);
                }
            }

            for (var i = 0; i < TargetKeys.Count; i++)
            {
                Target.Add(finalKeys[i], finalValues[i]);
            }
        }
    }
}
