using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net
{
    public static class IDictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict[key];
        }

        public static TValue Put<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            TValue old;

            if (dict.TryGetValue(key, out old))
            {
                dict[key] = value;
                return old;
            }

            dict[key] = value;
            return default(TValue);
        }

        public static void PutAll<TKey, TValue>(this IDictionary<TKey, TValue> dict, IEnumerable<KeyValuePair<TKey, TValue>> kvps)
        {
            foreach (var kvp in kvps)
            {
                dict[kvp.Key] = kvp.Value;
            }
        }

        public static bool IsEmpty<TKey, TValue>(this IDictionary<TKey, TValue> dict)
        {
            return dict.Count == 0;
        }
    }
}
