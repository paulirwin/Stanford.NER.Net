using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public class ConcurrentHashMap<TKey, TValue> : HashMap<TKey, TValue>
    {
        public ConcurrentHashMap()
            : base(new ConcurrentDictionary<TKey, TValue>())
        {

        }
    }
}
