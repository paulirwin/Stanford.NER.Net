using Stanford.NER.Net.Util.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class Quadruple<T1, T2, T3, T4> : IComparable<Quadruple<T1, T2, T3, T4>>, IPrettyLoggable
    {
        public T1 first;
        public T2 second;
        public T3 third;
        public T4 fourth;
        public Quadruple(T1 first, T2 second, T3 third, T4 fourth)
        {
            this.first = first;
            this.second = second;
            this.third = third;
            this.fourth = fourth;
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

        public virtual T4 Fourth()
        {
            return fourth;
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

        public virtual void SetFourth(T4 o)
        {
            fourth = o;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }

            if (!(o is Quadruple<T1, T2, T3, T4>))
            {
                return false;
            }

            Quadruple<T1, T2, T3, T4> quadruple = (Quadruple<T1, T2, T3, T4>)o;
            if (first != null ? !first.Equals(quadruple.first) : quadruple.first != null)
            {
                return false;
            }

            if (second != null ? !second.Equals(quadruple.second) : quadruple.second != null)
            {
                return false;
            }

            if (third != null ? !third.Equals(quadruple.third) : quadruple.third != null)
            {
                return false;
            }

            if (fourth != null ? !fourth.Equals(quadruple.fourth) : quadruple.fourth != null)
            {
                return false;
            }

            return true;
        }

        public override int GetHashCode()
        {
            int result = 17;
            result = (first != null ? first.GetHashCode() : 0);
            result = 29 * result + (second != null ? second.GetHashCode() : 0);
            result = 29 * result + (third != null ? third.GetHashCode() : 0);
            result = 29 * result + (fourth != null ? fourth.GetHashCode() : 0);
            return result;
        }

        public override string ToString()
        {
            return @"(" + first + @"," + second + @"," + third + @"," + fourth + @")";
        }

        public static Quadruple<T1, T2, T3, T4> MakeQuadruple(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            return new Quadruple<T1, T2, T3, T4>(t1, t2, t3, t4);
        }

        public virtual IList<Object> AsList()
        {
            return new List<object> { first, second, third, fourth };
        }

        public override int CompareTo(Quadruple<T1, T2, T3, T4> another)
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
                    comp = ((IComparable<T3>)Third()).CompareTo(another.Third());
                    if (comp != 0)
                    {
                        return comp;
                    }
                    else
                    {
                        return ((IComparable<T4>)Fourth()).CompareTo(another.Fourth());
                    }
                }
            }
        }

        public virtual void PrettyLog(string description)
        {
            //PrettyLogger.Log(channels, description, this.AsList());
        }
    }
}
