using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stanford.NER.Net.Util
{
    public interface ITypesafeMap
    {
        bool Has(Type key);

        VALUE Get<VALUE>(Type key);

        VALUE Set<VALUE>(Type key, VALUE value);

        VALUE Remove<VALUE>(Type key);

        ISet<Type> KeySet();

        bool ContainsKey(Type key);

        int Size();
    }

    public static class TypesafeMap
    {
        public interface Key<VALUE> { }
    }
}
