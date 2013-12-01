using Stanford.NER.Net.Math;
using Stanford.NER.Net.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class NonLinearSecondOrderCliquePotentialFunction : ICliquePotentialFunction
    {
        double[][] inputLayerWeights4Edge;
        double[][] outputLayerWeights4Edge;
        double[][] inputLayerWeights;
        double[][] outputLayerWeights;
        double[] layerOneCache, hiddenLayerCache;
        double[] layerOneCache4Edge, hiddenLayerCache4Edge;
        SeqClassifierFlags flags;
        public NonLinearSecondOrderCliquePotentialFunction(double[][] inputLayerWeights4Edge, double[][] outputLayerWeights4Edge, double[][] inputLayerWeights, double[][] outputLayerWeights, SeqClassifierFlags flags)
        {
            this.inputLayerWeights4Edge = inputLayerWeights4Edge;
            this.outputLayerWeights4Edge = outputLayerWeights4Edge;
            this.inputLayerWeights = inputLayerWeights;
            this.outputLayerWeights = outputLayerWeights;
            this.flags = flags;
        }

        public virtual double[] HiddenLayerOutput(double[][] inputLayerWeights, int[] nodeCliqueFeatures, SeqClassifierFlags aFlag, double[] featureVal, int cliqueSize)
        {
            double[] layerCache = null;
            double[] hlCache = null;
            int layerOneSize = inputLayerWeights.Length;
            if (cliqueSize > 1)
            {
                if (layerOneCache4Edge == null || layerOneSize != layerOneCache4Edge.Length)
                    layerOneCache4Edge = new double[layerOneSize];
                layerCache = layerOneCache4Edge;
            }
            else
            {
                if (layerOneCache == null || layerOneSize != layerOneCache.Length)
                    layerOneCache = new double[layerOneSize];
                layerCache = layerOneCache;
            }

            for (int i = 0; i < layerOneSize; i++)
            {
                double[] ws = inputLayerWeights[i];
                double lOneW = 0;
                double dotProd = 0;
                for (int m = 0; m < nodeCliqueFeatures.Length; m++)
                {
                    dotProd = ws[nodeCliqueFeatures[m]];
                    if (featureVal != null)
                        dotProd *= featureVal[m];
                    lOneW += dotProd;
                }

                layerCache[i] = lOneW;
            }

            if (!aFlag.useHiddenLayer)
                return layerCache;
            if (cliqueSize > 1)
            {
                if (hiddenLayerCache4Edge == null || layerOneSize != hiddenLayerCache4Edge.Length)
                    hiddenLayerCache4Edge = new double[layerOneSize];
                hlCache = hiddenLayerCache4Edge;
            }
            else
            {
                if (hiddenLayerCache == null || layerOneSize != hiddenLayerCache.Length)
                    hiddenLayerCache = new double[layerOneSize];
                hlCache = hiddenLayerCache;
            }

            for (int i = 0; i < layerOneSize; i++)
            {
                if (aFlag.useSigmoid)
                {
                    hlCache[i] = Sigmoid(layerCache[i]);
                }
                else
                {
                    hlCache[i] = System.Math.Tanh(layerCache[i]);
                }
            }

            return hlCache;
        }

        private static double Sigmoid(double x)
        {
            return 1 / (1 + System.Math.Exp(-x));
        }

        public override double ComputeCliquePotential(int cliqueSize, int labelIndex, int[] cliqueFeatures, double[] featureVal)
        {
            double output = 0.0;
            double[][] inputWeights, outputWeights = null;
            if (cliqueSize > 1)
            {
                inputWeights = inputLayerWeights4Edge;
                outputWeights = outputLayerWeights4Edge;
            }
            else
            {
                inputWeights = inputLayerWeights;
                outputWeights = outputLayerWeights;
            }

            double[] hiddenLayer = HiddenLayerOutput(inputWeights, cliqueFeatures, flags, featureVal, cliqueSize);
            int outputLayerSize = inputWeights.Length / outputWeights[0].Length;
            if (flags.useOutputLayer)
            {
                double[] outputWs = null;
                if (flags.tieOutputLayer)
                {
                    outputWs = outputWeights[0];
                }
                else
                {
                    outputWs = outputWeights[labelIndex];
                }

                if (flags.softmaxOutputLayer)
                {
                    outputWs = ArrayMath.Softmax(outputWs);
                }

                for (int i = 0; i < inputWeights.Length; i++)
                {
                    if (flags.sparseOutputLayer || flags.tieOutputLayer)
                    {
                        if (i % outputLayerSize == labelIndex)
                        {
                            output += outputWs[i / outputLayerSize] * hiddenLayer[i];
                        }
                    }
                    else
                    {
                        output += outputWs[i] * hiddenLayer[i];
                    }
                }
            }
            else
            {
                output = hiddenLayer[labelIndex];
            }

            return output;
        }
    }
}
