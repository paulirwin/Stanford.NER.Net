using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Stats;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public abstract class GeneralDataset<L, F> : IEnumerable<RVFDatum<L, F>>
    {
        public IIndex<L> labelIndex;
        public IIndex<F> featureIndex;
        protected int[] labels;
        protected int[][] data;
        protected int size;
        public GeneralDataset()
        {
        }

        public virtual IIndex<L> LabelIndex()
        {
            return labelIndex;
        }

        public virtual IIndex<F> FeatureIndex()
        {
            return featureIndex;
        }

        public virtual int NumFeatures()
        {
            return featureIndex.Size();
        }

        public virtual int NumClasses()
        {
            return labelIndex.Size();
        }

        public virtual int[] GetLabelsArray()
        {
            labels = TrimToSize(labels);
            return labels;
        }

        public virtual int[][] GetDataArray()
        {
            data = TrimToSize(data);
            return data;
        }

        public abstract double[][] GetValuesArray();
        public virtual void Clear()
        {
            Clear(10);
        }

        public virtual void Clear(int numDatums)
        {
            Initialize(numDatums);
        }

        protected abstract void Initialize(int numDatums);
        public abstract RVFDatum<L, F> GetRVFDatum(int index);
        public abstract IDatum<L, F> GetDatum(int index);
        public abstract void Add(IDatum<L, F> d);
        public virtual float[] GetFeatureCounts()
        {
            float[] counts = new float[featureIndex.Size()];
            for (int i = 0, m = size; i < m; i++)
            {
                for (int j = 0, n = data[i].Length; j < n; j++)
                {
                    counts[data[i][j]] += 1.0f;
                }
            }

            return counts;
        }

        public virtual void ApplyFeatureCountThreshold(int k)
        {
            float[] counts = GetFeatureCounts();
            IIndex<F> newFeatureIndex = new HashIndex<F>();
            int[] featMap = new int[featureIndex.Size()];
            for (int i = 0; i < featMap.Length; i++)
            {
                F feat = featureIndex.Get(i);
                if (counts[i] >= k)
                {
                    int newIndex = newFeatureIndex.Size();
                    newFeatureIndex.Add(feat);
                    featMap[i] = newIndex;
                }
                else
                {
                    featMap[i] = -1;
                }
            }

            featureIndex = newFeatureIndex;
            for (int i = 0; i < size; i++)
            {
                List<int> featList = new List<int>(data[i].Length);
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (featMap[data[i][j]] >= 0)
                    {
                        featList.Add(featMap[data[i][j]]);
                    }
                }

                data[i] = new int[featList.Size()];
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = featList.Get(j);
                }
            }
        }

        public virtual void ApplyFeatureMaxCountThreshold(int k)
        {
            float[] counts = GetFeatureCounts();
            HashIndex<F> newFeatureIndex = new HashIndex<F>();
            int[] featMap = new int[featureIndex.Size()];
            for (int i = 0; i < featMap.Length; i++)
            {
                F feat = featureIndex.Get(i);
                if (counts[i] <= k)
                {
                    int newIndex = newFeatureIndex.Size();
                    newFeatureIndex.Add(feat);
                    featMap[i] = newIndex;
                }
                else
                {
                    featMap[i] = -1;
                }
            }

            featureIndex = newFeatureIndex;
            for (int i = 0; i < size; i++)
            {
                List<int> featList = new List<int>(data[i].Length);
                for (int j = 0; j < data[i].Length; j++)
                {
                    if (featMap[data[i][j]] >= 0)
                    {
                        featList.Add(featMap[data[i][j]]);
                    }
                }

                data[i] = new int[featList.Size()];
                for (int j = 0; j < data[i].Length; j++)
                {
                    data[i][j] = featList.Get(j);
                }
            }
        }

        public virtual int NumFeatureTokens()
        {
            int x = 0;
            for (int i = 0, m = size; i < m; i++)
            {
                x += data[i].Length;
            }

            return x;
        }

        public virtual int NumFeatureTypes()
        {
            return featureIndex.Size();
        }

        public virtual void AddAll(IEnumerable<IDatum<L, F>> data)
        {
            foreach (IDatum<L, F> d in data)
            {
                Add(d);
            }
        }

        public abstract Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>> Split(int start, int end);
        public abstract Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>> Split(double p);
        public virtual int Size()
        {
            return size;
        }

        protected virtual void TrimData()
        {
            data = TrimToSize(data);
        }

        protected virtual void TrimLabels()
        {
            labels = TrimToSize(labels);
        }

        protected virtual int[] TrimToSize(int[] i)
        {
            int[] newI = new int[size];
            Array.Copy(i, 0, newI, 0, size);
            return newI;
        }

        protected virtual int[][] TrimToSize(int[][] i)
        {
            int[][] newI = new int[size][];
            Array.Copy(i, 0, newI, 0, size);
            return newI;
        }

        protected virtual double[][] TrimToSize(double[][] i)
        {
            double[][] newI = new double[size][];
            Array.Copy(i, 0, newI, 0, size);
            return newI;
        }

        public virtual void Randomize(int randomSeed)
        {
            Random rand = new Random(randomSeed);
            for (int j = size - 1; j > 0; j--)
            {
                int randIndex = rand.Next(j);
                int[] tmp = data[randIndex];
                data[randIndex] = data[j];
                data[j] = tmp;
                int tmpl = labels[randIndex];
                labels[randIndex] = labels[j];
                labels[j] = tmpl;
            }
        }

        public virtual GeneralDataset<L, F> SampleDataset(int randomSeed, double sampleFrac, bool sampleWithReplacement)
        {
            int sampleSize = (int)(this.Size() * sampleFrac);
            Random rand = new Random(randomSeed);
            GeneralDataset<L, F> subset;
            if (this is RVFDataset<L, F>)
                subset = new RVFDataset<L, F>();
            else if (this is Dataset)
            {
                subset = new Dataset<L, F>();
            }
            else
            {
                throw new Exception(@"Can't handle this type of GeneralDataset.");
            }

            if (sampleWithReplacement)
            {
                for (int i = 0; i < sampleSize; i++)
                {
                    int datumNum = rand.Next(this.Size());
                    subset.Add(this.GetDatum(datumNum));
                }
            }
            else
            {
                ISet<int> indicedSampled = new HashSet<int>();
                while (subset.Size() < sampleSize)
                {
                    int datumNum = rand.Next(this.Size());
                    if (!indicedSampled.Contains(datumNum))
                    {
                        subset.Add(this.GetDatum(datumNum));
                        indicedSampled.Add(datumNum);
                    }
                }
            }

            return subset;
        }

        public abstract void SummaryStatistics();
        public virtual IEnumerator<L> LabelIterator()
        {
            return labelIndex.GetEnumerator();
        }

        public virtual GeneralDataset<L, F> MapDataset(GeneralDataset<L, F> dataset)
        {
            GeneralDataset<L, F> newDataset;
            if (dataset is RVFDataset)
                newDataset = new RVFDataset<L, F>(this.featureIndex, this.labelIndex);
            else
                newDataset = new Dataset<L, F>(this.featureIndex, this.labelIndex);
            this.featureIndex.Lock();
            this.labelIndex.Lock();
            for (int i = 0; i < dataset.Size(); i++)
                newDataset.Add(dataset.GetDatum(i));
            this.featureIndex.Unlock();
            this.labelIndex.Unlock();
            return newDataset;
        }

        public static IDatum<L2, F> MapDatum<L2>(IDatum<L, F> d, IDictionary<L, L2> labelMapping, L2 defaultLabel)
        {
            L2 newLabel = labelMapping.Get(d.Label());
            if (newLabel == null)
            {
                newLabel = defaultLabel;
            }

            if (d is RVFDatum)
            {
                return new RVFDatum<L2, F>(((RVFDatum<L, F>)d).AsFeaturesCounter(), newLabel);
            }
            else
            {
                return new BasicDatum<L2, F>(d.AsFeatures(), newLabel);
            }
        }

        public virtual GeneralDataset<L2, F> MapDataset<L2>(GeneralDataset<L, F> dataset, IIndex<L2> newLabelIndex, IDictionary<L, L2> labelMapping, L2 defaultLabel)
        {
            GeneralDataset<L2, F> newDataset;
            if (dataset is RVFDataset)
                newDataset = new RVFDataset<L2, F>(this.featureIndex, newLabelIndex);
            else
                newDataset = new Dataset<L2, F>(this.featureIndex, newLabelIndex);
            this.featureIndex.Lock();
            this.labelIndex.Lock();
            for (int i = 0; i < dataset.Size(); i++)
            {
                IDatum<L, F> d = dataset.GetDatum(i);
                IDatum<L2, F> d2 = MapDatum(d, labelMapping, defaultLabel);
                newDataset.Add(d2);
            }

            this.featureIndex.Unlock();
            this.labelIndex.Unlock();
            return newDataset;
        }

        public virtual void PrintSVMLightFormat()
        {
            PrintSVMLightFormat(Console.Out);
        }

        public virtual String[] MakeSvmLabelMap()
        {
            String[] labelMap = new string[NumClasses()];
            if (NumClasses() > 2)
            {
                for (int i = 0; i < labelMap.Length; i++)
                {
                    labelMap[i] = (i + 1).ToString();
                }
            }
            else
            {
                labelMap = new string[] { @"+1", @"-1" };
            }

            return labelMap;
        }

        public virtual void PrintSVMLightFormat(TextWriter pw)
        {
            String[] labelMap = MakeSvmLabelMap();
            for (int i = 0; i < size; i++)
            {
                RVFDatum<L, F> d = GetRVFDatum(i);
                ICounter<F> c = d.AsFeaturesCounter();
                ClassicCounter<int> printC = new ClassicCounter<int>();
                foreach (F f in c.KeySet())
                {
                    printC.SetCount(featureIndex.IndexOf(f), c.GetCount(f));
                }

                int[] features = printC.KeySet().ToArray();
                Array.Sort(features);
                StringBuilder sb = new StringBuilder();
                sb.Append(labelMap[labels[i]]).Append(' ');
                foreach (int f in features)
                {
                    sb.Append((f + 1)).Append(':').Append(printC.GetCount(f)).Append(' ');
                }

                pw.WriteLine(sb.ToString());
            }
        }

        public virtual IEnumerator<RVFDatum<L, F>> GetEnumerator()
        {
            return new AnonymousIterator(this);
        }

        private sealed class AnonymousIterator : IEnumerator<RVFDatum<L, F>>
        {
            public AnonymousIterator(GeneralDataset<L, F> parent)
            {
                this.parent = parent;
            }

            private readonly GeneralDataset<L, F> parent;
            private int id;
            private RVFDatum<L, F> current;

            public bool MoveNext()
            {
                if (id < parent.Size())
                {
                    current = parent.GetRVFDatum(id++);
                    return true;
                }

                return false;
            }

            public RVFDatum<L, F> Current
            {
                get
                {
                    return current;
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
            }
        }
    }
}
