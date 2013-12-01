using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class IntUni : IntTuple
    {
        public IntUni()
            : base(1)
        {
        }

        public IntUni(int src)
            : base(1)
        {
            elements[0] = src;
        }

        public virtual int GetSource()
        {
            return elements[0];
        }

        public virtual void SetSource(int src)
        {
            elements[0] = src;
        }

        public override IntTuple GetCopy()
        {
            IntUni nT = new IntUni(elements[0]);
            return nT;
        }

        public virtual void Add(int val)
        {
            elements[0] += val;
        }
    }
}
