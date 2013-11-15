using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public class HashMap<TKey, TValue> : IDictionary<TKey, TValue>
        where TKey : class
    {
        private readonly IEqualityComparer<TValue> _valueEqComp = EqualityComparer<TValue>.Default;
        private readonly IDictionary<TKey, TValue> _dict;
        private TValue _nullKeyValue;
        private bool _hasNullKey;

        public HashMap()
        {
            _dict = new Dictionary<TKey, TValue>();
        }

        public HashMap(IDictionary<TKey, TValue> dict)
        {
            _dict = dict;
        }

        public void Add(TKey key, TValue value)
        {
            if (key == null)
            {
                _nullKeyValue = value;
                _hasNullKey = true;
            }
            else
                _dict.Add(key, value);
        }

        public bool ContainsKey(TKey key)
        {
            if (key == null)
                return _hasNullKey;

            return _dict.ContainsKey(key);
        }

        public ICollection<TKey> Keys
        {
            get { return _dict.Keys; }
        }

        public bool Remove(TKey key)
        {
            if (key == null)
            {
                if (!_hasNullKey)
                    return false;

                _hasNullKey = false;
                _nullKeyValue = default(TValue);
                return true;
            }

            return _dict.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            if (key == null)
            {
                if (!_hasNullKey)
                {
                    value = default(TValue);
                    return false;
                }

                value = _nullKeyValue;
                return true;
            }

            return _dict.TryGetValue(key, out value);
        }

        public ICollection<TValue> Values
        {
            get 
            {
                if (_hasNullKey)
                    return _dict.Values.Concat(new[] { _nullKeyValue }).ToList();

                return _dict.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                if (key == null)
                {
                    if (_hasNullKey)
                        return _nullKeyValue;
                    return default(TValue);
                }

                TValue value;

                if (_dict.TryGetValue(key, out value))
                    return value;

                return default(TValue);
            }
            set
            {
                Add(key, value);
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _hasNullKey = false;
            _nullKeyValue = default(TValue);
            _dict.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            if (item.Key == null)
                return _valueEqComp.Equals(_nullKeyValue, item.Value);

            return _dict.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (_hasNullKey)
                array[arrayIndex++] = new KeyValuePair<TKey,TValue>(null, _nullKeyValue);

            ((IDictionary<TKey, TValue>)_dict).CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _dict.Count + (_hasNullKey ? 1 : 0); }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return Remove(item.Key);
        }

        public IEnumerable<KeyValuePair<TKey, TValue>> GetEnumerable()
        {
            if (_hasNullKey)
                yield return new KeyValuePair<TKey, TValue>(null, _nullKeyValue);

            foreach (var kvp in _dict)
            {
                yield return kvp;
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public TValue Get(TKey key)
        {
            return this[key];
        }

        public void Put(TKey key, TValue value)
        {
            this[key] = value;
        }
    }
}
