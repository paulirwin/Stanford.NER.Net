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

        public static void Set<T>(this IList<T> list, int index, T value)
        {
            list[index] = value;
        }

        public static void RemoveAll<T>(this IList<T> list, IEnumerable<T> toRemove)
        {
            foreach (var item in toRemove)
            {
                list.Remove(item);
            }
        }

        public static int Size<T>(this ICollection<T> collection)
        {
            return collection.Count;
        }

        public static bool IsEmpty<T>(this IList<T> list)
        {
            return list.Count == 0;
        }
    }
}
