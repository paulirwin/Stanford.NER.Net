using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net
{
    public static class ListExtensions
    {
        public static T Get<T>(this IList<T> list, int index)
        {
            return list[index];
        }

        public static int Size<T>(this ICollection<T> collection)
        {
            return collection.Count;
        }
    }
}
