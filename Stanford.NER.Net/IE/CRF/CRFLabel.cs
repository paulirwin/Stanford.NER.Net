using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    [Serializable]
    public class CRFLabel
    {
        private readonly int[] label;
        int hashCode = -1;
        private static readonly int maxNumClasses = 10;

        public CRFLabel(int[] label)
        {
            this.label = label;
        }

        public override bool Equals(Object o)
        {
            if (!(o is CRFLabel))
            {
                return false;
            }

            CRFLabel other = (CRFLabel)o;
            if (other.label.Length != label.Length)
            {
                return false;
            }

            for (int i = 0; i < label.Length; i++)
            {
                if (label[i] != other.label[i])
                {
                    return false;
                }
            }

            return true;
        }

        public virtual CRFLabel GetSmallerLabel(int size)
        {
            int[] newLabel = new int[size];
            Array.Copy(label, label.Length - size, newLabel, 0, size);
            return new CRFLabel(newLabel);
        }

        public virtual CRFLabel GetOneSmallerLabel()
        {
            return GetSmallerLabel(label.Length - 1);
        }

        public virtual int[] GetLabel()
        {
            return label;
        }

        public virtual string ToString<E>(IIndex<E> classIndex)
        {
            IList<E> l = new List<E>();
            for (int i = 0; i < label.Length; i++)
            {
                l.Add(classIndex.Get(label[i]));
            }

            return l.ToString();
        }

        public override string ToString()
        {
            IList<int> l = new List<int>();
            for (int i = 0; i < label.Length; i++)
            {
                l.Add(label[i]);
            }

            return l.ToString();
        }

        public override int GetHashCode()
        {
            if (hashCode < 0)
            {
                hashCode = 0;
                for (int i = 0; i < label.Length; i++)
                {
                    hashCode *= maxNumClasses;
                    hashCode += label[i];
                }
            }

            return hashCode;
        }
    }
}
