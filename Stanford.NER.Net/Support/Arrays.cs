using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public static class Arrays
    {
        public static void Fill<T>(this T[] arr, T value)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = value;
            }
        }

        public static void Fill<T>(this T[] arr, int fromIndexInclusive, int toIndexExclusive, T value)
        {
            if (fromIndexInclusive > toIndexExclusive)
                throw new ArgumentException("fromIndex > toIndex");
            if (fromIndexInclusive < 0 || toIndexExclusive > arr.Length)
                throw new ArgumentOutOfRangeException();

            for (int i = fromIndexInclusive; i < toIndexExclusive; i++)
            {
                arr[i] = value;
            }
        }

        public static string ToString<T>(this T[] arr)
        {
            return string.Join(",", arr);
        }
    }
}
