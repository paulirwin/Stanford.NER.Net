using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public static class IDictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict[key];
        }

        public static void Put<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
        }
    }
}
