using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class ArrayCoreMap : ICoreMap
{
    private static readonly int INITIAL_CAPACITY = 4;
    private Type[] keys;
    private Object[] values;
    private int size;
    public ArrayCoreMap(): this (INITIAL_CAPACITY)
    {
    }

    public ArrayCoreMap(int capacity)
    {
        keys = new Type[capacity];
        values = new Object[capacity];
    }

    public ArrayCoreMap(ArrayCoreMap other)
    {
        size = other.size;
        keys = new Type[size];
        values = new Object[size];
        for (int i = 0; i < size; i++)
        {
            this.keys[i] = other.keys[i];
            this.values[i] = other.values[i];
        }
    }

    public ArrayCoreMap(ICoreMap other)
    {
        ISet<Type> otherKeys = other.KeySet();
        size = otherKeys.Size();
        keys = new Type[size];
        values = new Object[size];
        int i = 0;
        foreach (Type key in otherKeys)
        {
            this.keys[i] = key;
            this.values[i] = other.Get<object>(key);
            i++;
        }
    }

    public VALUE Get<VALUE>(Type key)
    {
        for (int i = 0; i < size; i++)
        {
            if (key == keys[i])
            {
                return (VALUE)values[i];
            }
        }

        return default(VALUE);
    }

    public bool Has(Type key)
    {
        for (int i = 0; i < size; i++)
        {
            if (keys[i] == key)
            {
                return true;
            }
        }

        return false;
    }

    public override VALUE Set<VALUE>(Type key, VALUE value)
    {
        for (int i = 0; i < size; i++)
        {
            if (keys[i] == key)
            {
                VALUE rv = (VALUE)values[i];
                values[i] = value;
                return rv;
            }
        }

        if (size >= keys.Length)
        {
            int capacity = keys.Length + (keys.Length < 16 ? 4 : 8);
            Type[] newKeys = new Type[capacity];
            Object[] newVals = new Object[capacity];
            Array.Copy(keys, 0, newKeys, 0, size);
            Array.Copy(values, 0, newVals, 0, size);
            keys = newKeys;
            values = newVals;
        }

        keys[size] = key;
        values[size] = value;
        size++;
        return default(VALUE);
    }

    public override ISet<Type> KeySet()
    {
        return new AnonymousAbstractSet(this);
    }

    private sealed class AnonymousIterator : IEnumerator<Type>
    {
        public AnonymousIterator(ArrayCoreMap parent)
        {
            this.parent = parent;
        }

        private readonly ArrayCoreMap parent;
        private int i = -1;

        public bool MoveNext()
        {
            if ((i + 1) < parent.size)
            {
                i++;
                return true;
            }

            return false;
        }

        public Type Current
        {
            get {
            try
            {
                return parent.keys[i];
            }
            catch (IndexOutOfRangeException aioobe)
            {
                throw new IndexOutOfRangeException(@"ArrayCoreMap keySet iterator exhausted");
            }
            }
        }
    
public void Dispose()
{
 	
}

object System.Collections.IEnumerator.Current
{
	get { return Current; }
}

public void Reset()
{
 	i = -1;
}
}

    private sealed class AnonymousAbstractSet : AbstractSet
    {
        public AnonymousAbstractSet(ArrayCoreMap parent)
        {
            this.parent = parent;
        }

        private readonly ArrayCoreMap parent;

        public override IEnumerator<Type> Iterator()
        {
            return new AnonymousIterator(parent);
        }

        public override int Size()
        {
            return parent.size;
        }
    }

    public VALUE Remove<VALUE>(Type key)
    {
        Object rv = null;
        for (int i = 0; i < size; i++)
        {
            if (keys[i] == key)
            {
                rv = values[i];
                if (i < size - 1)
                {
                    Array.Copy(keys, i + 1, keys, i, size - (i + 1));
                    Array.Copy(values, i + 1, values, i, size - (i + 1));
                }

                size--;
                break;
            }
        }

        return (VALUE)rv;
    }

    public bool ContainsKey(Type key)
    {
        for (int i = 0; i < size; i++)
        {
            if (keys[i] == key)
            {
                return true;
            }
        }

        return false;
    }

    public virtual void Compact()
    {
        if (keys.Length > size)
        {
            Type[] newKeys = new Type[size];
            Object[] newVals = new Object[size];
            Array.Copy(keys, 0, newKeys, 0, size);
            Array.Copy(values, 0, newVals, 0, size);
            keys = newKeys;
            values = newVals;
        }
    }

    public virtual void SetCapacity(int newSize)
    {
        if (size > newSize)
        {
            throw new Exception(@"You cannot set capacity to smaller than the current size.");
        }

        Type[] newKeys = new Type[newSize];
        Object[] newVals = new Object[newSize];
        Array.Copy(keys, 0, newKeys, 0, size);
        Array.Copy(values, 0, newVals, 0, size);
        keys = newKeys;
        values = newVals;
    }

    public int Size()
    {
        return size;
    }

    private static ThreadLocal<IdentityHashSet<ICoreMap>> toStringCalled = new ThreadLocal<IdentityHashSet<ICoreMap>>();

    public override string ToString()
    {
        IdentityHashSet<ICoreMap> calledSet = toStringCalled.Get();
        bool createdCalledSet = (calledSet == null);
        if (createdCalledSet)
        {
            calledSet = new IdentityHashSet<ICoreMap>();
            toStringCalled.Set(calledSet);
        }

        if (calledSet.Contains(this))
        {
            return @"[...]";
        }

        calledSet.Add(this);
        StringBuilder s = new StringBuilder(@"[");
        for (int i = 0; i < size; i++)
        {
            s.Append(keys[i].Name);
            s.Append('=');
            s.Append(values[i]);
            if (i < size - 1)
            {
                s.Append(' ');
            }
        }

        s.Append(']');
        if (createdCalledSet)
        {
            toStringCalled.Set(null);
        }
        else
        {
            calledSet.Remove(this);
        }

        return s.ToString();
    }

    public virtual string ToShorterString(params string[] what)
    {
        StringBuilder s = new StringBuilder(@"[");
        for (int i = 0; i < size; i++)
        {
            string name = keys[i].Name;
            int annoIdx = name.LastIndexOf(@"Annotation");
            if (annoIdx >= 0)
            {
                name = name.Substring(0, annoIdx);
            }

            bool include;
            if (what.Length > 0)
            {
                include = false;
                foreach (string item in what)
                {
                    if (item.Equals(name))
                    {
                        include = true;
                        break;
                    }
                }
            }
            else
            {
                include = true;
            }

            if (include)
            {
                if (s.Length > 1)
                {
                    s.Append(' ');
                }

                s.Append(name);
                s.Append('=');
                s.Append(values[i]);
            }
        }

        s.Append(']');
        return s.ToString();
    }

    public virtual string ToShortString(params string[] what)
    {
        return ToShortString('/', what);
    }

    public virtual string ToShortString(char separator, params string[] what)
    {
        StringBuilder s = new StringBuilder();
        for (int i = 0; i < size; i++)
        {
            bool include;
            if (what.Length > 0)
            {
                string name = keys[i].Name;
                int annoIdx = name.LastIndexOf(@"Annotation");
                if (annoIdx >= 0)
                {
                    name = name.Substring(0, annoIdx);
                }

                include = false;
                foreach (string item in what)
                {
                    if (item.Equals(name))
                    {
                        include = true;
                        break;
                    }
                }
            }
            else
            {
                include = true;
            }

            if (include)
            {
                if (s.Length > 0)
                {
                    s.Append(separator);
                }

                s.Append(values[i]);
            }
        }

        string answer = s.ToString();
        if (answer.IndexOf(' ') < 0)
        {
            return answer;
        }
        else
        {
            return '{' + answer + '}';
        }
    }

    private static ThreadLocal<TwoDimensionalMap<ICoreMap, ICoreMap, Boolean>> equalsCalled = new ThreadLocal<TwoDimensionalMap<ICoreMap, ICoreMap, Boolean>>();
    public override bool Equals(Object obj)
    {
        if (!(obj is ICoreMap))
        {
            return false;
        }

        if (obj is HashableCoreMap)
        {
            return obj.Equals(this);
        }

        if (obj is ArrayCoreMap)
        {
            return Equals((ArrayCoreMap)obj);
        }

        ICoreMap other = (ICoreMap)obj;
        if (!this.KeySet().Equals(other.KeySet()))
        {
            return false;
        }

        foreach (Type key in this.KeySet())
        {
            if (!other.Has(key))
            {
                return false;
            }

            Object thisV = this.Get<object>(key), otherV = other.Get<object>(key);
            if (thisV == otherV)
            {
                continue;
            }

            if (thisV == null || otherV == null)
            {
                return false;
            }

            if (!thisV.Equals(otherV))
            {
                return false;
            }
        }

        return true;
    }

    private bool Equals(ArrayCoreMap other)
    {
        TwoDimensionalMap<ICoreMap, ICoreMap, Boolean> calledMap = equalsCalled.Get();
        bool createdCalledMap = (calledMap == null);
        if (createdCalledMap)
        {
            calledMap = TwoDimensionalMap.IdentityHashMap();
            equalsCalled.Set(calledMap);
        }

        if (calledMap.Contains(this, other))
        {
            return true;
        }

        bool result = true;
        calledMap.Put(this, other, true);
        calledMap.Put(other, this, true);
        if (this.size != other.size)
        {
            result = false;
        }
        else
        {
            for (int i = 0; i < this.size; i++)
            {
                bool matched = false;
                for (int j = 0; j < other.size; j++)
                {
                    if (this.keys[i] == other.keys[j])
                    {
                        if ((this.values[i] == null && other.values[j] != null) || (this.values[i] != null && other.values[j] == null))
                        {
                            matched = false;
                            break;
                        }

                        if ((this.values[i] == null && other.values[j] == null) || (this.values[i].Equals(other.values[j])))
                        {
                            matched = true;
                            break;
                        }
                    }
                }

                if (!matched)
                {
                    result = false;
                    break;
                }
            }
        }

        if (createdCalledMap)
        {
            equalsCalled.Set(null);
        }

        return result;
    }

    private static ThreadLocal<IdentityHashSet<ICoreMap>> hashCodeCalled = new ThreadLocal<IdentityHashSet<ICoreMap>>();
    public override int GetHashCode()
    {
        IdentityHashSet<ICoreMap> calledSet = hashCodeCalled.Get();
        bool createdCalledSet = (calledSet == null);
        if (createdCalledSet)
        {
            calledSet = new IdentityHashSet<ICoreMap>();
            hashCodeCalled.Set(calledSet);
        }

        if (calledSet.Contains(this))
        {
            return 0;
        }

        calledSet.Add(this);
        int keysCode = 0;
        int valuesCode = 0;
        for (int i = 0; i < size; i++)
        {
            keysCode += keys[i].GetHashCode();
            valuesCode += (values[i] != null ? values[i].GetHashCode() : 0);
        }

        if (createdCalledSet)
        {
            hashCodeCalled.Set(null);
        }
        else
        {
            calledSet.Remove(this);
        }

        return keysCode * 37 + valuesCode;
    }

    private void WriteObject(ObjectOutputStream out_renamed)
    {
        Compact();
        out_renamed.DefaultWriteObject();
    }

    public override void PrettyLog(RedwoodChannels channels, string description)
    {
        Redwood.StartTrack(description);
        List<Type> sortedKeys = new List<Type>(this.KeySet());
        Collections.Sort(sortedKeys, new AnonymousComparator(this));
        foreach (Type key in sortedKeys)
        {
            string keyName = key.GetCanonicalName().Replace(@"class ", @"");
            Object value = this.Get(key);
            if (PrettyLogger.Dispatchable(value))
            {
                PrettyLogger.Log(channels, keyName, value);
            }
            else
            {
                channels.Logf(@"%s = %s", keyName, value);
            }
        }

        Redwood.EndTrack(description);
    }

    private sealed class AnonymousComparator : IComparer<Type>
    {
        public AnonymousComparator(ArrayCoreMap parent)
        {
            this.parent = parent;
        }

        private readonly ArrayCoreMap parent;
        public int Compare(Type a, Type b)
        {
            return a.FullName.CompareTo(b.FullName);
        }
    }
}
}
