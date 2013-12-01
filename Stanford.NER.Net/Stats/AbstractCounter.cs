using Stanford.NER.Net.Math;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Stats
{
    public abstract class AbstractCounter<E> : ICounter<E>
    {
        public virtual double LogIncrementCount(E key, double amount)
        {
            double count = SloppyMath.LogAdd(GetCount(key), amount);
            SetCount(key, count);
            return GetCount(key);
        }

        public virtual double IncrementCount(E key, double amount)
        {
            double count = GetCount(key) + amount;
            SetCount(key, count);
            return GetCount(key);
        }

        public virtual double IncrementCount(E key)
        {
            return IncrementCount(key, 1.0);
        }

        public virtual double DecrementCount(E key, double amount)
        {
            return IncrementCount(key, -amount);
        }

        public virtual double DecrementCount(E key)
        {
            return IncrementCount(key, -1.0);
        }

        public virtual void AddAll(ICounter<E> counter)
        {
            Counters.AddInPlace(this, counter);
        }

        public abstract IFactory<ICounter<E>> GetFactory();

        public abstract void SetDefaultReturnValue(double rv);

        public abstract double DefaultReturnValue();

        public abstract double GetCount(object key);

        public abstract void SetCount(E key, double value);

        public abstract double Remove(E key);

        public abstract bool ContainsKey(E key);

        public abstract ISet<E> KeySet();

        public abstract ICollection<double> Values();

        public abstract ISet<KeyValuePair<E, double>> EntrySet();

        public abstract void Clear();

        public abstract int Size();

        public abstract double TotalCount();
    }
}
