using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Stats;
using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public class Dataset<L, F> : GeneralDataset<L, F>
        where L : class
        where F : class
    {
        public Dataset()
            : this(10)
        {
        }

        public Dataset(int numDatums)
        {
            Initialize(numDatums);
        }

        public Dataset(int numDatums, IIndex<F> featureIndex, IIndex<L> labelIndex)
        {
            Initialize(numDatums);
            this.featureIndex = featureIndex;
            this.labelIndex = labelIndex;
        }

        public Dataset(IIndex<F> featureIndex, IIndex<L> labelIndex)
            : this(10, featureIndex, labelIndex)
        {
        }

        public Dataset(IIndex<L> labelIndex, int[] labels, IIndex<F> featureIndex, int[][] data)
            : this(labelIndex, labels, featureIndex, data, data.Length)
        {
        }

        public Dataset(IIndex<L> labelIndex, int[] labels, IIndex<F> featureIndex, int[][] data, int size)
        {
            this.labelIndex = labelIndex;
            this.labels = labels;
            this.featureIndex = featureIndex;
            this.data = data;
            this.size = size;
        }

        public override Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>> Split(double percentDev)
        {
            int devSize = (int)(percentDev * Size());
            int trainSize = Size() - devSize;
            int[][] devData = new int[devSize][];
            int[] devLabels = new int[devSize];
            int[][] trainData = new int[trainSize][];
            int[] trainLabels = new int[trainSize];
            Array.Copy(data, 0, devData, 0, devSize);
            Array.Copy(labels, 0, devLabels, 0, devSize);
            Array.Copy(data, devSize, trainData, 0, trainSize);
            Array.Copy(labels, devSize, trainLabels, 0, trainSize);
            if (this is WeightedDataset)
            {
                float[] trainWeights = new float[trainSize];
                float[] devWeights = new float[devSize];
                WeightedDataset<L, F> w = (WeightedDataset<L, F>)this;
                Array.Copy(w.weights, 0, devWeights, 0, devSize);
                Array.Copy(w.weights, devSize, trainWeights, 0, trainSize);
                WeightedDataset<L, F> dev = new WeightedDataset<L, F>(labelIndex, devLabels, featureIndex, devData, devSize, devWeights);
                WeightedDataset<L, F> train = new WeightedDataset<L, F>(labelIndex, trainLabels, featureIndex, trainData, trainSize, trainWeights);
                return new Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>>(train, dev);
            }

            Dataset<L, F> dev2 = new Dataset<L, F>(labelIndex, devLabels, featureIndex, devData, devSize);
            Dataset<L, F> train2 = new Dataset<L, F>(labelIndex, trainLabels, featureIndex, trainData, trainSize);
            return new Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>>(train2, dev2);
        }

        public override Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>> Split(int start, int end)
        {
            int devSize = end - start;
            int trainSize = Size() - devSize;
            int[][] devData = new int[devSize][];
            int[] devLabels = new int[devSize];
            int[][] trainData = new int[trainSize][];
            int[] trainLabels = new int[trainSize];
            Array.Copy(data, start, devData, 0, devSize);
            Array.Copy(labels, start, devLabels, 0, devSize);
            Array.Copy(data, 0, trainData, 0, start);
            Array.Copy(data, end, trainData, start, Size() - end);
            Array.Copy(labels, 0, trainLabels, 0, start);
            Array.Copy(labels, end, trainLabels, start, Size() - end);
            if (this is WeightedDataset)
            {
                float[] trainWeights = new float[trainSize];
                float[] devWeights = new float[devSize];
                WeightedDataset<L, F> w = (WeightedDataset<L, F>)this;
                Array.Copy(w.weights, start, devWeights, 0, devSize);
                Array.Copy(w.weights, 0, trainWeights, 0, start);
                Array.Copy(w.weights, end, trainWeights, start, Size() - end);
                WeightedDataset<L, F> dev = new WeightedDataset<L, F>(labelIndex, devLabels, featureIndex, devData, devSize, devWeights);
                WeightedDataset<L, F> train = new WeightedDataset<L, F>(labelIndex, trainLabels, featureIndex, trainData, trainSize, trainWeights);
                return new Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>>(train, dev);
            }

            Dataset<L, F> dev2 = new Dataset<L, F>(labelIndex, devLabels, featureIndex, devData, devSize);
            Dataset<L, F> train2 = new Dataset<L, F>(labelIndex, trainLabels, featureIndex, trainData, trainSize);
            return new Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>>(train2, dev2);
        }

        public virtual Dataset<L, F> GetRandomSubDataset(double p, int seed)
        {
            int newSize = (int)(p * Size());
            ISet<int> indicesToKeep = new HashSet<int>();
            Random r = new Random(seed);
            int s = Size();
            while (indicesToKeep.Size() < newSize)
            {
                indicesToKeep.Add(r.Next(s));
            }

            int[][] newData = new int[newSize][];
            int[] newLabels = new int[newSize];
            int i = 0;
            foreach (int j in indicesToKeep)
            {
                newData[i] = data[j];
                newLabels[i] = labels[j];
                i++;
            }

            return new Dataset<L, F>(labelIndex, newLabels, featureIndex, newData);
        }

        public override double[][] GetValuesArray()
        {
            return null;
        }

        public static Dataset<String, String> ReadSVMLightFormat(string filename)
        {
            return ReadSVMLightFormat(filename, new HashIndex<String>(), new HashIndex<String>());
        }

        public static Dataset<String, String> ReadSVMLightFormat(string filename, List<String> lines)
        {
            return ReadSVMLightFormat(filename, new HashIndex<String>(), new HashIndex<String>(), lines);
        }

        public static Dataset<String, String> ReadSVMLightFormat(string filename, IIndex<String> featureIndex, IIndex<String> labelIndex)
        {
            return ReadSVMLightFormat(filename, featureIndex, labelIndex, null);
        }

        public static Dataset<String, String> ReadSVMLightFormat(string filename, IIndex<String> featureIndex, IIndex<String> labelIndex, List<String> lines)
        {
            Dataset<String, String> dataset;
            try
            {
                dataset = new Dataset<String, String>(10, featureIndex, labelIndex);
                foreach (string line in ObjectBank.GetLineIterator(new File(filename)))
                {
                    if (lines != null)
                        lines.Add(line);
                    dataset.Add(SvmLightLineToDatum(line));
                }
            }
            catch (Exception e)
            {
                throw;
            }

            return dataset;
        }

        private static int line1 = 0;
        public static IDatum<String, String> SvmLightLineToDatum(string l)
        {
            var rx = new Regex("#.*");
            line1++;
            l = rx.Replace(l, "");

            rx = new Regex("\\s+");
            String[] line = rx.Split(l);
            ICollection<String> features = new List<String>();
            for (int i = 1; i < line.Length; i++)
            {
                String[] f = line[i].Split(':');
                if (f.Length != 2)
                {
                    Console.Error.WriteLine(@"Dataset error: line " + line1);
                }

                int val = (int)Double.Parse(f[1]);
                for (int j = 0; j < val; j++)
                {
                    features.Add(f[0]);
                }
            }

            features.Add(int.MaxValue.ToString());
            IDatum<String, String> d = new BasicDatum<String, String>(features, line[0]);
            return d;
        }

        public virtual ICounter<F> GetFeatureCounter()
        {
            ICounter<F> featureCounts = new ClassicCounter<F>();
            for (int i = 0; i < this.Size(); i++)
            {
                BasicDatum<L, F> datum = (BasicDatum<L, F>)GetDatum(i);
                ISet<F> featureSet = new HashSet<F>(datum.AsFeatures());
                foreach (F key in featureSet)
                {
                    featureCounts.IncrementCount(key, 1.0);
                }
            }

            return featureCounts;
        }

        public virtual RVFDatum<L, F> GetL1NormalizedTFIDFDatum(IDatum<L, F> datum, ICounter<F> featureDocCounts)
        {
            ICounter<F> tfidfFeatures = new ClassicCounter<F>();
            foreach (F feature in datum.AsFeatures())
            {
                if (featureDocCounts.ContainsKey(feature))
                    tfidfFeatures.IncrementCount(feature, 1.0);
            }

            double l1norm = 0;
            foreach (F feature in tfidfFeatures.KeySet())
            {
                double idf = System.Math.Log(((double)(this.Size() + 1)) / (featureDocCounts.GetCount(feature) + 0.5));
                double tf = tfidfFeatures.GetCount(feature);
                tfidfFeatures.SetCount(feature, tf * idf);
                l1norm += tf * idf;
            }

            foreach (F feature in tfidfFeatures.KeySet())
            {
                double tfidf = tfidfFeatures.GetCount(feature);
                tfidfFeatures.SetCount(feature, tfidf / l1norm);
            }

            RVFDatum<L, F> rvfDatum = new RVFDatum<L, F>(tfidfFeatures, datum.Label());
            return rvfDatum;
        }

        public virtual RVFDataset<L, F> GetL1NormalizedTFIDFDataset()
        {
            RVFDataset<L, F> rvfDataset = new RVFDataset<L, F>(this.Size(), this.featureIndex, this.labelIndex);
            ICounter<F> featureDocCounts = GetFeatureCounter();
            for (int i = 0; i < this.Size(); i++)
            {
                IDatum<L, F> datum = this.GetDatum(i);
                RVFDatum<L, F> rvfDatum = GetL1NormalizedTFIDFDatum(datum, featureDocCounts);
                rvfDataset.Add(rvfDatum);
            }

            return rvfDataset;
        }

        public override void Add(IDatum<L, F> d)
        {
            Add(d.AsFeatures(), d.Label());
        }

        public virtual void Add(ICollection<F> features, L label)
        {
            Add(features, label, true);
        }

        public virtual void Add(ICollection<F> features, L label, bool addNewFeatures)
        {
            EnsureSize();
            AddLabel(label);
            AddFeatures(features, addNewFeatures);
            size++;
        }

        public virtual void Add(int[] features, int label)
        {
            EnsureSize();
            AddLabelIndex(label);
            AddFeatureIndices(features);
            size++;
        }

        protected virtual void EnsureSize()
        {
            if (labels.Length == size)
            {
                int[] newLabels = new int[size * 2];
                Array.Copy(labels, 0, newLabels, 0, size);
                labels = newLabels;
                int[][] newData = new int[size * 2][];
                Array.Copy(data, 0, newData, 0, size);
                data = newData;
            }
        }

        protected virtual void AddLabel(L label)
        {
            labelIndex.Add(label);
            labels[size] = labelIndex.IndexOf(label);
        }

        protected virtual void AddLabelIndex(int label)
        {
            labels[size] = label;
        }

        protected virtual void AddFeatures(ICollection<F> features)
        {
            AddFeatures(features, true);
        }

        protected virtual void AddFeatures(ICollection<F> features, bool addNewFeatures)
        {
            int[] intFeatures = new int[features.Size()];
            int j = 0;
            foreach (F feature in features)
            {
                if (addNewFeatures)
                    featureIndex.Add(feature);
                int index = featureIndex.IndexOf(feature);
                if (index >= 0)
                {
                    intFeatures[j] = featureIndex.IndexOf(feature);
                    j++;
                }
            }

            data[size] = new int[j];
            Array.Copy(intFeatures, 0, data[size], 0, j);
        }

        protected virtual void AddFeatureIndices(int[] features)
        {
            data[size] = features;
        }

        protected override void Initialize(int numDatums)
        {
            labelIndex = new HashIndex<L>();
            featureIndex = new HashIndex<F>();
            labels = new int[numDatums];
            data = new int[numDatums][];
            size = 0;
        }

        public override IDatum<L, F> GetDatum(int index)
        {
            return new BasicDatum<L, F>(featureIndex.Objects(data[index]), labelIndex.Get(labels[index]));
        }

        public override RVFDatum<L, F> GetRVFDatum(int index)
        {
            ClassicCounter<F> c = new ClassicCounter<F>();
            foreach (F key in featureIndex.Objects(data[index]))
            {
                c.IncrementCount(key);
            }

            return new RVFDatum<L, F>(c, labelIndex.Get(labels[index]));
        }

        public override void SummaryStatistics()
        {
            Console.Error.WriteLine(ToSummaryStatistics());
        }

        public virtual string ToSummaryStatistics()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"numDatums: ").Append(size).Append('\n');
            sb.Append(@"numLabels: ").Append(labelIndex.Size()).Append(@" [");
            sb.Append(string.Join(", ", labelIndex));

            sb.Append(@"]\n");
            sb.Append(@"numFeatures (Phi(X) types): ").Append(featureIndex.Size()).Append('\n');
            return sb.ToString();
        }

        public virtual void ApplyFeatureCountThreshold(List<Tuple<Regex, int>> thresholds)
        {
            float[] counts = GetFeatureCounts();
            IIndex<F> newFeatureIndex = new HashIndex<F>();
        
            foreach (F f in featureIndex)
            {
                bool shouldContinueOuter = false;

                foreach (Tuple<Regex, int> threshold in thresholds)
                {
                    Regex p = threshold.Item1;
                    //Matcher m = p.Matcher(f.ToString());
                    if (p.IsMatch(f.ToString()))
                    {
                        if (counts[featureIndex.IndexOf(f)] >= threshold.Item2)
                        {
                            newFeatureIndex.Add(f);
                        }

                        shouldContinueOuter = true;
                        break;
                    }
                }

                if (shouldContinueOuter)
                    continue;

                newFeatureIndex.Add(f);
            }

            counts = null;
            int[] featMap = new int[featureIndex.Size()];
            for (int i = 0; i < featMap.Length; i++)
            {
                featMap[i] = newFeatureIndex.IndexOf(featureIndex.Get(i));
            }

            featureIndex = null;
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

            featureIndex = newFeatureIndex;
        }

        public virtual void PrintFullFeatureMatrix(TextWriter pw)
        {
            string sep = @"\t";
            for (int i = 0; i < featureIndex.Size(); i++)
            {
                pw.Write(sep + featureIndex.Get(i));
            }

            pw.WriteLine();
            for (int i = 0; i < labels.Length; i++)
            {
                pw.Write(labelIndex.Get(i));
                ISet<int> feats = new HashSet<int>();
                for (int j = 0; j < data[i].Length; j++)
                {
                    int feature = data[i][j];
                    feats.Add(feature);
                }

                for (int j = 0; j < featureIndex.Size(); j++)
                {
                    if (feats.Contains(j))
                    {
                        pw.Write(sep + '1');
                    }
                    else
                    {
                        pw.Write(sep + '0');
                    }
                }
            }
        }

        public virtual void PrintSparseFeatureMatrix()
        {
            PrintSparseFeatureMatrix(Console.Out);
        }

        public virtual void PrintSparseFeatureMatrix(TextWriter pw)
        {
            string sep = @"\t";
            for (int i = 0; i < size; i++)
            {
                pw.Write(labelIndex.Get(labels[i]));
                int[] datum = data[i];
                foreach (int j in datum)
                {
                    pw.Write(sep + featureIndex.Get(j));
                }

                pw.WriteLine();
            }
        }

        public virtual void ChangeLabelIndex(IIndex<L> newLabelIndex)
        {
            labels = TrimToSize(labels);
            for (int i = 0; i < labels.Length; i++)
            {
                labels[i] = newLabelIndex.IndexOf(labelIndex.Get(labels[i]));
            }

            labelIndex = newLabelIndex;
        }

        public virtual void ChangeFeatureIndex(IIndex<F> newFeatureIndex)
        {
            data = TrimToSize(data);
            labels = TrimToSize(labels);
            int[][] newData = new int[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                int[] newD = new int[data[i].Length];
                int k = 0;
                for (int j = 0; j < data[i].Length; j++)
                {
                    int newIndex = newFeatureIndex.IndexOf(featureIndex.Get(data[i][j]));
                    if (newIndex >= 0)
                    {
                        newD[k++] = newIndex;
                    }
                }

                newData[i] = new int[k];
                Array.Copy(newD, 0, newData[i], 0, k);
            }

            data = newData;
            featureIndex = newFeatureIndex;
        }

        public virtual void SelectFeaturesBinaryInformationGain(int numFeatures)
        {
            double[] scores = GetInformationGains();
            SelectFeatures(numFeatures, scores);
        }

        public virtual void SelectFeatures(int numFeatures, double[] scores)
        {
            List<ScoredObject<F>> scoredFeatures = new List<ScoredObject<F>>();
            for (int i = 0; i < scores.Length; i++)
            {
                scoredFeatures.Add(new ScoredObject<F>(featureIndex.Get(i), scores[i]));
            }

            scoredFeatures.Sort(ScoredComparator.DESCENDING_COMPARATOR);
            IIndex<F> newFeatureIndex = new HashIndex<F>();
            for (int i = 0; i < scoredFeatures.Size() && i < numFeatures; i++)
            {
                newFeatureIndex.Add(scoredFeatures.Get(i).Object());
            }

            for (int i = 0; i < size; i++)
            {
                int[] newData = new int[data[i].Length];
                int curIndex = 0;
                for (int j = 0; j < data[i].Length; j++)
                {
                    int index;
                    if ((index = newFeatureIndex.IndexOf(featureIndex.Get(data[i][j]))) != -1)
                    {
                        newData[curIndex++] = index;
                    }
                }

                int[] newDataTrimmed = new int[curIndex];
                Array.Copy(newData, 0, newDataTrimmed, 0, curIndex);
                data[i] = newDataTrimmed;
            }

            featureIndex = newFeatureIndex;
        }

        public virtual double[] GetInformationGains()
        {
            data = TrimToSize(data);
            labels = TrimToSize(labels);
            ClassicCounter<F> featureCounter = new ClassicCounter<F>();
            ClassicCounter<L> labelCounter = new ClassicCounter<L>();
            TwoDimensionalCounter<F, L> condCounter = new TwoDimensionalCounter<F, L>();
            for (int i = 0; i < labels.Length; i++)
            {
                labelCounter.IncrementCount(labelIndex.Get(labels[i]));
                bool[] doc = new bool[featureIndex.Size()];
                for (int j = 0; j < data[i].Length; j++)
                {
                    doc[data[i][j]] = true;
                }

                for (int j = 0; j < doc.Length; j++)
                {
                    if (doc[j])
                    {
                        featureCounter.IncrementCount(featureIndex.Get(j));
                        condCounter.IncrementCount(featureIndex.Get(j), labelIndex.Get(labels[i]), 1.0);
                    }
                }
            }

            double entropy = 0.0;
            for (int i = 0; i < labelIndex.Size(); i++)
            {
                double labelCount = labelCounter.GetCount(labelIndex.Get(i));
                double p = labelCount / Size();
                entropy -= p * (System.Math.Log(p) / System.Math.Log(2));
            }

            double[] ig = new double[featureIndex.Size()];
            Arrays.Fill(ig, entropy);
            for (int i = 0; i < featureIndex.Size(); i++)
            {
                F feature = featureIndex.Get(i);
                double featureCount = featureCounter.GetCount(feature);
                double notFeatureCount = Size() - featureCount;
                double pFeature = featureCount / Size();
                double pNotFeature = (1.0 - pFeature);
                if (featureCount == 0)
                {
                    ig[i] = 0;
                    continue;
                }

                if (notFeatureCount == 0)
                {
                    ig[i] = 0;
                    continue;
                }

                double sumFeature = 0.0;
                double sumNotFeature = 0.0;
                for (int j = 0; j < labelIndex.Size(); j++)
                {
                    L label = labelIndex.Get(j);
                    double featureLabelCount = condCounter.GetCount(feature, label);
                    double notFeatureLabelCount = Size() - featureLabelCount;
                    double p = featureLabelCount / featureCount;
                    double pNot = notFeatureLabelCount / notFeatureCount;
                    if (featureLabelCount != 0)
                    {
                        sumFeature += p * (System.Math.Log(p) / System.Math.Log(2));
                    }

                    if (notFeatureLabelCount != 0)
                    {
                        sumNotFeature += pNot * (System.Math.Log(pNot) / System.Math.Log(2));
                    }
                }

                ig[i] += pFeature * sumFeature + pNotFeature * sumNotFeature;
            }

            return ig;
        }

        public virtual void UpdateLabels(int[] labels)
        {
            if (labels.Length != Size())
                throw new ArgumentException(@"size of labels array does not match dataset size");
            this.labels = labels;
        }

        public override string ToString()
        {
            return @"Dataset of size " + size;
        }

        public virtual string ToSummaryString()
        {
            StringWriter sw = new StringWriter();
            TextWriter pw = sw;
            pw.WriteLine(@"Number of data points: " + Size());
            pw.WriteLine(@"Number of active feature tokens: " + NumFeatureTokens());
            pw.WriteLine(@"Number of active feature types:" + NumFeatureTypes());
            return pw.ToString();
        }

        public static void PrintSVMLightFormat(TextWriter pw, ClassicCounter<Integer> c, int classNo)
        {
            int[] features = c.KeySet().ToArray();
            Array.Sort(features);
            StringBuilder sb = new StringBuilder();
            sb.Append(classNo);
            sb.Append(' ');
            foreach (int f in features)
            {
                sb.Append(f + 1).Append(':').Append(c.GetCount(f)).Append(' ');
            }

            pw.WriteLine(sb.ToString());
        }
    }
}
