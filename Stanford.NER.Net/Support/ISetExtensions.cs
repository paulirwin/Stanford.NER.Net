using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net
{
    public static class ISetExtensions
    {
        public static void AddAll<T>(this ISet<T> set, IEnumerable<T> other)
        {
            set.UnionWith(other);
        }
    }
}
