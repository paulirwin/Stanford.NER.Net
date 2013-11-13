using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class PaddedList<E> : Collection<E>
        where E : class
    {
        private readonly E padding;

        public virtual E GetPad()
        {
            return padding;
        }

        public int Size()
        {
            return base.Items.Count;
        }
        
        public E Get(int i)
        {
            if (i < 0 || i >= Size())
            {
                return padding;
            }

            return base.Items[i];
        }

        public override string ToString()
        {
            return base.Items.ToString();
        }

        public PaddedList(IList<E> l)
            : this(l, null)
        {
        }

        public PaddedList(IList<E> l, E padding)
            : base(l)
        {
            this.padding = padding;
        }

        public virtual IList<E> GetWrappedList()
        {
            return base.Items;
        }

        public static PaddedList<IN> ValueOf<IN>(IList<IN> list, IN padding)
            where IN : class
        {
            return new PaddedList<IN>(list, padding);
        }

        public virtual bool SameInnerList(PaddedList<E> p)
        {
            return p != null && base.Items == p.Items;
        }

        //private static readonly long serialVersionUID = @"2064775966439971729L";
    }
}
