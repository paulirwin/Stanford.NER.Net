using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class LinearCliquePotentialFunction : ICliquePotentialFunction
    {
        double[][] weights;

        internal LinearCliquePotentialFunction(double[][] weights)
        {
            this.weights = weights;
        }

        public double ComputeCliquePotential(int cliqueSize, int labelIndex, int[] cliqueFeatures, double[] featureVal)
        {
            double output = 0.0;
            double dotProd = 0;
            for (int m = 0; m < cliqueFeatures.Length; m++)
            {
                dotProd = weights[cliqueFeatures[m]][labelIndex];
                if (featureVal != null)
                    dotProd *= featureVal[m];
                output += dotProd;
            }

            return output;
        }
    }
}
