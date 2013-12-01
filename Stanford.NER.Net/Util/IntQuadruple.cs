using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class IntQuadruple : IntTuple
    {
        public IntQuadruple()
            : base(4)
        {
        }

        public IntQuadruple(int src, int mid, int trgt, int trgt2)
            : base(4)
        {
            elements[0] = src;
            elements[1] = mid;
            elements[2] = trgt;
            elements[3] = trgt2;
        }

        public override IntTuple GetCopy()
        {
            IntQuadruple nT = new IntQuadruple(elements[0], elements[1], elements[2], elements[3]);
            return nT;
        }

        public virtual int GetSource()
        {
            return Get(0);
        }

        public virtual int GetMiddle()
        {
            return Get(1);
        }

        public virtual int GetTarget()
        {
            return Get(2);
        }

        public virtual int GetTarget2()
        {
            return Get(3);
        }
    }
}
