using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public interface IHeap<E> : IEnumerable<E>
    {
        E ExtractMin();
        E Min();
        bool Add(E o);
        int Size();
        bool IsEmpty();
        int DecreaseKey(E o);
    }
}
