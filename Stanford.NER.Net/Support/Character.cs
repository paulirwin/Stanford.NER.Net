using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public static class Character
    {
        public static char ForDigit(int digit, int radix)
        {
            if (digit < 0)
                throw new InvalidOperationException("non-negative value");

            if (digit < 10)
                return (char)(((int)'0') + digit);

            return (char)(((int)'a') + (digit - 10));
        }
    }
}
