using Stanford.NER.Net.Util.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public abstract class Pair
    {
        protected object first;
        protected object second;
    }

    public class Pair<T1, T2> : Pair, IComparable<Pair<T1, T2>>, IPrettyLoggable
    {
        public Pair()
        {
        }

        public Pair(T1 first, T2 second)
        {
            this.first = first;
            this.second = second;
        }

        public virtual T1 First()
        {
            return (T1)first;
        }

        public virtual T2 Second()
        {
            return (T2)second;
        }

        public virtual void SetFirst(T1 o)
        {
            first = o;
        }

        public virtual void SetSecond(T2 o)
        {
            second = o;
        }

        public override string ToString()
        {
            return @"(" + first + @"," + second + @")";
        }

        public override bool Equals(Object o)
        {
            if (o is Pair<T1, T2>)
            {
                var p = (Pair<T1, T2>)o;
                return (first == null ? p.First() == null : first.Equals(p.First())) && (second == null ? p.Second() == null : second.Equals(p.Second()));
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            int firstHash = (first == null ? 0 : first.GetHashCode());
            int secondHash = (second == null ? 0 : second.GetHashCode());
            return firstHash * 31 + secondHash;
        }

        public virtual IList<Object> AsList()
        {
            return new List<object> { first, second };
        }

        public static Pair<String, String> ReadStringPair(BinaryReader in_renamed)
        {
            Pair<String, String> p = new Pair<String, String>();
            try
            {
                p.first = in_renamed.ReadString();
                p.second = in_renamed.ReadString();
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }

            return p;
        }

        public static Pair<X, Y> MakePair<X, Y>(X x, Y y)
        {
            return new Pair<X, Y>(x, y);
        }

        public virtual void Save(BinaryWriter out_renamed)
        {
            try
            {
                out_renamed.Write(first.ToString());
                out_renamed.Write(second.ToString());
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }
        }

        public virtual int CompareTo(Pair<T1, T2> another)
        {
            int comp = ((IComparable<T1>)First()).CompareTo(another.First());
            if (comp != 0)
            {
                return comp;
            }
            else
            {
                return ((IComparable<T2>)Second()).CompareTo(another.Second());
            }
        }

        public static Pair<String, String> StringIntern(Pair<String, String> p)
        {
            return new MutableInternedPair(p);
        }

        public static Pair<String, String> InternedStringPair(string first, string second)
        {
            return new MutableInternedPair(first, second);
        }

        public class MutableInternedPair : Pair<String, String>
        {
            internal MutableInternedPair(Pair<String, String> p)
                : base((string)p.first, (string)p.second)
            {
                InternStrings();
            }

            internal MutableInternedPair(string first, string second)
                : base(first, second)
            {
                InternStrings();
            }

            protected virtual Object ReadResolve()
            {
                InternStrings();
                return this;
            }

            private void InternStrings()
            {
                if (first != null)
                {
                    first = string.Intern((string)first);
                }

                if (second != null)
                {
                    second = string.Intern((string)second);
                }
            }

        }

        public virtual void PrettyLog(string description)
        {
            //PrettyLogger.Log(channels, description, this.AsList());
        }

        public class ByFirstPairComparator<T1, T2> : IComparer<Pair<T1, T2>>
        {
            public override int Compare(Pair<T1, T2> pair1, Pair<T1, T2> pair2)
            {
                return ((IComparable<T1>)pair1.First()).CompareTo(pair2.First());
            }
        }

        public class ByFirstReversePairComparator<T1, T2> : IComparer<Pair<T1, T2>>
        {
            public override int Compare(Pair<T1, T2> pair1, Pair<T1, T2> pair2)
            {
                return -((IComparable<T1>)pair1.First()).CompareTo(pair2.First());
            }
        }

        public class BySecondPairComparator<T1, T2> : IComparer<Pair<T1, T2>>
        {
            public override int Compare(Pair<T1, T2> pair1, Pair<T1, T2> pair2)
            {
                return ((IComparable<T2>)pair1.Second()).CompareTo(pair2.Second());
            }
        }

        public class BySecondReversePairComparator<T1, T2> : IComparer<Pair<T1, T2>>
        {
            public override int Compare(Pair<T1, T2> pair1, Pair<T1, T2> pair2)
            {
                return -((IComparable<T2>)pair1.Second()).CompareTo(pair2.Second());
            }
        }
    }
}
