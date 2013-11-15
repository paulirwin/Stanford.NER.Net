using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Stats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public interface IProbabilisticClassifier<L, F> : IClassifier<L, F>
    {
        ICounter<L> ProbabilityOf(IDatum<L, F> example);
        ICounter<L> LogProbabilityOf(IDatum<L, F> example);
    }
}
