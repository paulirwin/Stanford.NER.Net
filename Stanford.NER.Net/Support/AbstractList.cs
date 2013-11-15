using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public abstract class AbstractList<T> : IList<T>
    {
        private readonly IEqualityComparer<T> _eqComparer;

        public AbstractList()
        {
            _eqComparer = EqualityComparer<T>.Default;
        }

        public abstract T Get(int index);

        public abstract int Size();

        public virtual void Set(int index, T value)
        {
            throw new NotImplementedException();
        }

        public virtual void Add(int index, T value)
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(int index)
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            for (int i = 0; i < Size(); i++)
            {
                if (_eqComparer.Equals(Get(i), item))
                    return i;
            }

            return -1;
        }

        public void Insert(int index, T item)
        {
            Add(index, item);
        }

        public void RemoveAt(int index)
        {
            Remove(index);
        }

        public T this[int index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Set(index, value);
            }
        }

        public void Add(T item)
        {
            Add(Size(), item);
        }

        public void Clear()
        {
            for (int i = Size() - 1; i >= 0; i--)
            {
                Remove(i);
            }
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return Size(); }
        }

        public virtual bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            int index = IndexOf(item);

            if (index == -1)
                return false;

            Remove(index);

            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return GetEnumerable().GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IEnumerable<T> GetEnumerable()
        {
            for (int i = 0; i < Size(); i++)
            {
                yield return Get(i);
            }
        }
    }
}
