using Stanford.NER.Net.Math;
using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class FactorTable
    {
        private readonly int numClasses;
        private readonly int windowSize;
        private readonly double[] table;

        public FactorTable(int numClasses, int windowSize)
        {
            this.numClasses = numClasses;
            this.windowSize = windowSize;
            table = new double[SloppyMath.IntPow(numClasses, windowSize)];
            Arrays.Fill(table, Double.NegativeInfinity);
        }

        public FactorTable(FactorTable t)
        {
            numClasses = t.NumClasses();
            windowSize = t.WindowSize();
            table = new double[t.Size()];
            Array.Copy(t.table, 0, table, 0, t.Size());
        }

        public virtual bool HasNaN()
        {
            return ArrayMath.HasNaN(table);
        }

        public virtual string ToProbString()
        {
            StringBuilder sb = new StringBuilder(@"{\n");
            for (int i = 0; i < table.Length; i++)
            {
                sb.Append(Arrays.ToString(ToArray(i)));
                sb.Append(@": ");
                sb.Append(Prob(ToArray(i)));
                sb.Append(@"\n");
            }

            sb.Append(@"}");
            return sb.ToString();
        }

        public virtual string ToNonLogString()
        {
            StringBuilder sb = new StringBuilder(@"{\n");
            for (int i = 0; i < table.Length; i++)
            {
                sb.Append(Arrays.ToString(ToArray(i)));
                sb.Append(@": ");
                sb.Append(System.Math.Exp(GetValue(i)));
                sb.Append(@"\n");
            }

            sb.Append(@"}");
            return sb.ToString();
        }

        public virtual string ToString(IIndex<L> classIndex)
        {
            StringBuilder sb = new StringBuilder(@"{\n");
            for (int i = 0; i < table.Length; i++)
            {
                sb.Append(ToString(ToArray(i), classIndex));
                sb.Append(@": ");
                sb.Append(GetValue(i));
                sb.Append(@"\n");
            }

            sb.Append(@"}");
            return sb.ToString();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(@"{\n");
            for (int i = 0; i < table.Length; i++)
            {
                sb.Append(Arrays.ToString(ToArray(i)));
                sb.Append(@": ");
                sb.Append(GetValue(i));
                sb.Append(@"\n");
            }

            sb.Append(@"}");
            return sb.ToString();
        }

        private static string ToString<L>(int[] array, IIndex<L> classIndex)
        {
            List<L> l = new List<L>(array.Length);
            for (int i = 0; i < array.Length; i++)
            {
                l.Add(classIndex.Get(array[i]));
            }

            return l.ToString();
        }

        public virtual int[] ToArray(int index)
        {
            int[] indices = new int[windowSize];
            for (int i = indices.Length - 1; i >= 0; i--)
            {
                indices[i] = index % numClasses;
                index /= numClasses;
            }

            return indices;
        }

        private int IndexOf(int[] entry)
        {
            int index = 0;
            for (int i = 0; i < entry.Length; i++)
            {
                index *= numClasses;
                index += entry[i];
            }

            return index;
        }

        private int IndexOf(int[] front, int end)
        {
            int index = 0;
            for (int i = 0; i < front.Length; i++)
            {
                index *= numClasses;
                index += front[i];
            }

            index *= numClasses;
            index += end;
            return index;
        }

        private int IndexOf(int front, int[] end)
        {
            int index = front;
            for (int i = 0; i < end.Length; i++)
            {
                index *= numClasses;
                index += end[i];
            }

            return index;
        }

        private int[] IndicesEnd(int[] entries)
        {
            int index = 0;
            for (int i = 0; i < entries.Length; i++)
            {
                index *= numClasses;
                index += entries[i];
            }

            int[] indices = new int[SloppyMath.IntPow(numClasses, windowSize - entries.Length)];
            int offset = SloppyMath.IntPow(numClasses, entries.Length);
            for (int i = 0; i < indices.Length; i++)
            {
                indices[i] = index;
                index += offset;
            }

            return indices;
        }

        private int IndicesFront(int[] entries)
        {
            int start = 0;
            foreach (int entry in entries)
            {
                start *= numClasses;
                start += entry;
            }

            int offset = SloppyMath.IntPow(numClasses, windowSize - entries.Length);
            return start * offset;
        }

        public virtual int WindowSize()
        {
            return windowSize;
        }

        public virtual int NumClasses()
        {
            return numClasses;
        }

        public virtual int Size()
        {
            return table.Length;
        }

        public virtual double TotalMass()
        {
            return ArrayMath.LogSum(table);
        }

        public virtual double UnnormalizedLogProb(int[] label)
        {
            return GetValue(label);
        }

        public virtual double LogProb(int[] label)
        {
            return UnnormalizedLogProb(label) - TotalMass();
        }

        public virtual double Prob(int[] label)
        {
            return System.Math.Exp(UnnormalizedLogProb(label) - TotalMass());
        }

        public virtual double ConditionalLogProbGivenPrevious(int[] given, int of)
        {
            if (given.Length != windowSize - 1)
            {
                throw new ArgumentException(@"conditionalLogProbGivenPrevious requires given one less than clique size (" + windowSize + @") but was " + Arrays.ToString(given));
            }

            int startIndex = IndicesFront(given);
            double z = ArrayMath.LogSum(table, startIndex, startIndex + numClasses);
            int i = startIndex + of;
            return table[i] - z;
        }

        public virtual double[] ConditionalLogProbsGivenPrevious(int[] given)
        {
            if (given.Length != windowSize - 1)
            {
                throw new ArgumentException(@"conditionalLogProbsGivenPrevious requires given one less than clique size (" + windowSize + @") but was " + Arrays.ToString(given));
            }

            double[] result = new double[numClasses];
            for (int i = 0; i < numClasses; i++)
            {
                int index = IndexOf(given, i);
                result[i] = table[index];
            }

            ArrayMath.LogNormalize(result);
            return result;
        }

        public virtual double ConditionalLogProbGivenFirst(int given, int[] of)
        {
            if (of.Length != windowSize - 1)
            {
                throw new ArgumentException(@"conditionalLogProbGivenFirst requires of one less than clique size (" + windowSize + @") but was " + Arrays.ToString(of));
            }

            int[] labels = new int[windowSize];
            labels[0] = given;
            Array.Copy(of, 0, labels, 1, windowSize - 1);
            double probAll = UnnormalizedLogProb(labels);
            double probGiven = UnnormalizedLogProbFront(given);
            return probAll - probGiven;
        }

        public virtual double UnnormalizedConditionalLogProbGivenFirst(int given, int[] of)
        {
            if (of.Length != windowSize - 1)
            {
                throw new ArgumentException(@"unnormalizedConditionalLogProbGivenFirst requires of one less than clique size (" + windowSize + @") but was " + Arrays.ToString(of));
            }

            int[] labels = new int[windowSize];
            labels[0] = given;
            Array.Copy(of, 0, labels, 1, windowSize - 1);
            double probAll = UnnormalizedLogProb(labels);
            return probAll;
        }

        public virtual double ConditionalLogProbGivenNext(int[] given, int of)
        {
            if (given.Length != windowSize - 1)
            {
                throw new ArgumentException(@"conditionalLogProbGivenNext requires given one less than clique size (" + windowSize + @") but was " + Arrays.ToString(given));
            }

            int[] label = IndicesEnd(given);
            double[] masses = new double[label.Length];
            for (int i = 0; i < masses.Length; i++)
            {
                masses[i] = table[label[i]];
            }

            double z = ArrayMath.LogSum(masses);
            return table[IndexOf(of, given)] - z;
        }

        public virtual double UnnormalizedLogProbFront(int[] labels)
        {
            int startIndex = IndicesFront(labels);
            int numCellsToSum = SloppyMath.IntPow(numClasses, windowSize - labels.Length);
            return ArrayMath.LogSum(table, startIndex, startIndex + numCellsToSum);
        }

        public virtual double LogProbFront(int[] label)
        {
            return UnnormalizedLogProbFront(label) - TotalMass();
        }

        public virtual double UnnormalizedLogProbFront(int label)
        {
            int[] labels = new[] { label };
            return UnnormalizedLogProbFront(labels);
        }

        public virtual double LogProbFront(int label)
        {
            return UnnormalizedLogProbFront(label) - TotalMass();
        }

        public virtual double UnnormalizedLogProbEnd(int[] labels)
        {
            labels = IndicesEnd(labels);
            double[] masses = new double[labels.Length];
            for (int i = 0; i < masses.Length; i++)
            {
                masses[i] = table[labels[i]];
            }

            return ArrayMath.LogSum(masses);
        }

        public virtual double LogProbEnd(int[] labels)
        {
            return UnnormalizedLogProbEnd(labels) - TotalMass();
        }

        public virtual double UnnormalizedLogProbEnd(int label)
        {
            int[] labels = new[] { label };
            return UnnormalizedLogProbEnd(labels);
        }

        public virtual double LogProbEnd(int label)
        {
            return UnnormalizedLogProbEnd(label) - TotalMass();
        }

        public virtual double GetValue(int index)
        {
            return table[index];
        }

        public virtual double GetValue(int[] label)
        {
            return table[IndexOf(label)];
        }

        public virtual void SetValue(int index, double value)
        {
            table[index] = value;
        }

        public virtual void SetValue(int[] label, double value)
        {
            table[IndexOf(label)] = value;
        }

        public virtual void IncrementValue(int[] label, double value)
        {
            IncrementValue(IndexOf(label), value);
        }

        public virtual void IncrementValue(int index, double value)
        {
            table[index] += value;
        }

        virtual void LogIncrementValue(int index, double value)
        {
            table[index] = SloppyMath.LogAdd(table[index], value);
        }

        public virtual void LogIncrementValue(int[] label, double value)
        {
            LogIncrementValue(IndexOf(label), value);
        }

        public virtual void MultiplyInFront(FactorTable other)
        {
            int divisor = SloppyMath.IntPow(numClasses, windowSize - other.WindowSize());
            for (int i = 0; i < table.Length; i++)
            {
                table[i] += other.GetValue(i / divisor);
            }
        }

        public virtual void MultiplyInEnd(FactorTable other)
        {
            int divisor = SloppyMath.IntPow(numClasses, other.WindowSize());
            for (int i = 0; i < table.Length; i++)
            {
                table[i] += other.GetValue(i % divisor);
            }
        }

        public virtual FactorTable SumOutEnd()
        {
            FactorTable ft = new FactorTable(numClasses, windowSize - 1);
            for (int i = 0, sz = ft.Size(); i < sz; i++)
            {
                ft.table[i] = ArrayMath.LogSum(table, i * numClasses, (i + 1) * numClasses);
            }

            return ft;
        }

        public virtual FactorTable SumOutFront()
        {
            FactorTable ft = new FactorTable(numClasses, windowSize - 1);
            int stride = ft.Size();
            for (int i = 0; i < stride; i++)
            {
                ft.SetValue(i, ArrayMath.LogSum(table, i, table.Length, stride));
            }

            return ft;
        }

        public virtual void DivideBy(FactorTable other)
        {
            for (int i = 0; i < table.Length; i++)
            {
                if (table[i] != Double.NegativeInfinity || other.table[i] != Double.NegativeInfinity)
                {
                    table[i] -= other.table[i];
                }
            }
        }

        public static void Main(String[] args)
        {
            int numClasses = 6;
            int cliqueSize = 3;
            Console.Error.Write(@"Creating factor table with {0} classes and window (clique) size {1}", numClasses, cliqueSize);
            FactorTable ft = new FactorTable(numClasses, cliqueSize);
            for (int i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < numClasses; j++)
                {
                    for (int k = 0; k < numClasses; k++)
                    {
                        int[] b = new[]
                    {
                    i, j, k
                    }

                        ;
                        ft.SetValue(b, (i * 4) + (j * 2) + k);
                    }
                }
            }

            Console.Error.WriteLine(ft);
            double normalization = 0.0;
            for (int i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < numClasses; j++)
                {
                    for (int k = 0; k < numClasses; k++)
                    {
                        normalization += ft.UnnormalizedLogProb(new int[] { i, j, k });
                    }
                }
            }

            Console.Error.WriteLine(@"Normalization Z = " + normalization);
            Console.Error.WriteLine(ft.SumOutFront());
            FactorTable ft2 = new FactorTable(numClasses, 2);
            for (int i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < numClasses; j++)
                {
                    int[] b = new[] { i, j };
                    ft2.SetValue(b, i * numClasses + j);
                }
            }

            Console.Error.WriteLine(ft2);
            for (int i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < numClasses; j++)
                {
                    int[] b = new[] { i, j };
                    double t = 0;
                    for (int k = 0; k < numClasses; k++)
                    {
                        t += System.Math.Exp(ft.ConditionalLogProbGivenPrevious(b, k));
                        Console.Error.WriteLine(k + @"|" + i + @"," + j + @" : " + System.Math.Exp(ft.ConditionalLogProbGivenPrevious(b, k)));
                    }

                    Console.Error.WriteLine(t);
                }
            }

            Console.Error.WriteLine(@"conditionalLogProbGivenFirst");
            for (int j = 0; j < numClasses; j++)
            {
                for (int k = 0; k < numClasses; k++)
                {
                    int[] b = new[] { j, k };
                    double t = 0.0;
                    for (int i = 0; i < numClasses; i++)
                    {
                        t += ft.UnnormalizedConditionalLogProbGivenFirst(i, b);
                        Console.Error.WriteLine(i + @"|" + j + @"," + k + @" : " + ft.UnnormalizedConditionalLogProbGivenFirst(i, b));
                    }

                    Console.Error.WriteLine(t);
                }
            }

            Console.Error.WriteLine(@"conditionalLogProbGivenFirst");
            for (int i = 0; i < numClasses; i++)
            {
                for (int j = 0; j < numClasses; j++)
                {
                    int[] b = new[] { i, j };
                    double t = 0.0;
                    for (int k = 0; k < numClasses; k++)
                    {
                        t += ft.ConditionalLogProbGivenNext(b, k);
                        Console.Error.WriteLine(i + @"," + j + @"|" + k + @" : " + ft.ConditionalLogProbGivenNext(b, k));
                    }

                    Console.Error.WriteLine(t);
                }
            }

            numClasses = 2;
            FactorTable ft3 = new FactorTable(numClasses, cliqueSize);
            ft3.SetValue(new int[] { 0, 0, 0 }, System.Math.Log(0.3));
            ft3.SetValue(new int[] { 0, 0, 1 }, System.Math.Log(0.4));
            ft3.SetValue(new int[] { 0, 1, 0 }, System.Math.Log(0.1));
            ft3.SetValue(new int[] { 0, 1, 1 }, System.Math.Log(0.1));
            ft3.SetValue(new int[] { 1, 0, 0 }, System.Math.Log(0.1));
            ft3.SetValue(new int[] { 1, 0, 1 }, System.Math.Log(0.2));
            ft3.SetValue(new int[] { 1, 1, 0 }, System.Math.Log(0.0));
            ft3.SetValue(new int[] { 1, 1, 1 }, System.Math.Log(0.0));
            FactorTable ft4 = ft3.SumOutFront();
            Console.Error.WriteLine(ft4.ToNonLogString());
            FactorTable ft5 = ft3.SumOutEnd();
            Console.Error.WriteLine(ft5.ToNonLogString());
        }
    }
}
