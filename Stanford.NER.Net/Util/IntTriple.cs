using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class IntTriple : IntTuple
    {
        public IntTriple()
            : base(3)
        {
        }

        public IntTriple(int src, int mid, int trgt)
            : base(3)
        {
            elements[0] = src;
            elements[1] = mid;
            elements[2] = trgt;
        }

        public override IntTuple GetCopy()
        {
            IntTriple nT = new IntTriple(elements[0], elements[1], elements[2]);
            return nT;
        }

        public virtual int GetSource()
        {
            return elements[0];
        }

        public virtual int GetTarget()
        {
            return elements[2];
        }

        public virtual int GetMiddle()
        {
            return elements[1];
        }
    }
}
