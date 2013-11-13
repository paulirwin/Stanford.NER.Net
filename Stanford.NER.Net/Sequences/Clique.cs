using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public class Clique
    {
        //private static readonly long serialVersionUID = -@"8109637472035159453L";
        
        private int[] relativeIndices;

        protected static IDictionary<CliqueEqualityWrapper, Clique> interner = new HashMap<CliqueEqualityWrapper, Clique>();
        
        private class CliqueEqualityWrapper
        {
            private Clique c;
            public CliqueEqualityWrapper(Clique c)
            {
                this.c = c;
            }

            public override bool Equals(Object o)
            {
                if (!(o is CliqueEqualityWrapper))
                {
                    return false;
                }

                CliqueEqualityWrapper otherC = (CliqueEqualityWrapper)o;
                if (otherC.c.relativeIndices.Length != c.relativeIndices.Length)
                {
                    return false;
                }

                for (int i = 0; i < c.relativeIndices.Length; i++)
                {
                    if (c.relativeIndices[i] != otherC.c.relativeIndices[i])
                    {
                        return false;
                    }
                }

                return true;
            }

            public override int GetHashCode()
            {
                int h = 1;
                foreach (int i in c.relativeIndices)
                {
                    h *= 17;
                    h += i;
                }

                return h;
            }
        }

        private static Clique Intern(Clique c)
        {
            CliqueEqualityWrapper wrapper = new CliqueEqualityWrapper(c);
            Clique newC = interner.Get(wrapper);
            if (newC == null)
            {
                interner.Put(wrapper, c);
                newC = c;
            }

            return newC;
        }

        private Clique()
        {
        }

        public static Clique ValueOf(int maxLeft, int maxRight)
        {
            int[] ri = new int[-maxLeft + maxRight + 1];
            int j = maxLeft;
            for (int i = 0; i < ri.Length; i++)
            {
                ri[i] = j++;
            }

            return ValueOfHelper(ri);
        }

        public static Clique ValueOf(int[] relativeIndices)
        {
            CheckSorted(relativeIndices);
            return ValueOfHelper(ArrayUtils.Copy(relativeIndices));
        }

        public static Clique ValueOf(Clique c, int offset)
        {
            int[] ri = new int[c.relativeIndices.Length];
            for (int i = 0; i < ri.Length; i++)
            {
                ri[i] = c.relativeIndices[i] + offset;
            }

            return ValueOfHelper(ri);
        }

        private static Clique ValueOfHelper(int[] relativeIndices)
        {
            Clique c = new Clique();
            c.relativeIndices = relativeIndices;
            return Intern(c);
        }

        private static void CheckSorted(int[] sorted)
        {
            for (int i = 0; i < sorted.Length - 1; i++)
            {
                if (sorted[i] > sorted[i + 1])
                {
                    throw new Exception(@"input must be sorted!");
                }
            }
        }

        public virtual int MaxLeft()
        {
            return relativeIndices[0];
        }

        public virtual int MaxRight()
        {
            return relativeIndices[relativeIndices.Length - 1];
        }

        public virtual int Size()
        {
            return relativeIndices.Length;
        }

        public virtual int RelativeIndex(int i)
        {
            return relativeIndices[i];
        }

        public virtual int IndexOfRelativeIndex(int relativeIndex)
        {
            for (int i = 0; i < relativeIndices.Length; i++)
            {
                if (relativeIndices[i] == relativeIndex)
                {
                    return i;
                }
            }

            return -1;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"[");
            for (int i = 0; i < relativeIndices.Length; i++)
            {
                sb.Append(relativeIndices[i]);
                if (i != relativeIndices.Length - 1)
                {
                    sb.Append(@", ");
                }
            }

            sb.Append(@"]");
            return sb.ToString();
        }

        public virtual Clique LeftMessage()
        {
            int[] ri = new int[relativeIndices.Length - 1];
            Array.Copy(relativeIndices, 0, ri, 0, ri.Length);
            return ValueOfHelper(ri);
        }

        public virtual Clique RightMessage()
        {
            int[] ri = new int[relativeIndices.Length - 1];
            Array.Copy(relativeIndices, 1, ri, 0, ri.Length);
            return ValueOfHelper(ri);
        }

        public virtual Clique Shift(int shiftAmount)
        {
            if (shiftAmount == 0)
            {
                return this;
            }

            int[] ri = new int[relativeIndices.Length];
            for (int i = 0; i < ri.Length; i++)
            {
                ri[i] = relativeIndices[i] + shiftAmount;
            }

            return ValueOfHelper(ri);
        }

        private int hashCode = -1;
        public override int GetHashCode()
        {
            if (hashCode == -1)
            {
                hashCode = ToString().GetHashCode();
            }

            return hashCode;
        }

        protected virtual Object ReadResolve()
        {
            return Intern(this);
        }
    }
}
