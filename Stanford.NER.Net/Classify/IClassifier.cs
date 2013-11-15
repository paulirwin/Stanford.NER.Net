using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public interface IClassifier<L, F>
    {
        L ClassOf(IDatum<L, F> example);
        ICounter<L> ScoresOf(IDatum<L, F> example);
        ICollection<L> Labels();
    }
}
