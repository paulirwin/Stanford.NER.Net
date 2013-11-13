using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public interface ICliquePotentialFunction
    {
        double ComputeCliquePotential(int cliqueSize, int labelIndex, int[] cliqueFeatures, double[] featureVal);
    }
}
