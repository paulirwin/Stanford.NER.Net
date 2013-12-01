using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Util
{
    public class ArrayHeap<E> : AbstractSet<E>, IHeap<E>
    {
        private sealed class HeapEntry<E>
        {
            public E obj;
            public int index;
        }

        private List<HeapEntry<E>> indexToEntry;
        private IDictionary<E, HeapEntry<E>> objectToEntry;
        private IComparer<E> cmp;

        private static int Parent(int index)
        {
            return (index - 1) / 2;
        }

        private HeapEntry<E> Parent(HeapEntry<E> entry)
        {
            int index = entry.index;
            return (index > 0 ? indexToEntry.Get((index - 1) / 2) : null);
        }

        private HeapEntry<E> LeftChild(HeapEntry<E> entry)
        {
            int index = entry.index;
            int leftIndex = index * 2 + 1;
            return (leftIndex < Count ? indexToEntry.Get(leftIndex) : null);
        }

        private HeapEntry<E> RightChild(HeapEntry<E> entry)
        {
            int index = entry.index;
            int rightIndex = index * 2 + 2;
            return (rightIndex < Count ? indexToEntry.Get(rightIndex) : null);
        }

        private int Compare(HeapEntry<E> entryA, HeapEntry<E> entryB)
        {
            return cmp.Compare(entryA.obj, entryB.obj);
        }

        private void Swap(HeapEntry<E> entryA, HeapEntry<E> entryB)
        {
            int indexA = entryA.index;
            int indexB = entryB.index;
            entryA.index = indexB;
            entryB.index = indexA;
            indexToEntry.Set(indexA, entryB);
            indexToEntry.Set(indexB, entryA);
        }

        private void RemoveLast(HeapEntry<E> entry)
        {
            indexToEntry.RemoveAt(entry.index);
            objectToEntry.Remove(entry.obj);
        }

        private HeapEntry<E> GetEntry(E o)
        {
            HeapEntry<E> entry = objectToEntry.Get(o);
            if (entry == null)
            {
                entry = new HeapEntry<E>();
                entry.index = Count;
                entry.obj = o;
                indexToEntry.Add(entry);
                objectToEntry.Put(o, entry);
            }

            return entry;
        }

        private int HeapifyUp(HeapEntry<E> entry)
        {
            int numSwaps = 0;
            while (true)
            {
                if (entry.index == 0)
                {
                    break;
                }

                HeapEntry<E> parentEntry = Parent(entry);
                if (Compare(entry, parentEntry) >= 0)
                {
                    break;
                }

                numSwaps++;
                Swap(entry, parentEntry);
            }

            return numSwaps;
        }

        private void HeapifyDown(HeapEntry<E> entry)
        {
            HeapEntry<E> minEntry;
            do
            {
                minEntry = entry;
                HeapEntry<E> leftEntry = LeftChild(entry);
                if (leftEntry != null)
                {
                    if (Compare(minEntry, leftEntry) > 0)
                    {
                        minEntry = leftEntry;
                    }
                }

                HeapEntry<E> rightEntry = RightChild(entry);
                if (rightEntry != null)
                {
                    if (Compare(minEntry, rightEntry) > 0)
                    {
                        minEntry = rightEntry;
                    }
                }

                if (minEntry != entry)
                {
                    Swap(minEntry, entry);
                }
            }
            while (minEntry != entry);
        }

        public override E ExtractMin()
        {
            if (IsEmpty())
            {
                throw new InvalidOperationException();
            }

            HeapEntry<E> minEntry = indexToEntry.Get(0);
            int lastIndex = Count - 1;
            if (lastIndex > 0)
            {
                HeapEntry<E> lastEntry = indexToEntry.Get(lastIndex);
                Swap(lastEntry, minEntry);
                RemoveLast(minEntry);
                HeapifyDown(lastEntry);
            }
            else
            {
                RemoveLast(minEntry);
            }

            return minEntry.obj;
        }

        public override E Min()
        {
            HeapEntry<E> minEntry = indexToEntry.Get(0);
            return minEntry.obj;
        }

        public override bool Add(E o)
        {
            DecreaseKey(o);
            return true;
        }

        public override int DecreaseKey(E o)
        {
            HeapEntry<E> entry = GetEntry(o);
            if (!object.Equals(o, entry.obj))
            {
                if (cmp.Compare(o, entry.obj) < 0)
                {
                    entry.obj = o;
                }
            }

            return HeapifyUp(entry);
        }

        public override bool IsEmpty()
        {
            return indexToEntry.IsEmpty();
        }

        public override int Count
        {
            get
            {
                return indexToEntry.Size();
            }
        }

        public override IEnumerator<E> GetEnumerator()
        {
            IHeap<E> tempHeap = new ArrayHeap<E>(cmp, Count);
            List<E> tempList = new List<E>(Count);
            foreach (E obj in objectToEntry.Keys)
            {
                tempHeap.Add(obj);
            }

            while (!tempHeap.IsEmpty())
            {
                tempList.Add(tempHeap.ExtractMin());
            }

            return tempList.GetEnumerator();
        }

        public override void Clear()
        {
            indexToEntry.Clear();
            objectToEntry.Clear();
        }

        public virtual void Dump()
        {
            for (int j = 0; j < indexToEntry.Size(); j++)
            {
                Console.Error.WriteLine(@" " + j + @" " + ((IScored)indexToEntry.Get(j).obj).Score());
            }
        }

        public virtual void Verify()
        {
            for (int i = 0; i < indexToEntry.Size(); i++)
            {
                if (i != 0)
                {
                    if (Compare(indexToEntry.Get(i), indexToEntry.Get(Parent(i))) < 0)
                    {
                        Console.Error.WriteLine(@"Error in the ordering of the heap! (" + i + @")");
                        Dump();
                        Environment.Exit(0);
                    }
                }

                if (i != indexToEntry.Get(i).index)
                {
                    Console.Error.WriteLine(@"Error in placement in the heap!");
                }
            }
        }

        public ArrayHeap(IComparer<E> cmp)
        {
            this.cmp = cmp;
            indexToEntry = new List<HeapEntry<E>>();
            objectToEntry = new HashMap<E, HeapEntry<E>>();
        }

        public ArrayHeap(IComparer<E> cmp, int initCapacity)
        {
            this.cmp = cmp;
            indexToEntry = new List<HeapEntry<E>>(initCapacity);
            objectToEntry = new HashMap<E, HeapEntry<E>>();
        }

        public virtual List<E> AsList()
        {
            return new List<E>(this);
        }

        public override string ToString()
        {
            List<E> result = new List<E>();
            foreach (E key in objectToEntry.Keys)
                result.Add(key);
            result.Sort(cmp);
            return result.ToString();
        }

        public override bool Remove(E item)
        {
            throw new NotImplementedException();
        }
    }
}
