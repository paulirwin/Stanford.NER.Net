using Stanford.NER.Net.Math;
using Stanford.NER.Net.Sequences;
using Stanford.NER.Net.Stats;
using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class CRFCliqueTree<E> : ISequenceModel, ISequenceListener
    {
        protected readonly FactorTable[] factorTables;
        protected readonly double z;
        protected readonly IIndex<E> classIndex;
        private readonly E backgroundSymbol;
        private readonly int backgroundIndex;
        protected readonly int windowSize;
        private readonly int numClasses;
        private readonly int[] possibleValues;

        public CRFCliqueTree(FactorTable[] factorTables, IIndex<E> classIndex, E backgroundSymbol)
            : this(factorTables, classIndex, backgroundSymbol, factorTables[0].TotalMass())
        {
        }

        CRFCliqueTree(FactorTable[] factorTables, IIndex<E> classIndex, E backgroundSymbol, double z)
        {
            this.factorTables = factorTables;
            this.z = z;
            this.classIndex = classIndex;
            this.backgroundSymbol = backgroundSymbol;
            backgroundIndex = classIndex.IndexOf(backgroundSymbol);
            windowSize = factorTables[0].WindowSize();
            numClasses = classIndex.Size();
            possibleValues = new int[numClasses];
            for (int i = 0; i < numClasses; i++)
            {
                possibleValues[i] = i;
            }
        }

        public virtual FactorTable[] GetFactorTables()
        {
            return this.factorTables;
        }

        public virtual IIndex<E> ClassIndex()
        {
            return classIndex;
        }

        public int Length()
        {
            return factorTables.Length;
        }

        public int LeftWindow()
        {
            return windowSize;
        }

        public int RightWindow()
        {
            return 0;
        }

        public int[] GetPossibleValues(int position)
        {
            return possibleValues;
        }

        public double ScoreOf(int[] sequence, int pos)
        {
            return ScoresOf(sequence, pos)[sequence[pos]];
        }

        public double[] ScoresOf(int[] sequence, int position)
        {
            if (position >= factorTables.Length)
                throw new Exception(@"Index out of bounds: " + position);
            double[] probThisGivenPrev = new double[numClasses];
            double[] probNextGivenThis = new double[numClasses];
            int prevLength = windowSize - 1;
            int[] prev = new int[prevLength + 1];
            int i = 0;
            for (; i < prevLength - position; i++)
            {
                prev[i] = classIndex.IndexOf(backgroundSymbol);
            }

            for (; i < prevLength; i++)
            {
                prev[i] = sequence[position - prevLength + i];
            }

            for (int label = 0; label < numClasses; label++)
            {
                prev[prev.Length - 1] = label;
                probThisGivenPrev[label] = factorTables[position].UnnormalizedLogProb(prev);
            }

            int nextLength = windowSize - 1;
            if (position + nextLength >= Length())
            {
                nextLength = Length() - position - 1;
            }

            FactorTable nextFactorTable = factorTables[position + nextLength];
            if (nextLength != windowSize - 1)
            {
                for (int j = 0; j < windowSize - 1 - nextLength; j++)
                {
                    nextFactorTable = nextFactorTable.SumOutFront();
                }
            }

            if (nextLength == 0)
            {
                Arrays.Fill(probNextGivenThis, 1.0);
            }
            else
            {
                int[] next = new int[nextLength];
                Array.Copy(sequence, position + 1, next, 0, nextLength);
                for (int label = 0; label < numClasses; label++)
                {
                    probNextGivenThis[label] = nextFactorTable.UnnormalizedConditionalLogProbGivenFirst(label, next);
                }
            }

            return ArrayMath.PairwiseAdd(probThisGivenPrev, probNextGivenThis);
        }

        public override double ScoreOf(int[] sequence)
        {
            int[] given = new int[Window() - 1];
            Arrays.Fill(given, classIndex.IndexOf(backgroundSymbol));
            double logProb = 0.0;
            for (int i = 0, length = Length(); i < length; i++)
            {
                int label = sequence[i];
                logProb += CondLogProbGivenPrevious(i, label, given);
                Array.Copy(given, 1, given, 0, given.Length - 1);
                given[given.Length - 1] = label;
            }

            return logProb;
        }

        public virtual int Window()
        {
            return windowSize;
        }

        public virtual int GetNumClasses()
        {
            return numClasses;
        }

        public virtual double TotalMass()
        {
            return z;
        }

        public virtual int BackgroundIndex()
        {
            return backgroundIndex;
        }

        public virtual E BackgroundSymbol()
        {
            return backgroundSymbol;
        }

        public virtual double LogProb(int position, int label)
        {
            double u = factorTables[position].UnnormalizedLogProbEnd(label);
            return u - z;
        }

        public virtual double Prob(int position, int label)
        {
            return System.Math.Exp(LogProb(position, label));
        }

        public virtual double LogProb(int position, E label)
        {
            return LogProb(position, classIndex.IndexOf(label));
        }

        public virtual double Prob(int position, E label)
        {
            return System.Math.Exp(LogProb(position, label));
        }

        public virtual ICounter<E> Probs(int position)
        {
            ICounter<E> c = new ClassicCounter<E>();
            for (int i = 0, sz = classIndex.Size(); i < sz; i++)
            {
                E label = classIndex.Get(i);
                c.IncrementCount(label, Prob(position, i));
            }

            return c;
        }

        public virtual ICounter<E> LogProbs(int position)
        {
            ICounter<E> c = new ClassicCounter<E>();
            for (int i = 0, sz = classIndex.Size(); i < sz; i++)
            {
                E label = classIndex.Get(i);
                c.IncrementCount(label, LogProb(position, i));
            }

            return c;
        }

        public virtual double LogProb(int position, int[] labels)
        {
            if (labels.Length < windowSize)
            {
                return factorTables[position].UnnormalizedLogProbEnd(labels) - z;
            }
            else if (labels.Length == windowSize)
            {
                return factorTables[position].UnnormalizedLogProb(labels) - z;
            }
            else
            {
                int[] l = new int[windowSize];
                Array.Copy(labels, 0, l, 0, l.Length);
                int position1 = position - labels.Length + windowSize;
                double p = factorTables[position1].UnnormalizedLogProb(l) - z;
                l = new int[windowSize - 1];
                Array.Copy(labels, 1, l, 0, l.Length);
                position1++;
                for (int i = windowSize; i < labels.Length; i++)
                {
                    p += CondLogProbGivenPrevious(position1++, labels[i], l);
                    Array.Copy(l, 1, l, 0, l.Length - 1);
                    l[windowSize - 2] = labels[i];
                }

                return p;
            }
        }

        public virtual double Prob(int position, int[] labels)
        {
            return System.Math.Exp(LogProb(position, labels));
        }

        public virtual double LogProb(int position, E[] labels)
        {
            return LogProb(position, ObjectArrayToIntArray(labels));
        }

        public virtual double Prob(int position, E[] labels)
        {
            return System.Math.Exp(LogProb(position, labels));
        }

        public virtual GeneralizedCounter LogProbs(int position, int window)
        {
            GeneralizedCounter<E> gc = new GeneralizedCounter<E>(window);
            int[] labels = new int[window];

            while (true)
            {
                List<E> labelsList = IntArrayToListE(labels);
                gc.IncrementCount(labelsList, LogProb(position, labels));

                bool shouldBreakWhile = false;

                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i]++;
                    if (labels[i] < numClasses)
                    {
                        break;
                    }

                    if (i == labels.Length - 1)
                    {
                        shouldBreakWhile = true;
                        break;
                    }

                    labels[i] = 0;
                }

                if (shouldBreakWhile)
                    break;
            }

            return gc;
        }

        public virtual GeneralizedCounter Probs(int position, int window)
        {
            GeneralizedCounter<E> gc = new GeneralizedCounter<E>(window);
            int[] labels = new int[window];

            while (true)
            {
                List<E> labelsList = IntArrayToListE(labels);
                gc.IncrementCount(labelsList, Prob(position, labels));

                bool shouldBreakWhile = false;

                for (int i = 0; i < labels.Length; i++)
                {
                    labels[i]++;
                    if (labels[i] < numClasses)
                    {
                        break;
                    }

                    if (i == labels.Length - 1)
                    {
                        shouldBreakWhile = true;
                        break;
                    }

                    labels[i] = 0;
                }

                if (shouldBreakWhile)
                    break;
            }

            return gc;
        }

        private int[] ObjectArrayToIntArray(E[] os)
        {
            int[] is2 = new int[os.Length];
            for (int i = 0; i < os.Length; i++)
            {
                is2[i] = classIndex.IndexOf(os[i]);
            }

            return is2;
        }

        private List<E> IntArrayToListE(int[] is_renamed)
        {
            List<E> os = new List<E>(is_renamed.Length);
            foreach (int i in is_renamed)
            {
                os.Add(classIndex.Get(i));
            }

            return os;
        }

        public virtual double CondLogProbGivenPrevious(int position, int label, int[] prevLabels)
        {
            if (prevLabels.Length + 1 == windowSize)
            {
                return factorTables[position].ConditionalLogProbGivenPrevious(prevLabels, label);
            }
            else if (prevLabels.Length + 1 < windowSize)
            {
                FactorTable ft = factorTables[position].SumOutFront();
                while (ft.WindowSize() > prevLabels.Length + 1)
                {
                    ft = ft.SumOutFront();
                }

                return ft.ConditionalLogProbGivenPrevious(prevLabels, label);
            }
            else
            {
                int[] p = new int[windowSize - 1];
                Array.Copy(prevLabels, prevLabels.Length - p.Length, p, 0, p.Length);
                return factorTables[position].ConditionalLogProbGivenPrevious(p, label);
            }
        }

        public virtual double CondLogProbGivenPrevious(int position, E label, E[] prevLabels)
        {
            return CondLogProbGivenPrevious(position, classIndex.IndexOf(label), ObjectArrayToIntArray(prevLabels));
        }

        public virtual double CondProbGivenPrevious(int position, int label, int[] prevLabels)
        {
            return System.Math.Exp(CondLogProbGivenPrevious(position, label, prevLabels));
        }

        public virtual double CondProbGivenPrevious(int position, E label, E[] prevLabels)
        {
            return System.Math.Exp(CondLogProbGivenPrevious(position, label, prevLabels));
        }

        public virtual ICounter<E> CondLogProbsGivenPrevious(int position, int[] prevlabels)
        {
            ICounter<E> c = new ClassicCounter<E>();
            for (int i = 0, sz = classIndex.Size(); i < sz; i++)
            {
                E label = classIndex.Get(i);
                c.IncrementCount(label, CondLogProbGivenPrevious(position, i, prevlabels));
            }

            return c;
        }

        public virtual ICounter<E> CondLogProbsGivenPrevious(int position, E[] prevlabels)
        {
            ICounter<E> c = new ClassicCounter<E>();
            for (int i = 0, sz = classIndex.Size(); i < sz; i++)
            {
                E label = classIndex.Get(i);
                c.IncrementCount(label, CondLogProbGivenPrevious(position, label, prevlabels));
            }

            return c;
        }

        public virtual double CondLogProbGivenNext(int position, int label, int[] nextLabels)
        {
            position = position + nextLabels.Length;
            if (nextLabels.Length + 1 == windowSize)
            {
                return factorTables[position].ConditionalLogProbGivenNext(nextLabels, label);
            }
            else if (nextLabels.Length + 1 < windowSize)
            {
                FactorTable ft = factorTables[position].SumOutFront();
                while (ft.WindowSize() > nextLabels.Length + 1)
                {
                    ft = ft.SumOutFront();
                }

                return ft.ConditionalLogProbGivenPrevious(nextLabels, label);
            }
            else
            {
                int[] p = new int[windowSize - 1];
                Array.Copy(nextLabels, 0, p, 0, p.Length);
                return factorTables[position].ConditionalLogProbGivenPrevious(p, label);
            }
        }

        public virtual double CondLogProbGivenNext(int position, E label, E[] nextLabels)
        {
            return CondLogProbGivenNext(position, classIndex.IndexOf(label), ObjectArrayToIntArray(nextLabels));
        }

        public virtual double CondProbGivenNext(int position, int label, int[] nextLabels)
        {
            return System.Math.Exp(CondLogProbGivenNext(position, label, nextLabels));
        }

        public virtual double CondProbGivenNext(int position, E label, E[] nextLabels)
        {
            return System.Math.Exp(CondLogProbGivenNext(position, label, nextLabels));
        }

        public virtual ICounter<E> CondLogProbsGivenNext(int position, int[] nextlabels)
        {
            ICounter<E> c = new ClassicCounter<E>();
            for (int i = 0, sz = classIndex.Size(); i < sz; i++)
            {
                E label = classIndex.Get(i);
                c.IncrementCount(label, CondLogProbGivenNext(position, i, nextlabels));
            }

            return c;
        }

        public virtual ICounter<E> CondLogProbsGivenNext(int position, E[] nextlabels)
        {
            ICounter<E> c = new ClassicCounter<E>();
            for (int i = 0, sz = classIndex.Size(); i < sz; i++)
            {
                E label = classIndex.Get(i);
                c.IncrementCount(label, CondLogProbGivenNext(position, label, nextlabels));
            }

            return c;
        }

        public static CRFCliqueTree<E> GetCalibratedCliqueTree(int[][][] data, List<IIndex<CRFLabel>> labelIndices, 
            int numClasses, IIndex<E> classIndex, E backgroundSymbol, 
            ICliquePotentialFunction cliquePotentialFunc, double[][][] featureVals)
        {
            FactorTable[] factorTables = new FactorTable[data.Length];
            FactorTable[] messages = new FactorTable[data.Length - 1];
            for (int i = 0; i < data.Length; i++)
            {
                double[][] featureValByCliqueSize = null;
                if (featureVals != null)
                    featureValByCliqueSize = featureVals[i];
                factorTables[i] = GetFactorTable(data[i], labelIndices, numClasses, cliquePotentialFunc, featureValByCliqueSize);
                if (i > 0)
                {
                    messages[i - 1] = factorTables[i - 1].SumOutFront();
                    factorTables[i].MultiplyInFront(messages[i - 1]);
                }
            }

            for (int i = factorTables.Length - 2; i >= 0; i--)
            {
                FactorTable summedOut = factorTables[i + 1].SumOutEnd();
                summedOut.DivideBy(messages[i]);
                factorTables[i].MultiplyInEnd(summedOut);
            }

            return new CRFCliqueTree<E>(factorTables, classIndex, backgroundSymbol);
        }

        public static CRFCliqueTree<E> GetCalibratedCliqueTree(double[] weights, double wscale, 
            int[][] weightIndices, int[][][] data, List<IIndex<CRFLabel>> labelIndices, int numClasses, 
            IIndex<E> classIndex, E backgroundSymbol)
        {
            FactorTable[] factorTables = new FactorTable[data.Length];
            FactorTable[] messages = new FactorTable[data.Length - 1];
            for (int i = 0; i < data.Length; i++)
            {
                factorTables[i] = GetFactorTable(weights, wscale, weightIndices, data[i], labelIndices, numClasses);
                if (i > 0)
                {
                    messages[i - 1] = factorTables[i - 1].SumOutFront();
                    factorTables[i].MultiplyInFront(messages[i - 1]);
                }
            }

            for (int i = factorTables.Length - 2; i >= 0; i--)
            {
                FactorTable summedOut = factorTables[i + 1].SumOutEnd();
                summedOut.DivideBy(messages[i]);
                factorTables[i].MultiplyInEnd(summedOut);
            }

            return new CRFCliqueTree<E>(factorTables, classIndex, backgroundSymbol);
        }

        private static FactorTable GetFactorTable(double[] weights, double wscale, 
            int[][] weightIndices, int[][] data, List<IIndex<CRFLabel>> labelIndices, int numClasses)
        {
            FactorTable factorTable = null;
            for (int j = 0, sz = labelIndices.Size(); j < sz; j++)
            {
                IIndex<CRFLabel> labelIndex = labelIndices.Get(j);
                FactorTable ft = new FactorTable(numClasses, j + 1);
                for (int k = 0, liSize = labelIndex.Size(); k < liSize; k++)
                {
                    int[] label = ((CRFLabel)labelIndex.Get(k)).GetLabel();
                    double weight = 0.0;
                    for (int m = 0; m < data[j].Length; m++)
                    {
                        int wi = weightIndices[data[j][m]][k];
                        weight += wscale * weights[wi];
                    }

                    ft.SetValue(label, weight);
                }

                if (j > 0)
                {
                    ft.MultiplyInEnd(factorTable);
                }

                factorTable = ft;
            }

            return factorTable;
        }

        static FactorTable GetFactorTable(double[][] weights, int[][] data, List<IIndex<CRFLabel>> labelIndices, 
            int numClasses)
        {
            ICliquePotentialFunction cliquePotentialFunc = new LinearCliquePotentialFunction(weights);
            return GetFactorTable(data, labelIndices, numClasses, cliquePotentialFunc, null);
        }

        private static FactorTable GetFactorTable(int[][] data, List<IIndex<CRFLabel>> labelIndices, int numClasses, 
            ICliquePotentialFunction cliquePotentialFunc, double[][] featureValByCliqueSize)
        {
            FactorTable factorTable = null;
            for (int j = 0, sz = labelIndices.Size(); j < sz; j++)
            {
                IIndex<CRFLabel> labelIndex = labelIndices.Get(j);
                FactorTable ft = new FactorTable(numClasses, j + 1);
                double[] featureVal = null;
                if (featureValByCliqueSize != null)
                    featureVal = featureValByCliqueSize[j];
                for (int k = 0, liSize = labelIndex.Size(); k < liSize; k++)
                {
                    int[] label = ((CRFLabel)labelIndex.Get(k)).GetLabel();
                    double cliquePotential = cliquePotentialFunc.ComputeCliquePotential(j + 1, k, data[j], featureVal);
                    ft.SetValue(label, cliquePotential);
                }

                if (j > 0)
                {
                    ft.MultiplyInEnd(factorTable);
                }

                factorTable = ft;
            }

            return factorTable;
        }

        public virtual double[] GetConditionalDistribution(int[] sequence, int position)
        {
            double[] result = ScoresOf(sequence, position);
            ArrayMath.LogNormalize(result);
            result = ArrayMath.Exp(result);
            return result;
        }

        public override void UpdateSequenceElement(int[] sequence, int pos, int oldVal)
        {
        }

        public override void SetInitialSequence(int[] sequence)
        {
        }

        public virtual int GetNumValues()
        {
            return numClasses;
        }
    }
}
