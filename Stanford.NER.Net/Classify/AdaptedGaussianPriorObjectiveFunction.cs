using Stanford.NER.Net.Math;
using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public class AdaptedGaussianPriorObjectiveFunction<L, F> : LogConditionalObjectiveFunction<L, F>
    {
        double[] weights;
        protected override void Calculate(double[] x)
        {
            if (useSummedConditionalLikelihood)
            {
                CalculateSCL(x);
            }
            else
            {
                CalculateCL(x);
            }
        }

        private void CalculateSCL(double[] x)
        {
            throw new NotSupportedException();
        }

        private void CalculateCL(double[] x)
        {
            value = 0.0;
            if (derivativeNumerator == null)
            {
                derivativeNumerator = new double[x.Length];
                for (int d = 0; d < data.length; d++)
                {
                    int[] features = data[d];
                    for (int f = 0; f < features.Length; f++)
                    {
                        int i = IndexOf(features[f], labels[d]);
                        if (dataweights == null)
                        {
                            derivativeNumerator[i] -= 1;
                        }
                        else
                        {
                            derivativeNumerator[i] -= dataweights[d];
                        }
                    }
                }
            }

            Copy(derivative, derivativeNumerator);
            double[] sums = new double[numClasses];
            double[] probs = new double[numClasses];
            for (int d = 0; d < data.length; d++)
            {
                int[] features = data[d];
                Arrays.Fill(sums, 0.0);
                for (int c = 0; c < numClasses; c++)
                {
                    for (int f = 0; f < features.Length; f++)
                    {
                        int i = IndexOf(features[f], c);
                        sums[c] += x[i];
                    }
                }

                double total = ArrayMath.LogSum(sums);
                for (int c = 0; c < numClasses; c++)
                {
                    probs[c] = System.Math.Exp(sums[c] - total);
                    if (dataweights != null)
                    {
                        probs[c] *= dataweights[d];
                    }

                    for (int f = 0; f < features.Length; f++)
                    {
                        int i = IndexOf(features[f], c);
                        derivative[i] += probs[c];
                    }
                }

                double dV = sums[labels[d]] - total;
                if (dataweights != null)
                {
                    dV *= dataweights[d];
                }

                value -= dV;
            }

            double[] newX = ArrayMath.PairwiseSubtract(x, weights);
            value += prior.Compute(newX, derivative);
        }

        protected override void Rvfcalculate(double[] x)
        {
            throw new NotSupportedException();
        }

        public AdaptedGaussianPriorObjectiveFunction(GeneralDataset<L, F> dataset, LogPrior prior, double weights)
            : base(dataset, prior)
        {
            this.weights = To1D(weights);
        }

        public virtual double[] To1D(double[][] x2)
        {
            double[] x = new double[numFeatures * numClasses];
            for (int i = 0; i < numFeatures; i++)
            {
                for (int j = 0; j < numClasses; j++)
                {
                    x[IndexOf(i, j)] = x2[i][j];
                }
            }

            return x;
        }
    }
}
