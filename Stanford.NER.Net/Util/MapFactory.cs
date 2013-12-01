using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{    
    public abstract class MapFactory<K, V>
        where K : class
    {
        protected MapFactory()
        {
        }

        public static readonly MapFactory<K, V> HASH_MAP_FACTORY = new HashMapFactory<K, V>();
        public static readonly MapFactory<K, V> IDENTITY_HASH_MAP_FACTORY = new IdentityHashMapFactory<K, V>();
        private static readonly MapFactory<K, V> WEAK_HASH_MAP_FACTORY = new WeakHashMapFactory<K, V>();
        private static readonly MapFactory<K, V> TREE_MAP_FACTORY = new TreeMapFactory<K, V>();
        private static readonly MapFactory<K, V> LINKED_HASH_MAP_FACTORY = new LinkedHashMapFactory<K, V>();
        private static readonly MapFactory<K, V> ARRAY_MAP_FACTORY = new ArrayMapFactory<K, V>();

        public static MapFactory<K, V> HashMapFactory()
        {
            return HASH_MAP_FACTORY;
        }

        public static MapFactory<K, V> IdentityHashMapFactory()
        {
            return IDENTITY_HASH_MAP_FACTORY;
        }

        public static MapFactory<K, V> WeakHashMapFactory()
        {
            return WEAK_HASH_MAP_FACTORY;
        }

        public static MapFactory<K, V> TreeMapFactory()
        {
            return TREE_MAP_FACTORY;
        }

        public static MapFactory<K, V> LinkedHashMapFactory()
        {
            return LINKED_HASH_MAP_FACTORY;
        }

        public static MapFactory<K, V> ArrayMapFactory()
        {
            return ARRAY_MAP_FACTORY;
        }

        private class HashMapFactory<K, V> : MapFactory<K, V>
            where K : class
        {
            public override IDictionary<K, V> NewMap()
            {
                return new HashMap<K, V>();
            }

            public override IDictionary<K, V> NewMap(int initCapacity)
            {
                return new HashMap<K, V>();
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map)
            {
                map = new HashMap<K1, V1>();
                return map;
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity)
            {
                map = new HashMap<K1, V1>();
                return map;
            }
        }

        private class IdentityHashMapFactory<K, V> : MapFactory<K, V>
        {
            public override IDictionary<K, V> NewMap()
            {
                return new IdentityHashMap<K, V>();
            }

            public override IDictionary<K, V> NewMap(int initCapacity)
            {
                return new IdentityHashMap<K, V>(initCapacity);
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map)
            {
                map = new IdentityHashMap<K1, V1>();
                return map;
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity)
            {
                map = new IdentityHashMap<K1, V1>(initCapacity);
                return map;
            }
        }

        private class WeakHashMapFactory<K, V> : MapFactory<K, V>
        {
            public override IDictionary<K, V> NewMap()
            {
                return new WeakHashMap<K, V>();
            }

            public override IDictionary<K, V> NewMap(int initCapacity)
            {
                return new WeakHashMap<K, V>(initCapacity);
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map)
            {
                map = new WeakHashMap<K1, V1>();
                return map;
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity)
            {
                map = new WeakHashMap<K1, V1>(initCapacity);
                return map;
            }
        }

        private class TreeMapFactory<K, V> : MapFactory<K, V>
        {
            public override IDictionary<K, V> NewMap()
            {
                return new TreeMap<K, V>();
            }

            public override IDictionary<K, V> NewMap(int initCapacity)
            {
                return NewMap();
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map)
            {
                map = new TreeMap<K1, V1>();
                return map;
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity)
            {
                map = new TreeMap<K1, V1>();
                return map;
            }
        }

        private class LinkedHashMapFactory<K, V> : MapFactory<K, V>
        {
            public override IDictionary<K, V> NewMap()
            {
                return new LinkedHashMap<K, V>();
            }

            public override IDictionary<K, V> NewMap(int initCapacity)
            {
                return NewMap();
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map)
            {
                map = new LinkedHashMap<K1, V1>();
                return map;
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity)
            {
                map = new LinkedHashMap<K1, V1>();
                return map;
            }
        }

        private class ArrayMapFactory<K, V> : MapFactory<K, V>
        {
            public override IDictionary<K, V> NewMap()
            {
                return new ArrayMap<K, V>();
            }

            public override IDictionary<K, V> NewMap(int initCapacity)
            {
                return new ArrayMap<K, V>(initCapacity);
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map)
            {
                return new ArrayMap<K1, V1>();
            }

            public override IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity)
            {
                map = new ArrayMap<K1, V1>(initCapacity);
                return map;
            }
        }

        public abstract IDictionary<K, V> NewMap();
        public abstract IDictionary<K, V> NewMap(int initCapacity);
        public abstract IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map);
        public abstract IDictionary<K1, V1> SetMap<K1, V1>(IDictionary<K1, V1> map, int initCapacity);
    }
}
