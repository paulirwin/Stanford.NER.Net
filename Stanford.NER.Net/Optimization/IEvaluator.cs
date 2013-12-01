using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Optimization
{
    public interface IEvaluator
    {
        double Evaluate(double[] x);
    }
}
