using Stanford.NER.Net.Math;
using Stanford.NER.Net.Sequences;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class NonLinearCliquePotentialFunction : ICliquePotentialFunction
    {
        double[][] linearWeights;
        double[][] inputLayerWeights;
        double[][] outputLayerWeights;
        SeqClassifierFlags flags;
        double[] layerOneCache, hiddenLayerCache;

        private static double Sigmoid(double x)
        {
            return 1 / (1 + System.Math.Exp(-x));
        }

        public NonLinearCliquePotentialFunction(double[][] linearWeights, double[][] inputLayerWeights, double[][] outputLayerWeights, SeqClassifierFlags flags)
        {
            this.linearWeights = linearWeights;
            this.inputLayerWeights = inputLayerWeights;
            this.outputLayerWeights = outputLayerWeights;
            this.flags = flags;
        }

        public virtual double[] HiddenLayerOutput(double[][] inputLayerWeights, int[] nodeCliqueFeatures, SeqClassifierFlags aFlag, double[] featureVal)
        {
            int layerOneSize = inputLayerWeights.Length;
            if (layerOneCache == null || layerOneSize != layerOneCache.Length)
                layerOneCache = new double[layerOneSize];
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

                layerOneCache[i] = lOneW;
            }

            if (!aFlag.useHiddenLayer)
                return layerOneCache;
            if (hiddenLayerCache == null || layerOneSize != hiddenLayerCache.Length)
                hiddenLayerCache = new double[layerOneSize];
            for (int i = 0; i < layerOneSize; i++)
            {
                if (aFlag.useSigmoid)
                {
                    hiddenLayerCache[i] = Sigmoid(layerOneCache[i]);
                }
                else
                {
                    hiddenLayerCache[i] = System.Math.Tanh(layerOneCache[i]);
                }
            }

            return hiddenLayerCache;
        }

        public override double ComputeCliquePotential(int cliqueSize, int labelIndex, int[] cliqueFeatures, double[] featureVal)
        {
            double output = 0.0;
            if (cliqueSize > 1)
            {
                for (int m = 0; m < cliqueFeatures.Length; m++)
                {
                    output += linearWeights[cliqueFeatures[m]][labelIndex];
                }
            }
            else
            {
                double[] hiddenLayer = HiddenLayerOutput(inputLayerWeights, cliqueFeatures, flags, featureVal);
                int outputLayerSize = inputLayerWeights.Length / outputLayerWeights[0].Length;
                if (flags.useOutputLayer)
                {
                    double[] outputWs = null;
                    if (flags.tieOutputLayer)
                    {
                        outputWs = outputLayerWeights[0];
                    }
                    else
                    {
                        outputWs = outputLayerWeights[labelIndex];
                    }

                    if (flags.softmaxOutputLayer)
                    {
                        outputWs = ArrayMath.Softmax(outputWs);
                    }

                    for (int i = 0; i < inputLayerWeights.Length; i++)
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
            }

            return output;
        }
    }
}
