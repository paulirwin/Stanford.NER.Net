using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Stats
{
    public interface ICounter<E>
    {
        IFactory<ICounter<E>> GetFactory();
        void SetDefaultReturnValue(double rv);
        double DefaultReturnValue();
        double GetCount(Object key);
        void SetCount(E key, double value);
        double IncrementCount(E key, double value);
        double IncrementCount(E key);
        double DecrementCount(E key, double value);
        double DecrementCount(E key);
        double LogIncrementCount(E key, double value);
        void AddAll(ICounter<E> counter);
        double Remove(E key);
        bool ContainsKey(E key);
        ISet<E> KeySet();
        ICollection<Double> Values();
        ISet<KeyValuePair<E, Double>> EntrySet();
        void Clear();
        int Size();
        double TotalCount();
    }
}
