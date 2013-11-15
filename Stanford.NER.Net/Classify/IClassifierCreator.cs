using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public interface IClassifierCreator<L, F>
    {
        IClassifier<L, F> CreateClassifier(double[] weights);
    }
}
