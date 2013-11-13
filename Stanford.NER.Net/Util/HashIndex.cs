using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class HashIndex<E> : IIndex<E>, IRandomAccess
    {
        List<E> objects = new List<E>();
        IDictionary<E, int?> indexes = Generics.NewHashMap();
        bool locked;
        public override void Clear()
        {
            objects.Clear();
            indexes.Clear();
        }

        public virtual int[] Indices(ICollection<E> elems)
        {
            int[] indices = new int[elems.Size()];
            int i = 0;
            foreach (E elem in elems)
            {
                indices[i++] = IndexOf(elem);
            }

            return indices;
        }

        public virtual ICollection<E> Objects(int[] indices)
        {
            return new AnonymousAbstractList(this);
        }

        private sealed class AnonymousAbstractList : AbstractList
        {
            public AnonymousAbstractList(HashIndex parent)
            {
                this.parent = parent;
            }

            private readonly HashIndex parent;
            public override E Get(int index)
            {
                return objects.Get(indices[index]);
            }

            public override int Size()
            {
                return indices.length;
            }
        }

        public override int Size()
        {
            return objects.Size();
        }

        public virtual E Get(int i)
        {
            if (i < 0 || i >= objects.Size())
                throw new ArrayIndexOutOfBoundsException(@"Index " + i + @" outside the bounds [0," + Size() + @")");
            return objects.Get(i);
        }

        public virtual List<E> ObjectsList()
        {
            return objects;
        }

        public virtual bool IsLocked()
        {
            return locked;
        }

        public virtual void Lock()
        {
            locked = true;
        }

        public virtual void Unlock()
        {
            locked = false;
        }

        public virtual int IndexOf(E o)
        {
            return IndexOf(o, false);
        }

        public virtual int IndexOf(E o, bool add)
        {
            int index = indexes.Get(o);
            if (index == null)
            {
                if (add && !locked)
                {
                    try
                    {
                        semaphore.Acquire();
                        index = indexes.Get(o);
                        if (index == null)
                        {
                            index = objects.Size();
                            objects.Add(o);
                            indexes.Put(o, index);
                        }

                        semaphore.Release();
                    }
                    catch (InterruptedException e)
                    {
                        throw new Exception(e);
                    }
                }
                else
                {
                    return -1;
                }
            }

            return index;
        }

        private readonly Semaphore semaphore = new Semaphore(1);
        public override bool AddAll(ICollection<E> c)
        {
            bool changed = false;
            foreach (E element in c)
            {
                changed |= Add(element);
            }

            return changed;
        }

        public override bool Add(E o)
        {
            int index = indexes.Get(o);
            if (index == null && !locked)
            {
                index = objects.Size();
                objects.Add(o);
                indexes.Put(o, index);
                return true;
            }

            return false;
        }

        public override bool Contains(Object o)
        {
            return indexes.ContainsKey(o);
        }

        public HashIndex()
            : base()
        {
        }

        public HashIndex(int capacity)
            : base()
        {
            objects = new ArrayList<E>(capacity);
            indexes = Generics.NewHashMap(capacity);
        }

        public HashIndex(ICollection<E> c)
            : this()
        {
            AddAll(c);
        }

        public HashIndex(IIndex<E> index)
            : this()
        {
            AddAll(index.ObjectsList());
        }

        public virtual void SaveToFilename(string file)
        {
            BufferedWriter bw = null;
            try
            {
                bw = new BufferedWriter(new FileWriter(file));
                for (int i = 0, sz = size(); i < sz; i++)
                {
                    bw.Write(i + @"=" + Get(i) + '\n');
                }

                bw.Close();
            }
            catch (IOException e)
            {
                //e.PrintStackTrace();
            }
            finally
            {
                if (bw != null)
                {
                    try
                    {
                        bw.Close();
                    }
                    catch (IOException ioe)
                    {
                    }
                }
            }
        }

        public static IIndex<String> LoadFromFilename(string file)
        {
            IIndex<String> index = new HashIndex<String>();
            BufferedReader br = null;
            try
            {
                br = new BufferedReader(new FileReader(file));
                for (string line; (line = br.ReadLine()) != null; )
                {
                    int start = line.IndexOf('=');
                    if (start == -1 || start == line.Length - 1)
                    {
                        continue;
                    }

                    index.Add(line.Substring(start + 1));
                }

                br.Close();
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }
            finally
            {
                if (br != null)
                {
                    try
                    {
                        br.Close();
                    }
                    catch (IOException ioe)
                    {
                    }
                }
            }

            return index;
        }

        public virtual void SaveToWriter(Writer bw)
        {
            for (int i = 0, sz = size(); i < sz; i++)
            {
                bw.Write(i + @"=" + Get(i) + '\n');
            }
        }

        public static IIndex<String> LoadFromReader(BufferedReader br)
        {
            HashIndex<String> index = new HashIndex<String>();
            string line = br.ReadLine();
            while ((line != null) && (line.Length > 0))
            {
                int start = line.IndexOf('=');
                if (start == -1 || start == line.Length - 1)
                {
                    continue;
                }

                index.Add(line.Substring(start + 1));
                line = br.ReadLine();
            }

            return index;
        }

        public override string ToString()
        {
            return ToString(int.MaxValue);
        }

        public virtual string ToStringOneEntryPerLine()
        {
            return ToStringOneEntryPerLine(int.MaxValue);
        }

        public virtual string ToString(int n)
        {
            StringBuilder buff = new StringBuilder(@"[");
            int sz = objects.Size();
            if (n > sz)
            {
                n = sz;
            }

            int i;
            for (i = 0; i < n; i++)
            {
                E e = objects.Get(i);
                buff.Append(i).Append('=').Append(e);
                if (i < (sz - 1))
                    buff.Append(',');
            }

            if (i < sz)
                buff.Append(@"...");
            buff.Append(']');
            return buff.ToString();
        }

        public virtual string ToStringOneEntryPerLine(int n)
        {
            StringBuilder buff = new StringBuilder();
            int sz = objects.Size();
            if (n > sz)
            {
                n = sz;
            }

            int i;
            for (i = 0; i < n; i++)
            {
                E e = objects.Get(i);
                buff.Append(e);
                if (i < (sz - 1))
                    buff.Append('\n');
            }

            if (i < sz)
                buff.Append(@"...");
            return buff.ToString();
        }

        public override Iterator<E> Iterator()
        {
            return objects.Iterator();
        }

        public virtual HashIndex<E> UnmodifiableView()
        {
            HashIndex<E> newIndex = new AnonymousHashIndex(this);
            newIndex.objects = objects;
            newIndex.indexes = indexes;
            newIndex.Lock();
            return newIndex;
        }

        private sealed class AnonymousHashIndex : HashIndex
        {
            public AnonymousHashIndex(HashIndex parent)
            {
                this.parent = parent;
            }

            private readonly HashIndex parent;
            public override void Unlock()
            {
                throw new NotSupportedException(@"This is an unmodifiable view!");
            }

        }

        public override bool Remove(Object o)
        {
            throw new NotSupportedException();
        }

        public override bool RemoveAll(ICollection<E> e)
        {
            throw new NotSupportedException();
        }

        public static IIndex<String> LoadFromFileWithList(string file)
        {
            IIndex<String> index = new HashIndex<String>();
            BufferedReader br = null;
            try
            {
                br = new BufferedReader(new FileReader(file));
                for (string line; (line = br.ReadLine()) != null; )
                {
                    index.Add(line.Trim());
                }

                br.Close();
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }
            finally
            {
                if (br != null)
                {
                    try
                    {
                        br.Close();
                    }
                    catch (IOException ioe)
                    {
                    }
                }
            }

            return index;
        }
    }
}
