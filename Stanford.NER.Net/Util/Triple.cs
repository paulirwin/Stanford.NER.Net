using Stanford.NER.Net.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class Triple<T1, T2, T3> : IComparable<Triple<T1, T2, T3>>, IPrettyLoggable
    {
        public T1 first;
        public T2 second;
        public T3 third;

        public Triple(T1 first, T2 second, T3 third)
        {
            this.first = first;
            this.second = second;
            this.third = third;
        }

        public virtual T1 First()
        {
            return first;
        }

        public virtual T2 Second()
        {
            return second;
        }

        public virtual T3 Third()
        {
            return third;
        }

        public virtual void SetFirst(T1 o)
        {
            first = o;
        }

        public virtual void SetSecond(T2 o)
        {
            second = o;
        }

        public virtual void SetThird(T3 o)
        {
            third = o;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }

            if (!(o is Triple<T1, T2, T3>))
            {
                return false;
            }

            Triple<T1, T2, T3> triple = (Triple<T1, T2, T3>)o;
            if (first != null ? !first.Equals(triple.first) : triple.first != null)
            {
                return false;
            }

            if (second != null ? !second.Equals(triple.second) : triple.second != null)
            {
                return false;
            }

            if (third != null ? !third.Equals(triple.third) : triple.third != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result;
            result = (first != null ? first.GetHashCode() : 0);
            result = 29 * result + (second != null ? second.GetHashCode() : 0);
            result = 29 * result + (third != null ? third.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return @"(" + first + @"," + second + @"," + third + @")";
        }

        public virtual IList<Object> AsList()
        {
            return new List<object> { first, second, third };
        }

        public static Triple<X, Y, Z> MakeTriple<X, Y, Z>(X x, Y y, Z z)
        {
            return new Triple<X, Y, Z>(x, y, z);
        }

        public virtual void PrettyLog(string description)
        {

        }

        public override int CompareTo(Triple<T1, T2, T3> another)
        {
            int comp = ((IComparable<T1>)First()).CompareTo(another.First());
            if (comp != 0)
            {
                return comp;
            }
            else
            {
                comp = ((IComparable<T2>)Second()).CompareTo(another.Second());
                if (comp != 0)
                {
                    return comp;
                }
                else
                {
                    return ((IComparable<T3>)Third()).CompareTo(another.Third());
                }
            }
        }
    }
}
