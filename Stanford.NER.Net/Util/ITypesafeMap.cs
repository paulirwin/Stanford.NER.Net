using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stanford.NER.Net.Util
{
    public interface ITypesafeMap
    {
        bool Has<T, VALUE>()
            where T : TypesafeMap.Key<VALUE>;

        VALUE Get<T, VALUE>()
            where T : TypesafeMap.Key<VALUE>;

        VALUE Set<T, VALUE>(VALUE value)
            where T : TypesafeMap.Key<VALUE>;

        VALUE Remove<T, VALUE>()
            where T : TypesafeMap.Key<VALUE>;

        ISet<Type> KeySet();

        bool ContainsKey<T, VALUE>()
            where T : TypesafeMap.Key<VALUE>;

        int Size();
    }

    public static class TypesafeMap
    {
        public interface Key<VALUE> { }
    }
}
