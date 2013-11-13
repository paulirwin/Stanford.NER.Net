using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Stats
{
    public interface ISampler<T>
    {
        T DrawSample();
    }
}
