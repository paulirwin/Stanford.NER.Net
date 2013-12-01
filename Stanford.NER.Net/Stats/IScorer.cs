using Stanford.NER.Net.Classify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Stats
{
    public interface IScorer<L>
    {
        double Score(ProbabilisticClassifier<L, F> classifier, GeneralDataset<L, F> data);
        string GetDescription(int numDigits);
    }
}
