using Stanford.NER.Net.Math;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Stats
{
    public class ClassicCounter<E> : ICounter<E>, IEnumerable<E>
    {
        IDictionary<E, MutableDouble> map;
        private MapFactory<E, MutableDouble> mapFactory;
        private double totalCount;
        private double defaultValue;
        private MutableDouble tempMDouble;
        public ClassicCounter()
            : this(MapFactory<E, MutableDouble>.HashMapFactory())
        {
        }

        public ClassicCounter(int initialCapacity)
            : this(MapFactory<E, MutableDouble>.HashMapFactory(), initialCapacity)
        {
        }

        public ClassicCounter(MapFactory<E, MutableDouble> mapFactory)
        {
            this.mapFactory = mapFactory;
            this.map = mapFactory.NewMap();
        }

        public ClassicCounter(MapFactory<E, MutableDouble> mapFactory, int initialCapacity)
        {
            this.mapFactory = mapFactory;
            this.map = mapFactory.NewMap(initialCapacity);
        }

        public ClassicCounter(ICounter<E> c)
            : this()
        {
            Counters.AddInPlace(this, c);
            SetDefaultReturnValue(c.DefaultReturnValue());
        }

        public ClassicCounter(ICollection<E> collection)
            : this()
        {
            foreach (E key in collection)
            {
                IncrementCount(key);
            }
        }

        virtual MapFactory<E, MutableDouble> GetMapFactory()
        {
            return mapFactory;
        }

        public IFactory<ICounter<E>> GetFactory()
        {
            return new ClassicCounterFactory<E>(GetMapFactory());
        }

        private class ClassicCounterFactory<E> : IFactory<ICounter<E>>
        {
            private readonly MapFactory<E, MutableDouble> mf;
            private ClassicCounterFactory(MapFactory<E, MutableDouble> mf)
            {
                this.mf = mf;
            }

            public ICounter<E> Create()
            {
                return new ClassicCounter<E>(mf);
            }
        }

        public void SetDefaultReturnValue(double rv)
        {
            defaultValue = rv;
        }

        public double DefaultReturnValue()
        {
            return defaultValue;
        }

        public double GetCount(Object key)
        {
            Number count = map.Get(key);
            if (count == null)
            {
                return defaultValue;
            }

            return count.DoubleValue();
        }

        public void SetCount(E key, double count)
        {
            if (tempMDouble == null)
            {
                tempMDouble = new MutableDouble();
            }

            tempMDouble.Set(count);
            tempMDouble = map.Put(key, tempMDouble);
            totalCount += count;
            if (tempMDouble != null)
            {
                totalCount -= tempMDouble.DoubleValue();
            }
        }

        public double IncrementCount(E key, double count)
        {
            if (tempMDouble == null)
            {
                tempMDouble = new MutableDouble();
            }

            MutableDouble oldMDouble = map.Put(key, tempMDouble);
            totalCount += count;
            if (oldMDouble != null)
            {
                count += oldMDouble.DoubleValue();
            }

            tempMDouble.Set(count);
            tempMDouble = oldMDouble;
            return count;
        }

        public double IncrementCount(E key)
        {
            return IncrementCount(key, 1.0);
        }

        public double DecrementCount(E key, double count)
        {
            return IncrementCount(key, -count);
        }

        public double DecrementCount(E key)
        {
            return IncrementCount(key, -1.0);
        }

        public double LogIncrementCount(E key, double count)
        {
            if (tempMDouble == null)
            {
                tempMDouble = new MutableDouble();
            }

            MutableDouble oldMDouble = map.Put(key, tempMDouble);
            if (oldMDouble != null)
            {
                count = SloppyMath.LogAdd(count, oldMDouble.DoubleValue());
                totalCount += count - oldMDouble.DoubleValue();
            }
            else
            {
                totalCount += count;
            }

            tempMDouble.Set(count);
            tempMDouble = oldMDouble;
            return count;
        }

        public void AddAll(ICounter<E> counter)
        {
            Counters.AddInPlace(this, counter);
        }

        public double Remove(E key)
        {
            MutableDouble d = MutableRemove(key);
            if (d != null)
            {
                return d.DoubleValue();
            }

            return defaultValue;
        }

        public bool ContainsKey(E key)
        {
            return map.ContainsKey(key);
        }

        public ICollection<E> KeySet()
        {
            return map.KeySet();
        }

        public ICollection<Double> Values()
        {
            return new AnonymousAbstractCollection(this);
        }

        private sealed class AnonymousIterator : Iterator
        {
            public AnonymousIterator(ClassicCounterFactory parent)
            {
                this.parent = parent;
            }

            private readonly ClassicCounterFactory parent;
            Iterator<MutableDouble> inner = map.Values().Iterator();
            public override bool HasNext()
            {
                return inner.HasNext();
            }

            public override Double Next()
            {
                return Double.ValueOf(inner.Next().DoubleValue());
            }

            public override void Remove()
            {
                throw new NotSupportedException();
            }
        }

        private sealed class AnonymousAbstractCollection : AbstractCollection
        {
            public AnonymousAbstractCollection(ClassicCounterFactory parent)
            {
                this.parent = parent;
            }

            private readonly ClassicCounterFactory parent;
            public override Iterator<Double> Iterator()
            {
                return new AnonymousIterator(this);
            }

            public override int Size()
            {
                return map.Size();
            }

            public override bool Contains(Object v)
            {
                return v is Double && map.Values().Contains(new MutableDouble((Double)v));
            }
        }

        public override ISet<KeyValuePair<E, Double>> EntrySet()
        {
            return new AnonymousAbstractSet(this);
        }

        private sealed class AnonymousEntry : Entry
        {
            public AnonymousEntry(ClassicCounterFactory parent)
            {
                this.parent = parent;
            }

            private readonly ClassicCounterFactory parent;
            readonly Entry<E, MutableDouble> e = inner.Next();
            public double GetDoubleValue()
            {
                return e.GetValue().DoubleValue();
            }

            public double SetValue(double value)
            {
                double old = e.GetValue().DoubleValue();
                e.GetValue().Set(value);
                totalCount = totalCount - old + value;
                return old;
            }

            public override E GetKey()
            {
                return e.GetKey();
            }

            public override Double GetValue()
            {
                return GetDoubleValue();
            }

            public override Double SetValue(Double value)
            {
                return SetValue(value.DoubleValue());
            }
        }

        private sealed class AnonymousIterator1 : Iterator
        {
            public AnonymousIterator1(ClassicCounterFactory parent)
            {
                this.parent = parent;
            }

            private readonly ClassicCounterFactory parent;
            readonly Iterator<Entry<E, MutableDouble>> inner = map.EntrySet().Iterator();
            public override bool HasNext()
            {
                return inner.HasNext();
            }

            public override Entry<E, Double> Next()
            {
                return new AnonymousEntry(this);
            }

            public override void Remove()
            {
                throw new NotSupportedException();
            }
        }

        private sealed class AnonymousAbstractSet : AbstractSet
        {
            public AnonymousAbstractSet(ClassicCounterFactory parent)
            {
                this.parent = parent;
            }

            private readonly ClassicCounterFactory parent;
            public override Iterator<Entry<E, Double>> Iterator()
            {
                return new AnonymousIterator1(this);
            }

            public override int Size()
            {
                return map.Size();
            }
        }

        public override void Clear()
        {
            map.Clear();
            totalCount = 0.0;
        }

        public override int Size()
        {
            return map.Size();
        }

        public override double TotalCount()
        {
            return totalCount;
        }

        public override Iterator<E> Iterator()
        {
            return KeySet().Iterator();
        }

        private MutableDouble MutableRemove(E key)
        {
            MutableDouble md = map.Remove(key);
            if (md != null)
            {
                totalCount -= md.DoubleValue();
            }

            return md;
        }

        public virtual void RemoveAll(ICollection<E> keys)
        {
            foreach (E key in keys)
            {
                MutableRemove(key);
            }
        }

        public virtual bool IsEmpty()
        {
            return Size() == 0;
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }
            else if (!(o is ICounter))
            {
                return false;
            }
            else if (!(o is ClassicCounter<E>))
            {
                return Counters.Equals(this, (ICounter<E>)o);
            }

            ClassicCounter<E> counter = (ClassicCounter<E>)o;
            return totalCount == counter.totalCount && map.Equals(counter.map);
        }

        public override int GetHashCode()
        {
            return map.GetHashCode();
        }

        public override string ToString()
        {
            return map.ToString();
        }

        public static ClassicCounter<String> ValueOfIgnoreComments(string s)
        {
            ClassicCounter<String> result = new ClassicCounter<String>();
            String[] lines = s.Split(@"\n");
            foreach (string line in lines)
            {
                String[] fields = line.Split(@"\t");
                if (fields.Length != 2)
                {
                    if (line.StartsWith(@"#"))
                    {
                        continue;
                    }
                    else
                    {
                        throw new Exception("Got unsplittable line: \"" + line + '\"');
                    }
                }

                result.SetCount(fields[0], Double.Parse(fields[1]));
            }

            return result;
        }

        public static ClassicCounter<String> FromString(string s)
        {
            ClassicCounter<String> result = new ClassicCounter<String>();
            if (!s.StartsWith(@"{") || !s.EndsWith(@"}"))
            {
                throw new Exception(@"invalid format: ||" + s + @"||");
            }

            s = s.Substring(1, s.Length() - 1);
            String[] lines = s.Split(@", ");
            foreach (string line in lines)
            {
                String[] fields = line.Split(@"=");
                if (fields.length != 2)
                    throw new Exception("Got unsplittable line: \"" + line + '\"');
                result.SetCount(fields[0], Double.ParseDouble(fields[1]));
            }

            return result;
        }

        public void PrettyLog(RedwoodChannels channels, string description)
        {
            PrettyLogger.Log(channels, description, Counters.AsMap(this));
        }
    }
}
