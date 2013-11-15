using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Math
{
    public static class SloppyMath
    {
        public static double Round(double x)
        {
            return System.Math.Floor(x + 0.5);
        }

        public static double Round(double x, int precision)
        {
            double power = System.Math.Pow(10.0, precision);
            return Round(x * power) / power;
        }

        public static int Max(int a, int b, int c)
        {
            int ma;
            ma = a;
            if (b > ma)
            {
                ma = b;
            }

            if (c > ma)
            {
                ma = c;
            }

            return ma;
        }

        public static int Max(ICollection<int> vals)
        {
            if (vals.Count == 0)
            {
                throw new Exception();
            }

            int max = int.MinValue;
            foreach (int i in vals)
            {
                if (i > max)
                {
                    max = i;
                }
            }

            return max;
        }

        public static int Max(int a, int b)
        {
            return (a >= b) ? a : b;
        }

        public static float Max(float a, float b)
        {
            return (a >= b) ? a : b;
        }

        public static double Max(double a, double b)
        {
            return (a >= b) ? a : b;
        }

        public static int Min(int a, int b, int c)
        {
            int mi;
            mi = a;
            if (b < mi)
            {
                mi = b;
            }

            if (c < mi)
            {
                mi = c;
            }

            return mi;
        }

        public static float Min(float a, float b)
        {
            return (a <= b) ? a : b;
        }

        public static double Min(double a, double b)
        {
            return (a <= b) ? a : b;
        }

        public static double Lgamma(double x)
        {
            double[] cof = new[]
        {
        76.2, -86.5, 24.0, -1.2, 0.0, -0.0
        }

            ;
            double xxx = x;
            double tmp = x + 5.5;
            tmp -= ((x + 0.5) * System.Math.Log(tmp));
            double ser = 1.0;
            for (int j = 0; j < 6; j++)
            {
                xxx++;
                ser += cof[j] / xxx;
            }

            return -tmp + System.Math.Log(2.5 * ser / x);
        }

        public static bool IsDangerous(double d)
        {
            return Double.IsInfinity(d) || Double.IsNaN(d) || d == 0.0;
        }

        public static bool IsVeryDangerous(double d)
        {
            return Double.IsInfinity(d) || Double.IsNaN(d);
        }

        public static bool IsCloseTo(double a, double b)
        {
            if (a > b)
            {
                return (a - b) < 0.0;
            }
            else
            {
                return (b - a) < 0.0;
            }
        }

        internal static readonly double LOGTOLERANCE = 30.0;
        internal static readonly float LOGTOLERANCE_F = 20F;
        public static double Gamma(double n)
        {
            return System.Math.Sqrt(2.0 * System.Math.PI / n) * System.Math.Pow((n / System.Math.E) * System.Math.Sqrt(n * System.Math.Sinh((1.0 / n) + (1 / 810 * System.Math.Pow(n, 6)))), n);
        }

        public static double Log(double num, double base_renamed)
        {
            return System.Math.Log(num) / System.Math.Log(base_renamed);
        }

        public static float LogAdd(float lx, float ly)
        {
            float max, negDiff;
            if (lx > ly)
            {
                max = lx;
                negDiff = ly - lx;
            }
            else
            {
                max = ly;
                negDiff = lx - ly;
            }

            if (max == Double.NegativeInfinity)
            {
                return max;
            }
            else if (negDiff < -LOGTOLERANCE_F)
            {
                return max;
            }
            else
            {
                return max + (float)System.Math.Log(1.0 + System.Math.Exp(negDiff));
            }
        }

        public static double LogAdd(double lx, double ly)
        {
            double max, negDiff;
            if (lx > ly)
            {
                max = lx;
                negDiff = ly - lx;
            }
            else
            {
                max = ly;
                negDiff = lx - ly;
            }

            if (max == Double.NegativeInfinity)
            {
                return max;
            }
            else if (negDiff < -LOGTOLERANCE)
            {
                return max;
            }
            else
            {
                return max + System.Math.Log(1.0 + System.Math.Exp(negDiff));
            }
        }

        public static int NChooseK(int n, int k)
        {
            k = System.Math.Min(k, n - k);
            if (k == 0)
            {
                return 1;
            }

            int accum = n;
            for (int i = 1; i < k; i++)
            {
                accum *= (n - i);
                accum /= i;
            }

            return accum / k;
        }

        public static double Pow(double a, double b)
        {
            int x = (int)(BitConverter.DoubleToInt64Bits(a) >> 32);
            int y = (int)(b * (x - 1072632447) + 1072632447);
            return BitConverter.Int64BitsToDouble(((long)y) << 32);
        }

        public static int IntPow(int b, int e)
        {
            switch (e)
            {
                case 0:
                    return 1;
                case 1:
                    return b;
                case 2:
                    return b * b;
                default:
                    int result = 1;
                    int currPow = b;
                    while (e > 0)
                    {
                        if ((e & 1) != 0)
                        {
                            result *= currPow;
                        }

                        currPow *= currPow;
                        e >>= 1;
                    }

                    return result;
            }
        }

        public static float IntPow(float b, int e)
        {
            float result = 1F;
            float currPow = b;
            while (e > 0)
            {
                if ((e & 1) != 0)
                {
                    result *= currPow;
                }

                currPow *= currPow;
                e >>= 1;
            }

            return result;
        }

        public static double IntPow(double b, int e)
        {
            double result = 1.0;
            double currPow = b;
            while (e > 0)
            {
                if ((e & 1) != 0)
                {
                    result *= currPow;
                }

                currPow *= currPow;
                e >>= 1;
            }

            return result;
        }

        public static double Hypergeometric(int k, int n, int r, int m)
        {
            if (k < 0 || r > n || m > n || n <= 0 || m < 0 || r < 0)
            {
                throw new ArgumentException(@"Invalid hypergeometric");
            }

            if (m > n / 2)
            {
                m = n - m;
                k = r - k;
            }

            if (r > n / 2)
            {
                r = n - r;
                k = m - k;
            }

            if (m > r)
            {
                int temp = m;
                m = r;
                r = temp;
            }

            if (k < (m + r) - n || k > m)
            {
                return 0.0;
            }

            if (r == n)
            {
                if (k == m)
                {
                    return 1.0;
                }
                else
                {
                    return 0.0;
                }
            }
            else if (r == n - 1)
            {
                if (k == m)
                {
                    return (n - m) / (double)n;
                }
                else if (k == m - 1)
                {
                    return m / (double)n;
                }
                else
                {
                    return 0.0;
                }
            }
            else if (m == 1)
            {
                if (k == 0)
                {
                    return (n - r) / (double)n;
                }
                else if (k == 1)
                {
                    return r / (double)n;
                }
                else
                {
                    return 0.0;
                }
            }
            else if (m == 0)
            {
                if (k == 0)
                {
                    return 1.0;
                }
                else
                {
                    return 0.0;
                }
            }
            else if (k == 0)
            {
                double ans = 1.0;
                for (int m0 = 0; m0 < m; m0++)
                {
                    ans *= ((n - r) - m0);
                    ans /= (n - m0);
                }

                return ans;
            }

            double ans2 = 1.0;
            for (int nr = n - r, n0 = n; nr > (n - r) - (m - k); nr--, n0--)
            {
                ans2 *= nr;
                ans2 /= n0;
            }

            for (int k0 = 0; k0 < k; k0++)
            {
                ans2 *= (m - k0);
                ans2 /= ((n - (m - k0)) + 1);
                ans2 *= (r - k0);
                ans2 /= (k0 + 1);
            }

            return ans2;
        }

        public static double ExactBinomial(int k, int n, double p)
        {
            double total = 0.0;
            for (int m = k; m <= n; m++)
            {
                double nChooseM = 1.0;
                for (int r = 1; r <= m; r++)
                {
                    nChooseM *= (n - r) + 1;
                    nChooseM /= r;
                }

                total += nChooseM * System.Math.Pow(p, m) * System.Math.Pow(1.0 - p, n - m);
            }

            return total;
        }

        public static double OneTailedFishersExact(int k, int n, int r, int m)
        {
            if (k < 0 || k < (m + r) - n || k > r || k > m || r > n || m > n)
            {
                throw new ArgumentException(@"Invalid Fisher's exact: " + @"k=" + k + @" n=" + n + @" r=" + r + @" m=" + m + @" k<0=" + (k < 0) + @" k<(m+r)-n=" + (k < (m + r) - n) + @" k>r=" + (k > r) + @" k>m=" + (k > m) + @" r>n=" + (r > n) + @"m>n=" + (m > n));
            }

            if (m > n / 2)
            {
                m = n - m;
                k = r - k;
            }

            if (r > n / 2)
            {
                r = n - r;
                k = m - k;
            }

            if (m > r)
            {
                int temp = m;
                m = r;
                r = temp;
            }

            double total = 0.0;
            if (k > m / 2)
            {
                for (int k0 = k; k0 <= m; k0++)
                {
                    total += SloppyMath.Hypergeometric(k0, n, r, m);
                }
            }
            else
            {
                int min = System.Math.Max(0, (m + r) - n);
                for (int k0 = min; k0 < k; k0++)
                {
                    total += SloppyMath.Hypergeometric(k0, n, r, m);
                }

                total = 1.0 - total;
            }

            return total;
        }

        public static double ChiSquare2by2(int k, int n, int r, int m)
        {
            int[][] cg = new[] {
                new[] { k, r - k }, 
                new[] { m - k, n - (k + (r - k) + (m - k)) } 
            };

            int[] cgr = new[] { r, n - r };
            int[] cgc = new[] { m, n - m };

            double total = 0.0;
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    double exp = (double)cgr[i] * cgc[j] / n;
                    total += (cg[i][j] - exp) * (cg[i][j] - exp) / exp;
                }
            }

            return total;
        }

        public static double Sigmoid(double x)
        {
            if (x < 0)
            {
                double num = System.Math.Exp(x);
                return num / (1.0 + num);
            }
            else
            {
                double den = 1.0 + System.Math.Exp(-x);
                return 1.0 / den;
            }
        }

        public static double Poisson(int x, double lambda)
        {
            if (x < 0 || lambda <= 0.0)
                throw new Exception(@"Bad arguments: " + x + @" and " + lambda);
            double p = (System.Math.Exp(-lambda) * System.Math.Pow(lambda, x)) / Factorial(x);
            if (Double.IsInfinity(p) || p <= 0.0)
                throw new Exception(System.Math.Exp(-lambda) + @" " + System.Math.Pow(lambda, x) + @" " + Factorial(x));
            return p;
        }

        public static double Factorial(int x)
        {
            double result = 1.0;
            for (int i = x; i > 1; i--)
            {
                result *= i;
            }

            return result;
        }

        public static void Main(String[] args)
        {
            if (args.Length == 0)
            {
                Console.Error.WriteLine(@"Usage: java edu.stanford.nlp.math.SloppyMath " + @"[-logAdd|-fishers k n r m|-binomial r n p");
            }
            else if (args[0].Equals(@"-logAdd"))
            {
                Console.Out.WriteLine(@"Log adds of neg infinity numbers, etc.");
                Console.Out.WriteLine(@"(logs) -Inf + -Inf = " + LogAdd(Double.NegativeInfinity, Double.NegativeInfinity));
                Console.Out.WriteLine(@"(logs) -Inf + -7 = " + LogAdd(Double.NegativeInfinity, -7.0));
                Console.Out.WriteLine(@"(logs) -7 + -Inf = " + LogAdd(-7.0, Double.NegativeInfinity));
                Console.Out.WriteLine(@"(logs) -50 + -7 = " + LogAdd(-50.0, -7.0));
                Console.Out.WriteLine(@"(logs) -11 + -7 = " + LogAdd(-11.0, -7.0));
                Console.Out.WriteLine(@"(logs) -7 + -11 = " + LogAdd(-7.0, -11.0));
                Console.Out.WriteLine(@"real 1/2 + 1/2 = " + LogAdd(System.Math.Log(0.5), System.Math.Log(0.5)));
            }
            else if (args[0].Equals(@"-fishers"))
            {
                int k = int.Parse(args[1]);
                int n = int.Parse(args[2]);
                int r = int.Parse(args[3]);
                int m = int.Parse(args[4]);
                double ans = SloppyMath.Hypergeometric(k, n, r, m);
                Console.Out.WriteLine(@"hypg(" + k + @"; " + n + @", " + r + @", " + m + @") = " + ans);
                ans = SloppyMath.OneTailedFishersExact(k, n, r, m);
                Console.Out.WriteLine(@"1-tailed Fisher's exact(" + k + @"; " + n + @", " + r + @", " + m + @") = " + ans);
                double ansChi = SloppyMath.ChiSquare2by2(k, n, r, m);
                Console.Out.WriteLine(@"chiSquare(" + k + @"; " + n + @", " + r + @", " + m + @") = " + ansChi);
                Console.Out.WriteLine(@"Swapping arguments should give same hypg:");
                ans = SloppyMath.Hypergeometric(k, n, r, m);
                Console.Out.WriteLine(@"hypg(" + k + @"; " + n + @", " + m + @", " + r + @") = " + ans);
                int othrow = n - m;
                int othcol = n - r;
                int cell12 = m - k;
                int cell21 = r - k;
                int cell22 = othrow - (r - k);
                ans = SloppyMath.Hypergeometric(cell12, n, othcol, m);
                Console.Out.WriteLine(@"hypg(" + cell12 + @"; " + n + @", " + othcol + @", " + m + @") = " + ans);
                ans = SloppyMath.Hypergeometric(cell21, n, r, othrow);
                Console.Out.WriteLine(@"hypg(" + cell21 + @"; " + n + @", " + r + @", " + othrow + @") = " + ans);
                ans = SloppyMath.Hypergeometric(cell22, n, othcol, othrow);
                Console.Out.WriteLine(@"hypg(" + cell22 + @"; " + n + @", " + othcol + @", " + othrow + @") = " + ans);
            }
            else if (args[0].Equals(@"-binomial"))
            {
                int k = int.Parse(args[1]);
                int n = int.Parse(args[2]);
                double p = Double.Parse(args[3]);
                double ans = SloppyMath.ExactBinomial(k, n, p);
                Console.Out.WriteLine(@"Binomial p(X >= " + k + @"; " + n + @", " + p + @") = " + ans);
            }
            else
            {
                Console.Error.WriteLine(@"Unknown option: " + args[0]);
            }
        }
    }
}
