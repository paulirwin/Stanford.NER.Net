using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util.Logging
{
    public interface IPrettyLoggable
    {
        void PrettyLog(string description);
    }
}
