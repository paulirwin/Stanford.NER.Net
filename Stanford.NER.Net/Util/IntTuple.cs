using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class IntTuple : IComparable<IntTuple>
    {
        protected readonly int[] elements;
        
        public IntTuple(int[] arr)
        {
            elements = arr;
        }

        public IntTuple(int num)
        {
            elements = new int[num];
        }

        public override int CompareTo(IntTuple o)
        {
            int commonLen = System.Math.Min(o.Length(), Length());
            for (int i = 0; i < commonLen; i++)
            {
                int a = Get(i);
                int b = o.Get(i);
                if (a < b)
                    return -1;
                if (b < a)
                    return 1;
            }

            if (o.Length() == Length())
            {
                return 0;
            }
            else
            {
                return (Length() < o.Length()) ? -1 : 1;
            }
        }

        public virtual int Get(int num)
        {
            return elements[num];
        }

        public virtual void Set(int num, int val)
        {
            elements[num] = val;
        }

        public virtual void ShiftLeft()
        {
            Array.Copy(elements, 1, elements, 0, elements.Length - 1);
            elements[elements.Length - 1] = 0;
        }

        public virtual IntTuple GetCopy()
        {
            IntTuple copy = IntTuple.GetIntTuple(elements.Length);
            Array.Copy(elements, 0, copy.elements, 0, elements.Length);
            return copy;
        }

        public virtual int[] Elems()
        {
            return elements;
        }

        public override bool Equals(Object iO)
        {
            if (!(iO is IntTuple))
            {
                return false;
            }

            IntTuple i = (IntTuple)iO;
            if (i.elements.Length != elements.Length)
            {
                return false;
            }

            for (int j = 0; j < elements.Length; j++)
            {
                if (elements[j] != i.Get(j))
                {
                    return false;
                }
            }

            return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;
            foreach (int element in elements)
            {
                sum = sum * 17 + element;
            }

            return sum;
        }

        public virtual int Length()
        {
            return elements.Length;
        }

        public static IntTuple GetIntTuple(int num)
        {
            if (num == 1)
            {
                return new IntUni();
            }

            if ((num == 2))
            {
                return new IntPair();
            }

            if (num == 3)
            {
                return new IntTriple();
            }

            if (num == 4)
            {
                return new IntQuadruple();
            }
            else
            {
                return new IntTuple(num);
            }
        }

        public static IntTuple GetIntTuple(IList<int> integers)
        {
            IntTuple t = IntTuple.GetIntTuple(integers.Count);
            for (int i = 0; i < t.Length(); i++)
            {
                t.Set(i, integers[i]);
            }

            return t;
        }

        public override string ToString()
        {
            StringBuilder name = new StringBuilder();
            for (int i = 0; i < elements.Length; i++)
            {
                name.Append(Get(i));
                if (i < elements.Length - 1)
                {
                    name.Append(' ');
                }
            }

            return name.ToString();
        }

        public static IntTuple Concat(IntTuple t1, IntTuple t2)
        {
            int n1 = t1.Length();
            int n2 = t2.Length();
            IntTuple res = IntTuple.GetIntTuple(n1 + n2);
            for (int j = 0; j < n1; j++)
            {
                res.Set(j, t1.Get(j));
            }

            for (int i = 0; i < n2; i++)
            {
                res.Set(n1 + i, t2.Get(i));
            }

            return res;
        }

        public virtual void Print()
        {
            string s = ToString();
            Console.Out.Write(s);
        }
    }
}
