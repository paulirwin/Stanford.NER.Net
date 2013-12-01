using Stanford.NER.Net.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class Beam<T> : AbstractSet<T>
    {
        protected int maxBeamSize;
        protected IHeap<T> elements;

        public virtual int Capacity()
        {
            return maxBeamSize;
        }

        public override int Count
        {
            get { return elements.Size(); }
        }

        public override IEnumerator<T> Iterator()
        {
            return AsSortedList().GetEnumerator();
        }

        public virtual List<T> AsSortedList()
        {
            List<T> list = new List<T>();
            foreach (var i in elements)
            {
                list.Insert(0, i);
            }

            return list;
        }

        public override bool Add(T o)
        {
            bool added = true;
            elements.Add(o);
            while (Count > Capacity())
            {
                Object dumped = elements.ExtractMin();
                if (dumped.Equals(o))
                {
                    added = false;
                }
            }

            return added;
        }

        public override bool Remove(T o)
        {
            throw new NotSupportedException();
        }

        public Beam()
            : this(100)
        {
        }

        public Beam(int maxBeamSize)
            : this(maxBeamSize, new ScoredComparerWrapper(ScoredComparator.ASCENDING_COMPARATOR))
        {
        }

        public Beam(int maxBeamSize, IComparer<IScored> cmp)
            : this(maxBeamSize, new ScoredComparerWrapper(cmp))
        {

        }

        public Beam(int maxBeamSize, IComparer<T> cmp)
        {
            elements = new ArrayHeap<T>(cmp);
            this.maxBeamSize = maxBeamSize;
        }

        private class ScoredComparerWrapper : IComparer<T>
        {
            private readonly IComparer<IScored> _comp;

            public ScoredComparerWrapper(IComparer<IScored> comp)
            {
                _comp = comp;
            }

            public int Compare(T x, T y)
            {
                return _comp.Compare((IScored)x, (IScored)y);
            }
        }
    }
}
