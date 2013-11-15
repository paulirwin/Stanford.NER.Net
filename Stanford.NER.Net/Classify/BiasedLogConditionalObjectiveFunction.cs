using Stanford.NER.Net.Math;
using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public class BiasedLogConditionalObjectiveFunction : AbstractCachingDiffFunction
    {
        public virtual void SetPrior(LogPrior prior)
        {
            this.prior = prior;
        }

        protected LogPrior prior;
        protected int numFeatures = 0;
        protected int numClasses = 0;
        protected int[][] data = null;
        protected int[] labels = null;
        private double[][] confusionMatrix;
        public override int DomainDimension()
        {
            return numFeatures * numClasses;
        }

        virtual int ClassOf(int index)
        {
            return index % numClasses;
        }

        virtual int FeatureOf(int index)
        {
            return index / numClasses;
        }

        protected virtual int IndexOf(int f, int c)
        {
            return f * numClasses + c;
        }

        public virtual double[][] To2D(double[] x)
        {
            double[][] x2 = new double[numFeatures];
            for (int i = 0; i < numFeatures; i++)
            {
                x2[i] = new double[numClasses];
                for (int j = 0; j < numClasses; j++)
                {
                    x2[i][j] = x[IndexOf(i, j)];
                }
            }

            return x2;
        }

        protected override void Calculate(double[] x)
        {
            if (derivative == null)
            {
                derivative = new double[x.Length];
            }
            else
            {
                Arrays.Fill(derivative, 0.0);
            }

            value = 0.0;
            double[] sums = new double[numClasses];
            double[] probs = new double[numClasses];
            double[] weightedProbs = new double[numClasses];
            for (int d = 0; d < data.Length; d++)
            {
                int[] features = data[d];
                int observedLabel = labels[d];
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
                double[] weightedSums = new double[numClasses];
                for (int trueLabel = 0; trueLabel < numClasses; trueLabel++)
                {
                    weightedSums[trueLabel] = System.Math.Log(confusionMatrix[observedLabel][trueLabel]) + sums[trueLabel];
                }

                double weightedTotal = ArrayMath.LogSum(weightedSums);
                for (int c = 0; c < numClasses; c++)
                {
                    probs[c] = System.Math.Exp(sums[c] - total);
                    weightedProbs[c] = System.Math.Exp(weightedSums[c] - weightedTotal);
                    for (int f = 0; f < features.Length; f++)
                    {
                        int i = IndexOf(features[f], c);
                        derivative[i] += probs[c] - weightedProbs[c];
                    }
                }

                double tmpValue = 0.0;
                for (int c = 0; c < numClasses; c++)
                {
                    tmpValue += confusionMatrix[observedLabel][c] * System.Math.Exp(sums[c] - total);
                }

                value -= System.Math.Log(tmpValue);
            }

            value += prior.Compute(x, derivative);
        }

        public BiasedLogConditionalObjectiveFunction(GeneralDataset dataset, double[][] confusionMatrix)
            : this(dataset, confusionMatrix, new LogPrior(LogPrior.LogPriorType.QUADRATIC))
        {
        }

        public BiasedLogConditionalObjectiveFunction(GeneralDataset dataset, double[][] confusionMatrix, LogPrior prior)
            : this(dataset.NumFeatures(), dataset.NumClasses(), dataset.GetDataArray(), dataset.GetLabelsArray(), confusionMatrix, prior)
        {
        }

        public BiasedLogConditionalObjectiveFunction(int numFeatures, int numClasses, int[][] data, int[] labels, double[][] confusionMatrix)
            : this(numFeatures, numClasses, data, labels, confusionMatrix, new LogPrior(LogPrior.LogPriorType.QUADRATIC))
        {
        }

        public BiasedLogConditionalObjectiveFunction(int numFeatures, int numClasses, int[][] data, int[] labels, double[][] confusionMatrix, LogPrior prior)
        {
            this.numFeatures = numFeatures;
            this.numClasses = numClasses;
            this.data = data;
            this.labels = labels;
            this.prior = prior;
            this.confusionMatrix = confusionMatrix;
        }
    }
}
