using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net
{
    public static class DateTimeExtensions
    {
        private static readonly DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0);

        public static long CurrentTimeMillis(this DateTime dt)
        {
            return (long)(dt - unixEpoch).TotalMilliseconds;
        }
    }
}
