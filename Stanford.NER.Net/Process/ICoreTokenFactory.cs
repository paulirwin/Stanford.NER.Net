using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Process
{
    public interface ICoreTokenFactory<IN>
        where IN : ICoreMap
    {
        IN MakeToken();
        IN MakeToken(String[] keys, String[] values);
        IN MakeToken(IN tokenToBeCopied);
    }
}
