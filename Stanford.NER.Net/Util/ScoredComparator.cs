using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public sealed class ScoredComparator : IComparer<IScored>
    {
        private static readonly bool ASCENDING = true;
        private static readonly bool DESCENDING = false;
        public static readonly ScoredComparator ASCENDING_COMPARATOR = new ScoredComparator(ASCENDING);
        public static readonly ScoredComparator DESCENDING_COMPARATOR = new ScoredComparator(DESCENDING);
        private readonly bool ascending;
        private ScoredComparator(bool ascending)
        {
            this.ascending = ascending;
        }

        public int Compare(IScored o1, IScored o2)
        {
            if (o1 == o2)
            {
                return 0;
            }

            double d1 = o1.Score();
            double d2 = o2.Score();
            if (ascending)
            {
                if (d1 < d2)
                {
                    return -1;
                }

                if (d1 > d2)
                {
                    return 1;
                }
            }
            else
            {
                if (d1 < d2)
                {
                    return 1;
                }

                if (d1 > d2)
                {
                    return -1;
                }
            }

            return 0;
        }

        public override bool Equals(Object o)
        {
            if (o is ScoredComparator)
            {
                ScoredComparator sc = (ScoredComparator)o;
                if (ascending == sc.ascending)
                {
                    return true;
                }
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (ascending)
            {
                return (1 << 23);
            }
            else
            {
                return (1 << 23) + 1;
            }
        }

        public override string ToString()
        {
            return @"ScoredComparator(" + (ascending ? @"ascending" : @"descending") + @")";
        }
    }
}
