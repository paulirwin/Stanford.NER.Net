using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public interface IFunction<T1, T2>
    {
        T2 Apply(T1 in_renamed);
    }
}
