using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public static class Collections
    {
        public static IList<T> SingletonList<T>(this T obj)
        {
            return new[] { obj };
        }

        public static ISet<TKey> NewSetFromMap<TKey, TValue>(IDictionary<TKey, TValue> map)
        {
            return new SetFromMap<TKey, TValue>(map);
        }

        public static void Reverse<T>(this List<T> coll)
        {
            coll.Reverse();
        }

        public static IDictionary<TKey, TValue> EmptyMap<TKey, TValue>()
        {
            // TODO: is it worth making this a singleton value?
            return new Dictionary<TKey, TValue>();
        }

        private class SetFromMap<TKey, TValue> : ISet<TKey>
        {
            private readonly IDictionary<TKey, TValue> _dict;

            public SetFromMap(IDictionary<TKey, TValue> dict)
            {
                _dict = dict;
            }

            public bool Add(TKey item)
            {
                _dict.Add(item, default(TValue));
                return true;
            }

            public void ExceptWith(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public void IntersectWith(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSubsetOf(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public bool IsProperSupersetOf(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSubsetOf(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public bool IsSupersetOf(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public bool Overlaps(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public bool SetEquals(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public void SymmetricExceptWith(IEnumerable<TKey> other)
            {
                throw new NotImplementedException();
            }

            public void UnionWith(IEnumerable<TKey> other)
            {
                foreach (var item in other)
                {
                    Add(item);
                }
            }

            void ICollection<TKey>.Add(TKey item)
            {
                Add(item);
            }

            public void Clear()
            {
                _dict.Clear();
            }

            public bool Contains(TKey item)
            {
                return _dict.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex)
            {
                var keys = _dict.Keys.ToList();

                for (int i = 0; i < keys.Count; i++)
                {
                    array[arrayIndex++] = keys[i];
                }
            }

            public int Count
            {
                get { return _dict.Count; }
            }

            public bool IsReadOnly
            {
                get { return false; }
            }

            public bool Remove(TKey item)
            {
                return _dict.Remove(item);
            }

            public IEnumerator<TKey> GetEnumerator()
            {
                return _dict.Keys.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
