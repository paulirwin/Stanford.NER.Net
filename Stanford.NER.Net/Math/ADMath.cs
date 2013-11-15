using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Math
{
    public static class ADMath
    {
        public static DoubleAD Mult(DoubleAD a, DoubleAD b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() * b.Getval());
            c.Setdot(a.Getdot() * b.Getval() + b.Getdot() * a.Getval());
            return c;
        }

        public static DoubleAD MultConst(DoubleAD a, double b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() * b);
            c.Setdot(a.Getdot() * b);
            return c;
        }

        public static DoubleAD Divide(DoubleAD a, DoubleAD b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() / b.Getval());
            c.Setdot((a.Getdot() / b.Getval()) - a.Getval() * b.Getdot() / (b.Getval() * b.Getval()));
            return c;
        }

        public static DoubleAD DivideConst(DoubleAD a, double b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() / b);
            c.Setdot(a.Getdot() / b);
            return c;
        }

        public static DoubleAD Exp(DoubleAD a)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(System.Math.Exp(a.Getval()));
            c.Setdot(a.Getdot() * System.Math.Exp(a.Getval()));
            return c;
        }

        public static DoubleAD Log(DoubleAD a)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(System.Math.Log(a.Getval()));
            c.Setdot(a.Getdot() / a.Getval());
            return c;
        }

        public static DoubleAD Plus(DoubleAD a, DoubleAD b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() + b.Getval());
            c.Setdot(a.Getdot() + b.Getdot());
            return c;
        }

        public static DoubleAD PlusConst(DoubleAD a, double b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() + b);
            c.Setdot(a.Getdot());
            return c;
        }

        public static DoubleAD Minus(DoubleAD a, DoubleAD b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() - b.Getval());
            c.Setdot(a.Getdot() - b.Getdot());
            return c;
        }

        public static DoubleAD MinusConst(DoubleAD a, double b)
        {
            DoubleAD c = new DoubleAD();
            c.Setval(a.Getval() - b);
            c.Setdot(a.Getdot());
            return c;
        }

        public static DoubleAD LogSum(DoubleAD[] logInputs)
        {
            return LogSum(logInputs, 0, logInputs.Length);
        }

        public static DoubleAD LogSum(DoubleAD[] logInputs, int fromIndex, int toIndex)
        {
            if (logInputs.Length == 0)
                throw new ArgumentException();
            if (fromIndex >= 0 && toIndex < logInputs.Length && fromIndex >= toIndex)
                return new DoubleAD(Double.NegativeInfinity, Double.NegativeInfinity);
            int maxIdx = fromIndex;
            double max = logInputs[fromIndex].Getval();
            double maxdot = logInputs[fromIndex].Getdot();
            for (int i = fromIndex + 1; i < toIndex; i++)
            {
                if (logInputs[i].Getval() > max)
                {
                    maxIdx = i;
                    maxdot = logInputs[i].Getdot();
                    max = logInputs[i].Getval();
                }
            }

            DoubleAD ret = new DoubleAD();
            bool haveTerms = false;
            double intermediate = 0.0;
            double intermediateDot = 0.0;
            double cutoff = max - SloppyMath.LOGTOLERANCE;
            for (int i = fromIndex; i < toIndex; i++)
            {
                if (i != maxIdx && logInputs[i].Getval() > cutoff)
                {
                    haveTerms = true;
                    double curEXP = System.Math.Exp(logInputs[i].Getval() - max);
                    intermediate += curEXP;
                    intermediateDot += curEXP * logInputs[i].Getdot();
                }
            }

            if (haveTerms)
            {
                ret.Setval(max + System.Math.Log(1.0 + intermediate));
                ret.Setdot((maxdot + intermediateDot) / (1.0 + intermediate));
            }
            else
            {
                ret.Setval(max);
                ret.Setdot(maxdot);
            }

            return ret;
        }
    }
}
