using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface IDocument<L, F, T> : IDatum<L, F>, IList<T>
    {
        string Title();
        IDocument<L, F, OUT> BlankDocument<OUT>();
    }
}
