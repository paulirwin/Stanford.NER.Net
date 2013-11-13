using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net
{
    public static class StringExtensions
    {
        public static bool EqualsIgnoreCase(this string value, string other)
        {
            return string.Equals(value, other, StringComparison.OrdinalIgnoreCase);
        }
    }
}
