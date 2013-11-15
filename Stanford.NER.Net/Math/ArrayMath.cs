using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Math
{
    public class ArrayMath
    {
        private static readonly Random rand = new Random();
        private ArrayMath()
        {
        }

        public static int NumRows(double[] v)
        {
            return v.Length;
        }

        public static int[] Range(int start, int end)
        {
            int len = end - start;
            int[] range = new int[len];
            for (int i = 0; i < range.Length; ++i)
                range[i] = i + start;
            return range;
        }

        public static float[] DoubleArrayToFloatArray(double[] a)
        {
            float[] result = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (float)a[i];
            }

            return result;
        }

        public static double[] FloatArrayToDoubleArray(float[] a)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i];
            }

            return result;
        }

        public static double[][] FloatArrayToDoubleArray(float[][] a)
        {
            double[][] result = new double[a.Length][];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = new double[a[i].Length];
                for (int j = 0; j < a[i].Length; j++)
                {
                    result[i][j] = a[i][j];
                }
            }

            return result;
        }

        public static float[][] DoubleArrayToFloatArray(double[][] a)
        {
            float[][] result = new float[a.Length][];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = new float[a[i].Length];
                for (int j = 0; j < a[i].Length; j++)
                {
                    result[i][j] = (float)a[i][j];
                }
            }

            return result;
        }

        public static double[] Exp(double[] a)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = System.Math.Exp(a[i]);
            }

            return result;
        }

        public static double[] Log(double[] a)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = System.Math.Log(a[i]);
            }

            return result;
        }

        public static void ExpInPlace(double[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = System.Math.Exp(a[i]);
            }
        }

        public static void LogInPlace(double[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = System.Math.Log(a[i]);
            }
        }

        public static double[] Softmax(double[] scales)
        {
            double[] newScales = new double[scales.Length];
            double sum = 0;
            for (int i = 0; i < scales.Length; i++)
            {
                newScales[i] = System.Math.Exp(scales[i]);
                sum += newScales[i];
            }

            for (int i = 0; i < scales.Length; i++)
            {
                newScales[i] /= sum;
            }

            return newScales;
        }

        public static void AddInPlace(double[] a, double b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = a[i] + b;
            }
        }

        public static void AddInPlace(float[] a, double b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = (float)(a[i] + b);
            }
        }

        public static void AddMultInPlace(double[] a, double[] b, double c)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] += b[i] * c;
            }
        }

        public static void MultiplyInPlace(double[] a, double b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = a[i] * b;
            }
        }

        public static void MultiplyInPlace(float[] a, double b)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = (float)(a[i] * b);
            }
        }

        public static void PowInPlace(double[] a, double c)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = System.Math.Pow(a[i], c);
            }
        }

        public static void PowInPlace(float[] a, float c)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = (float)System.Math.Pow(a[i], c);
            }
        }

        public static double[] Add(double[] a, double c)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] + c;
            }

            return result;
        }

        public static float[] Add(float[] a, double c)
        {
            float[] result = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (float)(a[i] + c);
            }

            return result;
        }

        public static double[] Multiply(double[] a, double c)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] * c;
            }

            return result;
        }

        public static float[] Multiply(float[] a, float c)
        {
            float[] result = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] * c;
            }

            return result;
        }

        public static double[] Pow(double[] a, double c)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = System.Math.Pow(a[i], c);
            }

            return result;
        }

        public static float[] Pow(float[] a, float c)
        {
            float[] result = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = (float)System.Math.Pow(a[i], c);
            }

            return result;
        }

        public static void PairwiseAddInPlace(double[] to, double[] from)
        {
            if (to.Length != from.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = to[i] + from[i];
            }
        }

        public static void PairwiseAddInPlace(double[] to, int[] from)
        {
            if (to.Length != from.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = to[i] + from[i];
            }
        }

        public static void PairwiseAddInPlace(double[] to, short[] from)
        {
            if (to.Length != from.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = to[i] + from[i];
            }
        }

        public static void PairwiseSubtractInPlace(double[] to, double[] from)
        {
            if (to.Length != from.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = to[i] - from[i];
            }
        }

        public static void PairwiseScaleAddInPlace(double[] to, double[] from, double fromScale)
        {
            if (to.Length != from.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < to.Length; i++)
            {
                to[i] = to[i] + fromScale * from[i];
            }
        }

        public static int[] PairwiseAdd(int[] a, int[] b)
        {
            int[] result = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        public static double[] PairwiseAdd(double[] a, double[] b)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                if (i < b.Length)
                {
                    result[i] = a[i] + b[i];
                }
                else
                {
                    result[i] = a[i];
                }
            }

            return result;
        }

        public static float[] PairwiseAdd(float[] a, float[] b)
        {
            float[] result = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] + b[i];
            }

            return result;
        }

        public static double[] PairwiseScaleAdd(double[] a, double[] b, double bScale)
        {
            double[] result = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = a[i] + bScale * b[i];
            }

            return result;
        }

        public static double[] PairwiseSubtract(double[] a, double[] b)
        {
            double[] c = new double[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                c[i] = a[i] - b[i];
            }

            return c;
        }

        public static float[] PairwiseSubtract(float[] a, float[] b)
        {
            float[] c = new float[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                c[i] = a[i] - b[i];
            }

            return c;
        }

        public static double[] PairwiseMultiply(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new Exception(@"Can't pairwise multiple different lengths: a.Length=" + a.Length + @" b.Length=" + b.Length);
            }

            double[] result = new double[a.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] * b[i];
            }

            return result;
        }

        public static float[] PairwiseMultiply(float[] a, float[] b)
        {
            if (a.Length != b.Length)
            {
                throw new Exception();
            }

            float[] result = new float[a.Length];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] * b[i];
            }

            return result;
        }

        public static void PairwiseMultiply(double[] a, double[] b, double[] result)
        {
            if (a.Length != b.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] * b[i];
            }
        }

        public static void PairwiseMultiply(float[] a, float[] b, float[] result)
        {
            if (a.Length != b.Length)
            {
                throw new Exception();
            }

            for (int i = 0; i < result.Length; i++)
            {
                result[i] = a[i] * b[i];
            }
        }

        public static bool HasNaN(double[] a)
        {
            foreach (double x in a)
            {
                if (Double.IsNaN(x))
                    return true;
            }

            return false;
        }

        public static bool HasInfinite(double[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (Double.IsInfinity(a[i]))
                    return true;
            }

            return false;
        }

        public static bool HasNaN(float[] a)
        {
            foreach (float x in a)
            {
                if (float.IsNaN(x))
                    return true;
            }

            return false;
        }

        public static int CountNaN(double[] v)
        {
            int c = 0;
            foreach (double d in v)
            {
                if (Double.IsNaN(d))
                {
                    c++;
                }
            }

            return c;
        }

        public static double[] FilterNaN(double[] v)
        {
            double[] u = new double[NumRows(v) - CountNaN(v)];
            int j = 0;
            foreach (double d in v)
            {
                if (!Double.IsNaN(d))
                {
                    u[j++] = d;
                }
            }

            return u;
        }

        public static int CountInfinite(double[] v)
        {
            int c = 0;
            for (int i = 0; i < v.Length; i++)
                if (Double.IsInfinity(v[i]))
                    c++;
            return c;
        }

        public static int CountNonZero(double[] v)
        {
            int c = 0;
            for (int i = 0; i < v.Length; i++)
                if (v[i] != 0.0)
                    ++c;
            return c;
        }

        public static int CountCloseToZero(double[] v, double epsilon)
        {
            int c = 0;
            for (int i = 0; i < v.Length; i++)
                if (System.Math.Abs(v[i]) < epsilon)
                    ++c;
            return c;
        }

        public static int CountPositive(double[] v)
        {
            int c = 0;
            for (int i = 0; i < v.Length; i++)
                if (v[i] > 0.0)
                    ++c;
            return c;
        }

        public static int CountNegative(double[] v)
        {
            int c = 0;
            for (int i = 0; i < v.Length; i++)
                if (v[i] < 0.0)
                    ++c;
            return c;
        }

        public static double[] FilterInfinite(double[] v)
        {
            double[] u = new double[NumRows(v) - CountInfinite(v)];
            int j = 0;
            for (int i = 0; i < v.Length; i++)
            {
                if (!Double.IsInfinity(v[i]))
                {
                    u[j++] = v[i];
                }
            }

            return u;
        }

        public static double[] FilterNaNAndInfinite(double[] v)
        {
            return FilterInfinite(FilterNaN(v));
        }

        public static double Sum(double[] a)
        {
            return Sum(a, 0, a.Length);
        }

        public static double Sum(double[] a, int fromIndex, int toIndex)
        {
            double result = 0.0;
            for (int i = fromIndex; i < toIndex; i++)
            {
                result += a[i];
            }

            return result;
        }

        public static int Sum(int[] a)
        {
            int result = 0;
            foreach (int i in a)
            {
                result += i;
            }

            return result;
        }

        public static float Sum(float[] a)
        {
            float result = 0F;
            foreach (float f in a)
            {
                result += f;
            }

            return result;
        }

        public static int Sum(int[][] a)
        {
            int result = 0;
            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < a[i].Length; j++)
                {
                    result += a[i][j];
                }
            }

            return result;
        }

        public static int[] Diag(int[][] a)
        {
            int[] rv = new int[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                rv[i] = a[i][i];
            }

            return rv;
        }

        public static double Average(double[] a)
        {
            double total = ArrayMath.Sum(a);
            return total / a.Length;
        }

        public static double Norm_inf(double[] a)
        {
            double max = Double.NegativeInfinity;
            foreach (double d in a)
            {
                if (System.Math.Abs(d) > max)
                {
                    max = System.Math.Abs(d);
                }
            }

            return max;
        }

        public static double Norm_inf(float[] a)
        {
            double max = Double.NegativeInfinity;
            for (int i = 0; i < a.Length; i++)
            {
                if (System.Math.Abs(a[i]) > max)
                {
                    max = System.Math.Abs(a[i]);
                }
            }

            return max;
        }

        public static double Norm_1(double[] a)
        {
            double sum = 0;
            foreach (double anA in a)
            {
                sum += (anA < 0 ? -anA : anA);
            }

            return sum;
        }

        public static double Norm_1(float[] a)
        {
            double sum = 0;
            foreach (float anA in a)
            {
                sum += (anA < 0 ? -anA : anA);
            }

            return sum;
        }

        public static double Norm(double[] a)
        {
            double squaredSum = 0;
            foreach (double anA in a)
            {
                squaredSum += anA * anA;
            }

            return System.Math.Sqrt(squaredSum);
        }

        public static double Norm(float[] a)
        {
            double squaredSum = 0;
            foreach (float anA in a)
            {
                squaredSum += anA * anA;
            }

            return System.Math.Sqrt(squaredSum);
        }

        public static int Argmax(double[] a)
        {
            double max = Double.NegativeInfinity;
            int argmax = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                    argmax = i;
                }
            }

            return argmax;
        }

        public static int Argmax_tieLast(double[] a)
        {
            double max = Double.NegativeInfinity;
            int argmax = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] >= max)
                {
                    max = a[i];
                    argmax = i;
                }
            }

            return argmax;
        }

        public static double Max(double[] a)
        {
            return a[Argmax(a)];
        }

        public static double Max(IEnumerable<Double> a)
        {
            double max = Double.NegativeInfinity;
            foreach (double d in a)
            {
                if (d > max)
                {
                    max = d;
                }
            }

            return max;
        }

        public static int Argmax(float[] a)
        {
            float max = float.NegativeInfinity;
            int argmax = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                    argmax = i;
                }
            }

            return argmax;
        }

        public static float Max(float[] a)
        {
            return a[Argmax(a)];
        }

        public static int Argmin(double[] a)
        {
            double min = Double.PositiveInfinity;
            int argmin = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < min)
                {
                    min = a[i];
                    argmin = i;
                }
            }

            return argmin;
        }

        public static double Min(double[] a)
        {
            return a[Argmin(a)];
        }

        public static double SafeMin(double[] v)
        {
            double[] u = FilterNaNAndInfinite(v);
            if (NumRows(u) == 0)
                return 0.0;
            return Min(u);
        }

        public static int Argmin(float[] a)
        {
            float min = float.NegativeInfinity;
            int argmin = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < min)
                {
                    min = a[i];
                    argmin = i;
                }
            }

            return argmin;
        }

        public static float Min(float[] a)
        {
            return a[Argmin(a)];
        }

        public static int Argmin(int[] a)
        {
            int min = int.MaxValue;
            int argmin = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] < min)
                {
                    min = a[i];
                    argmin = i;
                }
            }

            return argmin;
        }

        public static int Min(int[] a)
        {
            return a[Argmin(a)];
        }

        public static int Argmax(int[] a)
        {
            int max = int.MinValue;
            int argmax = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] > max)
                {
                    max = a[i];
                    argmax = i;
                }
            }

            return argmax;
        }

        public static int Max(int[] a)
        {
            return a[Argmax(a)];
        }

        public static int Min(int[][] matrix)
        {
            int min = int.MaxValue;
            foreach (int[] row in matrix)
            {
                foreach (int elem in row)
                {
                    min = System.Math.Min(min, elem);
                }
            }

            return min;
        }

        public static int Max(int[][] matrix)
        {
            int max = int.MinValue;
            foreach (int[] row in matrix)
            {
                foreach (int elem in row)
                {
                    max = System.Math.Max(max, elem);
                }
            }

            return max;
        }

        public static double SafeMax(double[] v)
        {
            double[] u = FilterNaNAndInfinite(v);
            if (NumRows(u) == 0)
                return 0.0;
            return Max(u);
        }

        public static double LogSum(params double[] logInputs)
        {
            return LogSum(logInputs, 0, logInputs.Length);
        }

        public static double LogSum(double[] logInputs, int fromIndex, int toIndex)
        {
            if (logInputs.Length == 0)
                throw new ArgumentException();
            if (fromIndex >= 0 && toIndex < logInputs.Length && fromIndex >= toIndex)
                return Double.NegativeInfinity;
            int maxIdx = fromIndex;
            double max = logInputs[fromIndex];
            for (int i = fromIndex + 1; i < toIndex; i++)
            {
                if (logInputs[i] > max)
                {
                    maxIdx = i;
                    max = logInputs[i];
                }
            }

            bool haveTerms = false;
            double intermediate = 0.0;
            double cutoff = max - SloppyMath.LOGTOLERANCE;
            for (int i = fromIndex; i < toIndex; i++)
            {
                if (i != maxIdx && logInputs[i] > cutoff)
                {
                    haveTerms = true;
                    intermediate += System.Math.Exp(logInputs[i] - max);
                }
            }

            if (haveTerms)
            {
                return max + System.Math.Log(1.0 + intermediate);
            }
            else
            {
                return max;
            }
        }

        public static double LogSum(double[] logInputs, int fromIndex, int afterIndex, int stride)
        {
            if (logInputs.Length == 0)
                throw new ArgumentException();
            if (fromIndex >= 0 && afterIndex < logInputs.Length && fromIndex >= afterIndex)
                return Double.NegativeInfinity;
            int maxIdx = fromIndex;
            double max = logInputs[fromIndex];
            for (int i = fromIndex + stride; i < afterIndex; i += stride)
            {
                if (logInputs[i] > max)
                {
                    maxIdx = i;
                    max = logInputs[i];
                }
            }

            bool haveTerms = false;
            double intermediate = 0.0;
            double cutoff = max - SloppyMath.LOGTOLERANCE;
            for (int i = fromIndex; i < afterIndex; i += stride)
            {
                if (i != maxIdx && logInputs[i] > cutoff)
                {
                    haveTerms = true;
                    intermediate += System.Math.Exp(logInputs[i] - max);
                }
            }

            if (haveTerms)
            {
                return max + System.Math.Log(1.0 + intermediate);
            }
            else
            {
                return max;
            }
        }

        public static double LogSum(List<Double> logInputs)
        {
            return LogSum(logInputs, 0, logInputs.Size());
        }

        public static double LogSum(List<Double> logInputs, int fromIndex, int toIndex)
        {
            int length = logInputs.Size();
            if (length == 0)
                throw new ArgumentException();
            if (fromIndex >= 0 && toIndex < length && fromIndex >= toIndex)
                return Double.NegativeInfinity;
            int maxIdx = fromIndex;
            double max = logInputs.Get(fromIndex);
            for (int i = fromIndex + 1; i < toIndex; i++)
            {
                double d = logInputs.Get(i);
                if (d > max)
                {
                    maxIdx = i;
                    max = d;
                }
            }

            bool haveTerms = false;
            double intermediate = 0.0;
            double cutoff = max - SloppyMath.LOGTOLERANCE;
            for (int i = fromIndex; i < toIndex; i++)
            {
                double d = logInputs.Get(i);
                if (i != maxIdx && d > cutoff)
                {
                    haveTerms = true;
                    intermediate += System.Math.Exp(d - max);
                }
            }

            if (haveTerms)
            {
                return max + System.Math.Log(1.0 + intermediate);
            }
            else
            {
                return max;
            }
        }

        public static float LogSum(float[] logInputs)
        {
            int leng = logInputs.Length;
            if (leng == 0)
            {
                throw new ArgumentException();
            }

            int maxIdx = 0;
            float max = logInputs[0];
            for (int i = 1; i < leng; i++)
            {
                if (logInputs[i] > max)
                {
                    maxIdx = i;
                    max = logInputs[i];
                }
            }

            bool haveTerms = false;
            double intermediate = 0F;
            float cutoff = max - SloppyMath.LOGTOLERANCE_F;
            for (int i = 0; i < leng; i++)
            {
                if (i != maxIdx && logInputs[i] > cutoff)
                {
                    haveTerms = true;
                    intermediate += System.Math.Exp(logInputs[i] - max);
                }
            }

            if (haveTerms)
            {
                return max + (float)System.Math.Log(1.0 + intermediate);
            }
            else
            {
                return max;
            }
        }

        public static double InnerProduct(double[] a, double[] b)
        {
            double result = 0.0;
            int len = System.Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                result += a[i] * b[i];
            }

            return result;
        }

        public static double InnerProduct(float[] a, float[] b)
        {
            double result = 0.0;
            int len = System.Math.Min(a.Length, b.Length);
            for (int i = 0; i < len; i++)
            {
                result += a[i] * b[i];
            }

            return result;
        }

        public static int[] SubArray(int[] a, int from, int to)
        {
            int[] result = new int[to - from];
            Array.Copy(a, from, result, 0, to - from);
            return result;
        }

        public static double[][] Load2DMatrixFromFile(string filename)
        {
            string s = IOUtils.SlurpFile(filename);
            String[] rows = s.Split(@"[\r\n]+");
            double[][] result = new double[rows.Length][];
            for (int i = 0; i < result.Length; i++)
            {
                String[] columns = rows[i].Split(@"\\s+");
                result[i] = new double[columns.Length];
                for (int j = 0; j < result[i].Length; j++)
                {
                    result[i][j] = double.Parse(columns[j]);
                }
            }

            return result;
        }


        public static int IndexOf(int n, int[] a)
        {
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == n)
                    return i;
            }

            return -1;
        }

        public static int[][] CastToInt(double[][] doubleCounts)
        {
            int[][] result = new int[doubleCounts.Length][];
            for (int i = 0; i < doubleCounts.Length; i++)
            {
                result[i] = new int[doubleCounts[i].Length];
                for (int j = 0; j < doubleCounts[i].Length; j++)
                {
                    result[i][j] = (int)doubleCounts[i][j];
                }
            }

            return result;
        }

        public static void Normalize(double[] a)
        {
            double total = Sum(a);
            if (total == 0.0 || Double.IsNaN(total))
            {
                throw new Exception(@"Can't normalize an array with sum 0.0 or NaN: " + Arrays.ToString(a));
            }

            MultiplyInPlace(a, 1.0 / total);
        }

        public static void L1normalize(double[] a)
        {
            double total = L1Norm(a);
            if (total == 0.0 || Double.IsNaN(total))
            {
                if (a.Length < 100)
                {
                    throw new Exception(@"Can't normalize an array with sum 0.0 or NaN: " + Arrays.ToString(a));
                }
                else
                {
                    double[] aTrunc = new double[100];
                    Array.Copy(a, 0, aTrunc, 0, 100);
                    throw new Exception(@"Can't normalize an array with sum 0.0 or NaN: " + Arrays.ToString(aTrunc) + @" ... ");
                }
            }

            MultiplyInPlace(a, 1.0 / total);
        }

        public static void Normalize(float[] a)
        {
            float total = Sum(a);
            if (total == 0F || Double.IsNaN(total))
            {
                throw new Exception(@"Can't normalize an array with sum 0.0 or NaN");
            }

            MultiplyInPlace(a, 1F / total);
        }

        public static void Standardize(double[] a)
        {
            double m = Mean(a);
            if (Double.IsNaN(m))
                throw new Exception(@"Can't standardize array whose mean is NaN");
            double s = Stdev(a);
            if (s == 0.0 || Double.IsNaN(s))
                throw new Exception(@"Can't standardize array whose standard deviation is 0.0 or NaN");
            AddInPlace(a, -m);
            MultiplyInPlace(a, 1.0 / s);
        }

        public static double L2Norm(double[] a)
        {
            double result = 0.0;
            foreach (double d in a)
            {
                result += System.Math.Pow(d, 2);
            }

            return System.Math.Sqrt(result);
        }

        public static double L1Norm(double[] a)
        {
            double result = 0.0;
            foreach (double d in a)
            {
                result += System.Math.Abs(d);
            }

            return result;
        }

        public static void LogNormalize(double[] a)
        {
            double logTotal = LogSum(a);
            if (logTotal == Double.NegativeInfinity)
            {
                double v = -System.Math.Log(a.Length);
                for (int i = 0; i < a.Length; i++)
                {
                    a[i] = v;
                }

                return;
            }

            AddInPlace(a, -logTotal);
        }

        public static int SampleFromDistribution(double[] d)
        {
            return SampleFromDistribution(d, rand);
        }

        public static int SampleFromDistribution(double[] d, Random random)
        {
            double r = random.NextDouble();
            double total = 0;
            for (int i = 0; i < d.Length - 1; i++)
            {
                if (Double.IsNaN(d[i]))
                {
                    throw new Exception(@"Can't sample from NaN");
                }

                total += d[i];
                if (r < total)
                {
                    return i;
                }
            }

            return d.Length - 1;
        }

        public static int SampleFromDistribution(float[] d, Random random)
        {
            double r = random.NextDouble();
            double total = 0;
            for (int i = 0; i < d.Length - 1; i++)
            {
                if (float.IsNaN(d[i]))
                {
                    throw new Exception(@"Can't sample from NaN");
                }

                total += d[i];
                if (r < total)
                {
                    return i;
                }
            }

            return d.Length - 1;
        }

        public static double KlDivergence(double[] from, double[] to)
        {
            double kl = 0.0;
            double tot = Sum(from);
            double tot2 = Sum(to);
            for (int i = 0; i < from.Length; i++)
            {
                if (from[i] == 0.0)
                {
                    continue;
                }

                double num = from[i] / tot;
                double num2 = to[i] / tot2;
                kl += num * (System.Math.Log(num / num2) / System.Math.Log(2.0));
            }

            return kl;
        }

        public static double JensenShannonDivergence(double[] a, double[] b)
        {
            double[] average = PairwiseAdd(a, b);
            MultiplyInPlace(average, 0.5);
            return 0.5 * KlDivergence(a, average) + 0.5 * KlDivergence(b, average);
        }

        public static void SetToLogDeterministic(float[] a, int i)
        {
            for (int j = 0; j < a.Length; j++)
            {
                if (j == i)
                {
                    a[j] = 0F;
                }
                else
                {
                    a[j] = float.NegativeInfinity;
                }
            }
        }

        public static void SetToLogDeterministic(double[] a, int i)
        {
            for (int j = 0; j < a.Length; j++)
            {
                if (j == i)
                {
                    a[j] = 0.0;
                }
                else
                {
                    a[j] = Double.NegativeInfinity;
                }
            }
        }

        public static double Mean(double[] a)
        {
            return Sum(a) / a.Length;
        }

        public static double Median(double[] a)
        {
            double[] b = new double[a.Length];
            Array.Copy(a, 0, b, 0, b.Length);
            Array.Sort(b);
            int mid = b.Length / 2;
            if (b.Length % 2 == 0)
            {
                return (b[mid - 1] + b[mid]) / 2.0;
            }
            else
            {
                return b[mid];
            }
        }

        public static double SafeMean(double[] v)
        {
            double[] u = FilterNaNAndInfinite(v);
            if (NumRows(u) == 0)
                return 0.0;
            return Mean(u);
        }

        public static double SumSquaredError(double[] a)
        {
            double mean = Mean(a);
            double result = 0.0;
            foreach (double anA in a)
            {
                double diff = anA - mean;
                result += (diff * diff);
            }

            return result;
        }

        public static double SumSquared(double[] a)
        {
            double result = 0.0;
            foreach (double anA in a)
            {
                result += (anA * anA);
            }

            return result;
        }

        public static double Variance(double[] a)
        {
            return SumSquaredError(a) / (a.Length - 1);
        }

        public static double Stdev(double[] a)
        {
            return System.Math.Sqrt(Variance(a));
        }

        public static double SafeStdev(double[] v)
        {
            double[] u = FilterNaNAndInfinite(v);
            if (NumRows(u) < 2)
                return 1.0;
            return Stdev(u);
        }

        public static double StandardErrorOfMean(double[] a)
        {
            return Stdev(a) / System.Math.Sqrt(a.Length);
        }

        public static void SampleWithoutReplacement(int[] array, int numArgClasses)
        {
            SampleWithoutReplacement(array, numArgClasses, rand);
        }

        public static void SampleWithoutReplacement(int[] array, int numArgClasses, Random rand)
        {
            int[] temp = new int[numArgClasses];
            for (int i = 0; i < temp.Length; i++)
            {
                temp[i] = i;
            }

            Shuffle(temp, rand);
            Array.Copy(temp, 0, array, 0, array.Length);
        }

        public static void Shuffle(int[] a)
        {
            Shuffle(a, rand);
        }

        public static void Shuffle(int[] a, Random rand)
        {
            for (int i = a.Length - 1; i >= 1; i--)
            {
                int j = rand.Next(i + 1);
                int tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
            }
        }

        public static void Reverse(int[] a)
        {
            for (int i = 0; i < a.Length / 2; i++)
            {
                int j = a.Length - i - 1;
                int tmp = a[i];
                a[i] = a[j];
                a[j] = tmp;
            }
        }

        public static bool Contains(int[] a, int i)
        {
            foreach (int k in a)
            {
                if (k == i)
                    return true;
            }

            return false;
        }

        public static bool ContainsInSubarray(int[] a, int begin, int end, int i)
        {
            for (int j = begin; j < end; j++)
            {
                if (a[j] == i)
                    return true;
            }

            return false;
        }

        public static double PearsonCorrelation(double[] x, double[] y)
        {
            double result;
            double sum_sq_x = 0, sum_sq_y = 0;
            double mean_x = x[0], mean_y = y[0];
            double sum_coproduct = 0;
            for (int i = 2; i < x.Length + 1; ++i)
            {
                double w = (i - 1) * 1.0 / i;
                double delta_x = x[i - 1] - mean_x;
                double delta_y = y[i - 1] - mean_y;
                sum_sq_x += delta_x * delta_x * w;
                sum_sq_y += delta_y * delta_y * w;
                sum_coproduct += delta_x * delta_y * w;
                mean_x += delta_x / i;
                mean_y += delta_y / i;
            }

            double pop_sd_x = System.Math.Sqrt(sum_sq_x / x.Length);
            double pop_sd_y = System.Math.Sqrt(sum_sq_y / y.Length);
            double cov_x_y = sum_coproduct / x.Length;
            double denom = pop_sd_x * pop_sd_y;
            if (denom == 0.0)
                return 0.0;
            result = cov_x_y / denom;
            return result;
        }

        public static double SigLevelByApproxRand(double[] A, double[] B)
        {
            return SigLevelByApproxRand(A, B, 1000);
        }

        public static double SigLevelByApproxRand(double[] A, double[] B, int iterations)
        {
            if (A.Length == 0)
                throw new ArgumentException(@"Input arrays must not be empty!");
            if (A.Length != B.Length)
                throw new ArgumentException(@"Input arrays must have equal length!");
            if (iterations <= 0)
                throw new ArgumentException(@"Number of iterations must be positive!");
            double testStatistic = AbsDiffOfMeans(A, B, false);
            int successes = 0;
            for (int i = 0; i < iterations; i++)
            {
                double t = AbsDiffOfMeans(A, B, true);
                if (t >= testStatistic)
                    successes++;
            }

            return (double)(successes + 1) / (double)(iterations + 1);
        }

        public static double SigLevelByApproxRand(int[] A, int[] B)
        {
            return SigLevelByApproxRand(A, B, 1000);
        }

        public static double SigLevelByApproxRand(int[] A, int[] B, int iterations)
        {
            if (A.Length == 0)
                throw new ArgumentException(@"Input arrays must not be empty!");
            if (A.Length != B.Length)
                throw new ArgumentException(@"Input arrays must have equal length!");
            if (iterations <= 0)
                throw new ArgumentException(@"Number of iterations must be positive!");
            double[] X = new double[A.Length];
            double[] Y = new double[B.Length];
            for (int i = 0; i < A.Length; i++)
            {
                X[i] = A[i];
                Y[i] = B[i];
            }

            return SigLevelByApproxRand(X, Y, iterations);
        }

        public static double SigLevelByApproxRand(bool[] A, bool[] B)
        {
            return SigLevelByApproxRand(A, B, 1000);
        }

        public static double SigLevelByApproxRand(bool[] A, bool[] B, int iterations)
        {
            if (A.Length == 0)
                throw new ArgumentException(@"Input arrays must not be empty!");
            if (A.Length != B.Length)
                throw new ArgumentException(@"Input arrays must have equal length!");
            if (iterations <= 0)
                throw new ArgumentException(@"Number of iterations must be positive!");
            double[] X = new double[A.Length];
            double[] Y = new double[B.Length];
            for (int i = 0; i < A.Length; i++)
            {
                X[i] = (A[i] ? 1.0 : 0.0);
                Y[i] = (B[i] ? 1.0 : 0.0);
            }

            return SigLevelByApproxRand(X, Y, iterations);
        }

        private static double AbsDiffOfMeans(double[] A, double[] B, bool randomize)
        {
            Random random = new Random();
            double aTotal = 0.0;
            double bTotal = 0.0;
            for (int i = 0; i < A.Length; i++)
            {
                if (randomize && random.NextBoolean())
                {
                    aTotal += B[i];
                    bTotal += A[i];
                }
                else
                {
                    aTotal += A[i];
                    bTotal += B[i];
                }
            }

            double aMean = aTotal / A.Length;
            double bMean = bTotal / B.Length;
            return System.Math.Abs(aMean - bMean);
        }

        public static string ToBinaryString(byte[] b)
        {
            StringBuilder s = new StringBuilder();
            foreach (byte by in b)
            {
                for (int j = 7; j >= 0; j--)
                {
                    if ((by & (1 << j)) > 0)
                    {
                        s.Append('1');
                    }
                    else
                    {
                        s.Append('0');
                    }
                }

                s.Append(' ');
            }

            return s.ToString();
        }

        public static string ToString(double[] a)
        {
            return ToString(a, null);
        }

        public static string ToString(double[] a, NumberFormat nf)
        {
            if (a == null)
                return null;
            if (a.Length == 0)
                return @"[]";
            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; i < a.Length - 1; i++)
            {
                string s;
                if (nf == null)
                {
                    s = String.ValueOf(a[i]);
                }
                else
                {
                    s = nf.Format(a[i]);
                }

                b.Append(s);
                b.Append(@", ");
            }

            string s;
            if (nf == null)
            {
                s = String.ValueOf(a[a.Length - 1]);
            }
            else
            {
                s = nf.Format(a[a.Length - 1]);
            }

            b.Append(s);
            b.Append(']');
            return b.ToString();
        }

        public static string ToString(float[] a)
        {
            return ToString(a, null);
        }

        public static string ToString(float[] a, NumberFormat nf)
        {
            if (a == null)
                return null;
            if (a.Length == 0)
                return @"[]";
            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; i < a.Length - 1; i++)
            {
                string s;
                if (nf == null)
                {
                    s = String.ValueOf(a[i]);
                }
                else
                {
                    s = nf.Format(a[i]);
                }

                b.Append(s);
                b.Append(@", ");
            }

            string s;
            if (nf == null)
            {
                s = String.ValueOf(a[a.Length - 1]);
            }
            else
            {
                s = nf.Format(a[a.Length - 1]);
            }

            b.Append(s);
            b.Append(']');
            return b.ToString();
        }

        public static string ToString(int[] a)
        {
            return ToString(a, null);
        }

        public static string ToString(int[] a, NumberFormat nf)
        {
            if (a == null)
                return null;
            if (a.Length == 0)
                return @"[]";
            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; i < a.Length - 1; i++)
            {
                string s;
                if (nf == null)
                {
                    s = String.ValueOf(a[i]);
                }
                else
                {
                    s = nf.Format(a[i]);
                }

                b.Append(s);
                b.Append(@", ");
            }

            string s;
            if (nf == null)
            {
                s = String.ValueOf(a[a.Length - 1]);
            }
            else
            {
                s = nf.Format(a[a.Length - 1]);
            }

            b.Append(s);
            b.Append(']');
            return b.ToString();
        }

        public static string ToString(byte[] a)
        {
            return ToString(a, null);
        }

        public static string ToString(byte[] a, NumberFormat nf)
        {
            if (a == null)
                return null;
            if (a.Length == 0)
                return @"[]";
            StringBuilder b = new StringBuilder();
            b.Append('[');
            for (int i = 0; i < a.Length - 1; i++)
            {
                string s;
                if (nf == null)
                {
                    s = String.ValueOf(a[i]);
                }
                else
                {
                    s = nf.Format(a[i]);
                }

                b.Append(s);
                b.Append(@", ");
            }

            string s;
            if (nf == null)
            {
                s = String.ValueOf(a[a.Length - 1]);
            }
            else
            {
                s = nf.Format(a[a.Length - 1]);
            }

            b.Append(s);
            b.Append(']');
            return b.ToString();
        }

        public static string ToString(int[][] counts)
        {
            return ToString(counts, null, null, 10, 10, NumberFormat.GetInstance(), false);
        }

        public static string ToString(int[][] counts, Object[] rowLabels, Object[] colLabels, int labelSize, int cellSize, NumberFormat nf, bool printTotals)
        {
            if (counts.Length == 0 || counts[0].Length == 0)
                return @"";
            int[] rowTotals = new int[counts.Length];
            int[] colTotals = new int[counts[0].Length];
            int total = 0;
            for (int i = 0; i < counts.Length; i++)
            {
                for (int j = 0; j < counts[i].Length; j++)
                {
                    rowTotals[i] += counts[i][j];
                    colTotals[j] += counts[i][j];
                    total += counts[i][j];
                }
            }

            StringBuilder result = new StringBuilder();
            if (colLabels != null)
            {
                result.Append(StringUtils.PadLeft(@"", labelSize));
                for (int j = 0; j < counts[0].Length; j++)
                {
                    string s = (colLabels[j] == null ? @"null" : colLabels[j].ToString());
                    if (s.Length() > cellSize - 1)
                    {
                        s = s.Substring(0, cellSize - 1);
                    }

                    s = StringUtils.PadLeft(s, cellSize);
                    result.Append(s);
                }

                if (printTotals)
                {
                    result.Append(StringUtils.PadLeftOrTrim(@"Total", cellSize));
                }

                result.Append('\n');
            }

            for (int i = 0; i < counts.Length; i++)
            {
                if (rowLabels != null)
                {
                    string s = (rowLabels[i] == null ? @"null" : rowLabels[i].ToString());
                    s = StringUtils.PadOrTrim(s, labelSize);
                    result.Append(s);
                }

                for (int j = 0; j < counts[i].Length; j++)
                {
                    result.Append(StringUtils.PadLeft(nf.Format(counts[i][j]), cellSize));
                }

                if (printTotals)
                {
                    result.Append(StringUtils.PadLeft(nf.Format(rowTotals[i]), cellSize));
                }

                result.Append('\n');
            }

            if (printTotals)
            {
                result.Append(StringUtils.Pad(@"Total", labelSize));
                foreach (int colTotal in colTotals)
                {
                    result.Append(StringUtils.PadLeft(nf.Format(colTotal), cellSize));
                }

                result.Append(StringUtils.PadLeft(nf.Format(total), cellSize));
            }

            return result.ToString();
        }

        public static string ToString(double[][] counts)
        {
            return ToString(counts, 10, null, null, CultureInfo.CurrentCulture.NumberFormat, false);
        }

        public static string ToString(double[][] counts, int cellSize, Object[] rowLabels, Object[] colLabels, NumberFormatInfo nf, bool printTotals)
        {
            if (counts == null)
                return null;
            double[] rowTotals = new double[counts.Length];
            double[] colTotals = new double[counts[0].Length];
            double total = 0.0;
            for (int i = 0; i < counts.Length; i++)
            {
                for (int j = 0; j < counts[i].Length; j++)
                {
                    rowTotals[i] += counts[i][j];
                    colTotals[j] += counts[i][j];
                    total += counts[i][j];
                }
            }

            StringBuilder result = new StringBuilder();
            if (colLabels != null)
            {
                result.Append(StringUtils.PadLeft(@"", cellSize));
                for (int j = 0; j < counts[0].Length; j++)
                {
                    string s = colLabels[j].ToString();
                    if (s.Length > cellSize - 1)
                    {
                        s = s.Substring(0, cellSize - 1);
                    }

                    s = StringUtils.PadLeft(s, cellSize);
                    result.Append(s);
                }

                if (printTotals)
                {
                    result.Append(StringUtils.PadLeftOrTrim(@"Total", cellSize));
                }

                result.Append('\n');
            }

            for (int i = 0; i < counts.Length; i++)
            {
                if (rowLabels != null)
                {
                    string s = rowLabels[i].ToString();
                    s = StringUtils.PadOrTrim(s, cellSize);
                    result.Append(s);
                }

                for (int j = 0; j < counts[i].Length; j++)
                {
                    result.Append(StringUtils.PadLeft((counts[i][j]).ToString(nf), cellSize));
                }

                if (printTotals)
                {
                    result.Append(StringUtils.PadLeft((rowTotals[i]).ToString(nf), cellSize));
                }

                result.Append('\n');
            }

            if (printTotals)
            {
                result.Append(StringUtils.Pad(@"Total", cellSize));
                foreach (double colTotal in colTotals)
                {
                    result.Append(StringUtils.PadLeft(nf.Format(colTotal), cellSize));
                }

                result.Append(StringUtils.PadLeft(nf.Format(total), cellSize));
            }

            return result.ToString();
        }

        public static string ToString(float[][] counts)
        {
            return ToString(counts, 10, null, null, CultureInfo.CurrentCulture.NumberFormat, false);
        }

        public static string ToString(float[][] counts, int cellSize, Object[] rowLabels, Object[] colLabels, NumberFormatInfo nf, bool printTotals)
        {
            double[] rowTotals = new double[counts.Length];
            double[] colTotals = new double[counts[0].Length];
            double total = 0.0;
            for (int i = 0; i < counts.Length; i++)
            {
                for (int j = 0; j < counts[i].Length; j++)
                {
                    rowTotals[i] += counts[i][j];
                    colTotals[j] += counts[i][j];
                    total += counts[i][j];
                }
            }

            StringBuilder result = new StringBuilder();
            if (colLabels != null)
            {
                result.Append(StringUtils.PadLeft(@"", cellSize));
                for (int j = 0; j < counts[0].Length; j++)
                {
                    string s = colLabels[j].ToString();
                    s = StringUtils.PadLeftOrTrim(s, cellSize);
                    result.Append(s);
                }

                if (printTotals)
                {
                    result.Append(StringUtils.PadLeftOrTrim(@"Total", cellSize));
                }

                result.Append('\n');
            }

            for (int i = 0; i < counts.Length; i++)
            {
                if (rowLabels != null)
                {
                    string s = rowLabels[i].ToString();
                    s = StringUtils.Pad(s, cellSize);
                    result.Append(s);
                }

                for (int j = 0; j < counts[i].Length; j++)
                {
                    result.Append(StringUtils.PadLeft((counts[i][j]).ToString(nf), cellSize));
                }

                if (printTotals)
                {
                    result.Append(StringUtils.PadLeft((rowTotals[i]).ToString(nf), cellSize));
                }

                result.Append('\n');
            }

            if (printTotals)
            {
                result.Append(StringUtils.Pad(@"Total", cellSize));
                foreach (double colTotal in colTotals)
                {
                    result.Append(StringUtils.PadLeft((colTotal).ToString(nf), cellSize));
                }

                result.Append(StringUtils.PadLeft((total).ToString(nf), cellSize));
            }

            return result.ToString();
        }

        public static void Main(String[] args)
        {
            Random random = new Random();
            int length = 100;
            double[] A = new double[length];
            double[] B = new double[length];
            double aAvg = 70.0;
            double bAvg = 70.5;
            for (int i = 0; i < length; i++)
            {
                A[i] = aAvg + random.NextGaussian();
                B[i] = bAvg + random.NextGaussian();
            }

            Console.Out.WriteLine(@"A has length " + A.Length + @" and mean " + Mean(A));
            Console.Out.WriteLine(@"B has length " + B.Length + @" and mean " + Mean(B));
            for (int t = 0; t < 10; t++)
            {
                Console.Out.WriteLine(@"p-value: " + SigLevelByApproxRand(A, B));
            }
        }

        public static int[][] DeepCopy(int[][] counts)
        {
            int[][] result = new int[counts.Length][];
            for (int i = 0; i < counts.Length; i++)
            {
                result[i] = new int[counts[i].Length];
                Array.Copy(counts[i], 0, result[i], 0, counts[i].Length);
            }

            return result;
        }

        public static double[][] Covariance(double[][] data)
        {
            double[] means = new double[data.Length];
            for (int i = 0; i < means.Length; i++)
            {
                means[i] = Mean(data[i]);
            }

            double[][] covariance = new double[means.Length][];
            for (int i = 0; i < data[0].Length; i++)
            {
                for (int j = 0; j < means.Length; j++)
                {
                    covariance[j] = new double[means.Length];
                    for (int k = 0; k < means.Length; k++)
                    {
                        covariance[j][k] += (means[j] - data[j][i]) * (means[k] - data[k][i]);
                    }
                }
            }

            for (int i = 0; i < covariance.Length; i++)
            {
                for (int j = 0; j < covariance[i].Length; j++)
                {
                    covariance[i][j] = System.Math.Sqrt(covariance[i][j]) / (data[0].Length);
                }
            }

            return covariance;
        }

        public static void AddMultInto(double[] a, double[] b, double[] c, double d)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = b[i] + c[i] * d;
            }
        }

        public static void MultiplyInto(double[] a, double[] b, double c)
        {
            for (int i = 0; i < a.Length; i++)
            {
                a[i] = b[i] * c;
            }
        }

        public static double[] CopyOf(double[] original, int newSize)
        {
            double[] a = new double[newSize];
            Array.Copy(original, 0, a, 0, original.Length);
            return a;
        }

        public static void AssertFinite(double[] vector, string vectorName)
        {
            for (int i = 0; i < vector.Length; i++)
            {
                if (Double.IsNaN(vector[i]))
                {
                    throw new InvalidElementException(@"NaN found in " + vectorName + @" element " + i);
                }
                else if (Double.IsInfinity(vector[i]))
                {
                    throw new InvalidElementException(@"Infinity found in " + vectorName + @" element " + i);
                }
            }
        }

        public class InvalidElementException : Exception
        {
            public InvalidElementException(string s)
                : base(s)
            {
            }
        }
    }
}
