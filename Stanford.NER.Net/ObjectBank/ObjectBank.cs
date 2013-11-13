using Stanford.NER.Net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.ObjectBank
{
    public class ObjectBank<E> : ICollection<E>
    {
        public ObjectBank(ReaderIteratorFactory rif, IteratorFromReaderFactory<E> ifrf)
        {
            this.rif = rif;
            this.ifrf = ifrf;
        }

        protected ReaderIteratorFactory rif;
        protected IteratorFromReaderFactory<E> ifrf;
        private List<E> contents;

        public static ObjectBank<String> GetLineIterator(string filename)
        {
            return GetLineIterator(new FileInfo(filename));
        }

        public static ObjectBank<X> GetLineIterator<X>(string filename, IFunction<String, X> op)
        {
            return GetLineIterator(new FileInfo(filename), op);
        }

        public static ObjectBank<String> GetLineIterator(string filename, string encoding)
        {
            return GetLineIterator(new FileInfo(filename), encoding);
        }

        public static ObjectBank<String> GetLineIterator(TextReader reader)
        {
            return GetLineIterator(reader, new IdentityFunction<String>());
        }

        public static ObjectBank<X> GetLineIterator<X>(TextReader reader, IFunction<String, X> op)
        {
            ReaderIteratorFactory rif = new ReaderIteratorFactory(reader);
            IteratorFromReaderFactory<X> ifrf = LineIterator.GetFactory(op);
            return new ObjectBank<X>(rif, ifrf);
        }

        public static ObjectBank<String> GetLineIterator(FileInfo file)
        {
            return GetLineIterator(new[] { file }, new IdentityFunction<String>());
        }

        public static ObjectBank<X> GetLineIterator<X>(FileInfo file, IFunction<String, X> op)
        {
            return GetLineIterator(new[] { file }, op);
        }

        public static ObjectBank<String> GetLineIterator(FileInfo file, string encoding)
        {
            return GetLineIterator(file, new IdentityFunction<String>(), encoding);
        }

        public static ObjectBank<X> GetLineIterator<X>(FileInfo file, IFunction<String, X> op, string encoding)
        {
            ReaderIteratorFactory rif = new ReaderIteratorFactory(file, encoding);
            IteratorFromReaderFactory<X> ifrf = LineIterator.GetFactory(op);
            return new ObjectBank<X>(rif, ifrf);
        }

        public static ObjectBank<X> GetLineIterator<X>(ICollection filesStringsAndReaders, IFunction<String, X> op)
        {
            ReaderIteratorFactory rif = new ReaderIteratorFactory(filesStringsAndReaders);
            IteratorFromReaderFactory<X> ifrf = LineIterator.GetFactory(op);
            return new ObjectBank<X>(rif, ifrf);
        }

        public static ObjectBank<String> GetLineIterator(ICollection filesStringsAndReaders, string encoding)
        {
            return GetLineIterator(filesStringsAndReaders, new IdentityFunction<String>(), encoding);
        }

        public static ObjectBank<X> GetLineIterator<X>(ICollection filesStringsAndReaders, IFunction<String, X> op, string encoding)
        {
            ReaderIteratorFactory rif = new ReaderIteratorFactory(filesStringsAndReaders, encoding);
            IteratorFromReaderFactory<X> ifrf = LineIterator.GetFactory(op);
            return new ObjectBank<X>(rif, ifrf);
        }

        public class PathToFileFunction : IFunction<String, FileInfo>
        {
            public override FileInfo Apply(string str)
            {
                return new FileInfo(str);
            }
        }

        public IEnumerator<E> GetEnumerator()
        {
            if (keepInMemory)
            {
                if (contents == null)
                {
                    contents = new List<E>();
                    IEnumerator<E> iter = new OBIterator();
                    while (iter.HasNext())
                    {
                        contents.Add(iter.Next());
                    }
                }

                return contents.GetEnumerator();
            }

            return new OBIterator();
        }

        private bool keepInMemory;
        public virtual void KeepInMemory(bool keep)
        {
            keepInMemory = keep;
        }

        public virtual void ClearMemory()
        {
            contents = null;
        }

        public override bool IsEmpty()
        {
            return !Iterator().HasNext();
        }

        public override bool Contains(Object o)
        {
            IEnumerator<E> iter = GetEnumerator();
            while (iter.HasNext())
            {
                if (iter.Next() == o)
                {
                    return true;
                }
            }

            return false;
        }

        public override bool ContainsAll(ICollection c)
        {
            foreach (Object obj in c)
            {
                if (!Contains(obj))
                {
                    return false;
                }
            }

            return true;
        }

        public override int Size()
        {
            Iterator<E> iter = Iterator();
            int size = 0;
            while (iter.HasNext())
            {
                size++;
                iter.Next();
            }

            return size;
        }

        public override void Clear()
        {
            rif = new ReaderIteratorFactory();
        }

        public virtual Object[] ToArray()
        {
            Iterator<E> iter = Iterator();
            List<Object> al = new List<Object>();
            while (iter.HasNext())
            {
                al.Add(iter.Next());
            }

            return al.ToArray();
        }

        public override T[] ToArray<T>(T[] o)
        {
            Iterator<E> iter = Iterator();
            List<E> al = new List<E>();
            while (iter.HasNext())
            {
                al.Add(iter.Next());
            }

            return al.ToArray(o);
        }

        public override bool Add(E o)
        {
            throw new NotSupportedException();
        }

        public override bool Remove(Object o)
        {
            throw new NotSupportedException();
        }

        public override bool AddAll(ICollection<E> c)
        {
            throw new NotSupportedException();
        }

        public override bool RemoveAll(ICollection c)
        {
            throw new NotSupportedException();
        }

        public override bool RetainAll(ICollection c)
        {
            throw new NotSupportedException();
        }

        class OBIterator : AbstractIterator<E>
        {
            private readonly IEnumerator<TextReader> readerIterator;
            private Iterator<E> tok;
            private E nextObject;
            private TextReader currReader;
            public OBIterator()
            {
                readerIterator = rif.Iterator();
                SetNextObject();
            }

            private void SetNextObject()
            {
                if (tok != null && tok.HasNext())
                {
                    nextObject = tok.Next();
                    return;
                }

                while (true)
                {
                    try
                    {
                        if (currReader != null)
                        {
                            currReader.Close();
                        }
                    }
                    catch (IOException e)
                    {
                        //throw new Exception(e);
                    }

                    if (readerIterator.HasNext())
                    {
                        currReader = readerIterator.Next();
                        tok = ifrf.GetIterator(currReader);
                    }
                    else
                    {
                        nextObject = null;
                        return;
                    }

                    if (tok.HasNext())
                    {
                        nextObject = tok.Next();
                        return;
                    }
                }
            }

            public override bool HasNext()
            {
                return nextObject != null;
            }

            public override E Next()
            {
                if (nextObject == null)
                {
                    throw new NoSuchElementException();
                }

                E tmp = nextObject;
                SetNextObject();
                return tmp;
            }
        }

        //private static readonly long serialVersionUID = -@"4030295596701541770L";
    }
}
