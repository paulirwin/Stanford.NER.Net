using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public interface IIndex<E> : IEnumerable<E>
    {
        int Size();

        E Get(int i);

        int IndexOf(E o);

        int IndexOf(E o, bool add);

        IList<E> ObjectsList();

        ICollection<E> Objects(int[] indices);

        bool IsLocked();

        void Lock();

        void Unlock();

        void SaveToWriter(TextWriter output);

        void SaveToFilename(string s);

        bool Contains(Object o);

        T[] ToArray<T>(T[] a);

        bool Add(E e);

        bool AddAll(ICollection<E> c);

        void Clear();
    }
}
