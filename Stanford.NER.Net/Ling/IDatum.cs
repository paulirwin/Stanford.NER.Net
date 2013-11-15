using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface IDatum<L, F> : IDatum, IFeaturizable<F>, ILabeled<L>
    {
    }

    public interface IDatum
    {
        // non-generic marker interface
    }
}
