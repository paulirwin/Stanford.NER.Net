using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Math
{
    public class DoubleAD // : Number
    {
        private double val;
        private double dot;
        
        public DoubleAD()
        {
            Setval(0);
            Setdot(1);
        }

        public DoubleAD(double initVal, double initDot)
        {
            val = initVal;
            dot = initDot;
        }

        public virtual bool Equals(DoubleAD b)
        {
            if (b.Getval() == val && b.Getdot() == dot)
            {
                return true;
            }
            else
                return false;
        }

        public virtual bool Equals(double valToCompare, double dotToCompare)
        {
            if (valToCompare == val && dotToCompare == dot)
            {
                return true;
            }
            else
                return false;
        }

        public virtual bool Equals(double valToCompare, double dotToCompare, double TOL)
        {
            if (System.Math.Abs(valToCompare - val) < TOL && System.Math.Abs(dotToCompare - dot) < TOL)
            {
                return true;
            }
            else
                return false;
        }

        public virtual double Getval()
        {
            return val;
        }

        public virtual double Getdot()
        {
            return dot;
        }

        public virtual void Set(double value, double dotValue)
        {
            val = value;
            dot = dotValue;
        }

        public virtual void Setval(double a)
        {
            val = a;
        }

        public virtual void Setdot(double a)
        {
            dot = a;
        }

        public virtual void PlusEqualsConst(double a)
        {
            Setval(val + a);
        }

        public virtual void PlusEquals(DoubleAD a)
        {
            Setval(val + a.Getval());
            Setdot(dot + a.Getdot());
        }

        public virtual void MinusEquals(DoubleAD a)
        {
            Setval(val - a.Getval());
            Setdot(dot - a.Getdot());
        }

        public virtual void MinusEqualsConst(double a)
        {
            Setval(val - a);
        }

        public virtual double DoubleValue()
        {
            return Getval();
        }

        public virtual float FloatValue()
        {
            return (float)DoubleValue();
        }

        public virtual int IntValue()
        {
            return (int)DoubleValue();
        }

        public virtual long LongValue()
        {
            return (long)DoubleValue();
        }

        public override string ToString()
        {
            return @"Value= " + val + @"; Dot= " + dot;
        }
    }
}
