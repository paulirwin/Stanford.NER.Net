using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public sealed class MutableDouble : IComparable<MutableDouble>
    {
        private double d;
        public void Set(double d)
        {
            this.d = d;
        }

        public override int GetHashCode()
        {
            long bits = BitConverter.DoubleToInt64Bits(d);
            return (int)(bits ^ (bits >> 32));
        }

        public override bool Equals(Object obj)
        {
            return obj is MutableDouble && d == ((MutableDouble)obj).d;
        }

        public override string ToString()
        {
            return d.ToString();
        }

        public int CompareTo(MutableDouble anotherMutableDouble)
        {
            double thisVal = this.d;
            double anotherVal = anotherMutableDouble.d;
            return (thisVal < anotherVal ? -1 : (thisVal == anotherVal ? 0 : 1));
        }

        public int IntValue()
        {
            return (int)d;
        }

        public long LongValue()
        {
            return (long)d;
        }

        public short ShortValue()
        {
            return (short)d;
        }

        public byte ByteValue()
        {
            return (byte)d;
        }

        public float FloatValue()
        {
            return (float)d;
        }

        public double DoubleValue()
        {
            return d;
        }

        public MutableDouble()
            : this(0.0)
        {
        }

        public MutableDouble(double d)
        {
            this.d = d;
        }

        //public MutableDouble(Number num)
        //{
        //    this.d = num.DoubleValue();
        //}
    }
}
