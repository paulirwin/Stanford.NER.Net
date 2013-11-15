using Stanford.NER.Net.Ling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public interface IClassifierFactory<L, F, C>
    {
        C TrainClassifier(IList<RVFDatum<L, F>> examples);
        C TrainClassifier(GeneralDataset<L, F> dataset);
    }
}
