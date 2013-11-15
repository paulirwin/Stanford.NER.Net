using Stanford.NER.Net.Ling;
using Stanford.NER.Net.ObjectBank;
using Stanford.NER.Net.Sequences;
using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class CRFClassifier<IN> : AbstractSequenceClassifier<IN>
        where IN : ICoreMap
    {
        List<IIndex<CRFLabel>> labelIndices;
        IIndex<String> tagIndex;
        Tuple<double[][], double[][]> entityMatrices;
        ICliquePotentialFunction cliquePotentialFunction;
        IHasCliquePotentialFunction cliquePotentialFunctionHelper;
        double[][] weights;
        double[][] linearWeights;
        double[][] inputLayerWeights4Edge;
        double[][] outputLayerWeights4Edge;
        double[][] inputLayerWeights;
        double[][] outputLayerWeights;
        IIndex<String> featureIndex;
        int[] map;
        IList<ISet<int>> featureIndicesSetArray;
        IList<IList<int>> featureIndicesListArray;
        Random random = new Random(2147483647);
        IIndex<int> nodeFeatureIndicesMap;
        IIndex<int> edgeFeatureIndicesMap;
        IDictionary<String, double[]> embeddings;
        public static readonly string DEFAULT_CLASSIFIER = @"edu/stanford/nlp/models/ner/english.all.3class.distsim.crf.ser.gz";
        private static readonly bool VERBOSE = false;
        Regex suffixPatt = new Regex(".+?((?:-[A-Z]+)+)\\|.*C");
        IIndex<String> templateGroupIndex;
        IDictionary<int, int> featureIndexToTemplateIndex;

        protected CRFClassifier()
            : base(new SeqClassifierFlags())
        {
        }

        //public CRFClassifier(Properties props)
        //    : base(props)
        //{
        //}

        public CRFClassifier(SeqClassifierFlags flags)
            : base(flags)
        {
        }

        public CRFClassifier(CRFClassifier<IN> crf)
            : base(crf.flags)
        {
            this.windowSize = crf.windowSize;
            this.featureFactory = crf.featureFactory;
            this.pad = crf.pad;
            this.knownLCWords = (crf.knownLCWords != null) ? new HashSet<string>(crf.knownLCWords) : null;
            this.featureIndex = (crf.featureIndex != null) ? new HashIndex<String>(crf.featureIndex.ObjectsList()) : null;
            if (crf.flags.nonLinearCRF)
            {
                this.nodeFeatureIndicesMap = (crf.nodeFeatureIndicesMap != null) ? new HashIndex<int>(crf.nodeFeatureIndicesMap.ObjectsList()) : null;
                this.edgeFeatureIndicesMap = (crf.edgeFeatureIndicesMap != null) ? new HashIndex<int>(crf.edgeFeatureIndicesMap.ObjectsList()) : null;
            }

            this.classIndex = (crf.classIndex != null) ? new HashIndex<String>(crf.classIndex.ObjectsList()) : null;
            if (crf.labelIndices != null)
            {
                this.labelIndices = new List<IIndex<CRFLabel>>(crf.labelIndices.Size());
                for (int i = 0; i < crf.labelIndices.Size(); i++)
                {
                    this.labelIndices.Add((crf.labelIndices.Get(i) != null) ? new HashIndex<CRFLabel>(crf.labelIndices.Get(i).ObjectsList()) : null);
                }
            }
            else
            {
                this.labelIndices = null;
            }

            this.cliquePotentialFunction = crf.cliquePotentialFunction;
        }

        public virtual int GetNumWeights()
        {
            if (weights == null)
                return 0;
            int numWeights = 0;
            foreach (double[] wts in weights)
            {
                numWeights += wts.Length;
            }

            return numWeights;
        }

        private int GetFeatureTypeIndex(int i)
        {
            return GetFeatureTypeIndex(featureIndex.Get(i));
        }

        private static int GetFeatureTypeIndex(string feature)
        {
            if (feature.EndsWith(@"|C"))
            {
                return 0;
            }
            else if (feature.EndsWith(@"|CpC"))
            {
                return 1;
            }
            else if (feature.EndsWith(@"|Cp2C"))
            {
                return 2;
            }
            else if (feature.EndsWith(@"|Cp3C"))
            {
                return 3;
            }
            else if (feature.EndsWith(@"|Cp4C"))
            {
                return 4;
            }
            else if (feature.EndsWith(@"|Cp5C"))
            {
                return 5;
            }
            else
            {
                throw new Exception(@"Unknown feature type " + feature);
            }
        }

        public virtual void ScaleWeights(double scale)
        {
            for (int i = 0; i < weights.Length; i++)
            {
                for (int j = 0; j < weights[i].Length; j++)
                {
                    weights[i][j] *= scale;
                }
            }
        }

        private void CombineWeights(CRFClassifier<IN> crf, double weight)
        {
            int numFeatures = featureIndex.Size();
            int oldNumFeatures = weights.Length;
            IDictionary<CRFLabel, CRFLabel> crfLabelMap = new HashMap<CRFLabel, CRFLabel>();
            for (int i = 0; i < crf.labelIndices.Count; i++)
            {
                for (int j = 0; j < crf.labelIndices[i].Size(); j++)
                {
                    CRFLabel labels = crf.labelIndices[i].Get(j);
                    int[] newLabelIndices = new int[i + 1];
                    for (int ci = 0; ci <= i; ci++)
                    {
                        string classLabel = crf.classIndex.Get(labels.GetLabel()[ci]);
                        newLabelIndices[ci] = this.classIndex.IndexOf(classLabel);
                    }

                    CRFLabel newLabels = new CRFLabel(newLabelIndices);
                    crfLabelMap[labels] = newLabels;
                    int k = this.labelIndices[i].IndexOf(newLabels);
                }
            }

            map = new int[numFeatures];
            for (int i = 0; i < numFeatures; i++)
            {
                map[i] = GetFeatureTypeIndex(i);
            }

            double[][] newWeights = new double[numFeatures][];
            for (int i = 0; i < numFeatures; i++)
            {
                int length = labelIndices[map[i]].Size();
                newWeights[i] = new double[length];
                if (i < oldNumFeatures)
                {
                    Array.Copy(weights[i], 0, newWeights[i], 0, weights[i].Length);
                }
            }

            weights = newWeights;
            for (int i = 0; i < crf.weights.Length; i++)
            {
                string feature = crf.featureIndex.Get(i);
                int newIndex = featureIndex.IndexOf(feature);
                if (weights[newIndex].Length < crf.weights[i].Length)
                {
                    throw new Exception(@"Incompatible CRFClassifier: weight length mismatch for feature " + newIndex + @": " + featureIndex.Get(newIndex) + @" (also feature " + i + @": " + crf.featureIndex.Get(i) + @") " + @", len1=" + weights[newIndex].length + @", len2=" + crf.weights[i].length);
                }

                int featureTypeIndex = map[newIndex];
                for (int j = 0; j < crf.weights[i].Length; j++)
                {
                    CRFLabel labels = crf.labelIndices[featureTypeIndex].Get(j);
                    CRFLabel newLabels = crfLabelMap[labels];
                    int k = this.labelIndices[featureTypeIndex].IndexOf(newLabels);
                    weights[newIndex][k] += crf.weights[i][j] * weight;
                }
            }
        }

        public virtual void Combine(CRFClassifier<IN> crf, double weight)
        {
            Timing timer = new Timing();
            if (!this.pad.Equals(crf.pad))
            {
                throw new Exception(@"Incompatible CRFClassifier: pad does not match");
            }

            if (this.windowSize != crf.windowSize)
            {
                throw new Exception(@"Incompatible CRFClassifier: windowSize does not match");
            }

            if (this.labelIndices.Size() != crf.labelIndices.Size())
            {
                throw new Exception(@"Incompatible CRFClassifier: labelIndices length does not match");
            }

            this.classIndex.AddAll(crf.classIndex.ObjectsList());
            int oldNumFeatures1 = this.featureIndex.Size();
            int oldNumFeatures2 = crf.featureIndex.Size();
            int oldNumWeights1 = this.GetNumWeights();
            int oldNumWeights2 = crf.GetNumWeights();
            this.featureIndex.AddAll(crf.featureIndex.ObjectsList());
            this.knownLCWords.AddAll(crf.knownLCWords);
            for (int i = 0; i < labelIndices.Size(); i++)
            {
                this.labelIndices.Get(i).AddAll(crf.labelIndices.Get(i).ObjectsList());
            }

            Console.Error.WriteLine(@"Combining weights: will automatically match labelIndices");
            CombineWeights(crf, weight);
            int numFeatures = featureIndex.Size();
            int numWeights = GetNumWeights();
            long elapsedMs = timer.Stop();
            Console.Error.WriteLine(@"numFeatures: orig1=" + oldNumFeatures1 + @", orig2=" + oldNumFeatures2 + @", combined=" + numFeatures);
            Console.Error.WriteLine(@"numWeights: orig1=" + oldNumWeights1 + @", orig2=" + oldNumWeights2 + @", combined=" + numWeights);
            Console.Error.WriteLine(@"Time to combine CRFClassifier: " + Timing.ToSecondsString(elapsedMs) + @" seconds");
        }

        public virtual void DropFeaturesBelowThreshold(double threshold)
        {
            IIndex<String> newFeatureIndex = new HashIndex<String>();
            for (int i = 0; i < weights.Length; i++)
            {
                double smallest = weights[i][0];
                double biggest = weights[i][0];
                for (int j = 1; j < weights[i].Length; j++)
                {
                    if (weights[i][j] > biggest)
                    {
                        biggest = weights[i][j];
                    }

                    if (weights[i][j] < smallest)
                    {
                        smallest = weights[i][j];
                    }

                    if (biggest - smallest > threshold)
                    {
                        newFeatureIndex.Add(featureIndex.Get(i));
                        break;
                    }
                }
            }

            int[] newMap = new int[newFeatureIndex.Size()];
            for (int i = 0; i < newMap.Length; i++)
            {
                int index = featureIndex.IndexOf(newFeatureIndex.Get(i));
                newMap[i] = map[index];
            }

            map = newMap;
            featureIndex = newFeatureIndex;
        }

        public virtual Tuple<int[][][], int[], double[][][]> DocumentToDataAndLabels(List<IN> document)
        {
            return DocumentToDataAndLabels(document, false);
        }

        public virtual Tuple<int[][][], int[], double[][][]> DocumentToDataAndLabels(List<IN> document, bool trainTime)
        {
            bool droppedFeature = false;
            int docSize = document.Count;
            int[][][] data = new int[docSize][][];
            double[][][] featureVals = new double[docSize][][];
            int[] labels = new int[docSize];
            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            for (int j = 0; j < docSize; j++)
            {
                CRFDatum<List<String>, CRFLabel> d = MakeDatum(document, j, featureFactory);
                List<List<String>> features = d.AsFeatures();
                List<double[]> featureValList = d.AsFeatureVals();
                data[j] = new int[windowSize][];
                featureVals[j] = new double[windowSize][];
                for (int k = 0, fSize = features.Count; k < fSize; k++)
                {
                    ICollection<String> cliqueFeatures = features.Get(k);
                    data[j][k] = new int[cliqueFeatures.Size()];
                    if (featureValList != null)
                    {
                        featureVals[j][k] = featureValList[k];
                    }

                    int m = 0;
                    foreach (string feature in cliqueFeatures)
                    {
                        int index = featureIndex.IndexOf(feature);
                        if (index >= 0)
                        {
                            data[j][k][m] = index;
                            m++;
                        }
                        else
                        {
                        }
                    }

                    if (m < data[j][k].Length)
                    {
                        int[] f = new int[m];
                        Array.Copy(data[j][k], 0, f, 0, m);
                        data[j][k] = f;
                        if (featureVals[j][k] != null)
                        {
                            double[] fVal = new double[m];
                            Array.Copy(featureVals[j][k], 0, fVal, 0, m);
                            featureVals[j][k] = fVal;
                        }
                    }
                }

                IN wi = document[j];
                labels[j] = classIndex.IndexOf(wi.Get(typeof(CoreAnnotations.AnswerAnnotation)));
            }

            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            if (flags.nonLinearCRF)
            {
                data = TransformDocData(data);
            }

            return new Tuple<int[][][], int[], double[][][]>(data, labels, featureVals);
        }

        private int[][][] TransformDocData(int[][][] docData)
        {
            int[][][] transData = new int[docData.Length][][];
            for (int i = 0; i < docData.Length; i++)
            {
                transData[i] = new int[docData[i].Length][];
                for (int j = 0; j < docData[i].Length; j++)
                {
                    int[] cliqueFeatures = docData[i][j];
                    transData[i][j] = new int[cliqueFeatures.Length];
                    for (int n = 0; n < cliqueFeatures.Length; n++)
                    {
                        int transFeatureIndex = -1;
                        if (j == 0)
                        {
                            transFeatureIndex = nodeFeatureIndicesMap.IndexOf(cliqueFeatures[n]);
                            if (transFeatureIndex == -1)
                                throw new Exception(@"node cliqueFeatures[n]=" + cliqueFeatures[n] + @" not found, nodeFeatureIndicesMap.size=" + nodeFeatureIndicesMap.Size());
                        }
                        else
                        {
                            transFeatureIndex = edgeFeatureIndicesMap.IndexOf(cliqueFeatures[n]);
                            if (transFeatureIndex == -1)
                                throw new Exception(@"edge cliqueFeatures[n]=" + cliqueFeatures[n] + @" not found, edgeFeatureIndicesMap.size=" + edgeFeatureIndicesMap.Size());
                        }

                        transData[i][j][n] = transFeatureIndex;
                    }
                }
            }

            return transData;
        }

        public virtual void PrintLabelInformation(string testFile, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFile(testFile, readerAndWriter);
            foreach (List<IN> document in documents)
            {
                PrintLabelValue(document);
            }
        }

        public virtual void PrintLabelValue(List<IN> document)
        {
            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            NumberFormat nf = new DecimalFormat();
            List<String> classes = new List<String>();
            for (int i = 0; i < classIndex.Size(); i++)
            {
                classes.Add(classIndex.Get(i));
            }

            String[] columnHeaders = classes.ToArray();
            for (int j = 0; j < document.Count; j++)
            {
                Console.Out.WriteLine(@"--== " + document[j].Get(typeof(CoreAnnotations.TextAnnotation)) + @" ==--");
                List<String[]> lines = new List<String[]>();
                List<String> rowHeaders = new List<String>();
                List<String> line = new List<String>();
                for (int p = 0; p < labelIndices.Count; p++)
                {
                    if (j + p >= document.Count)
                    {
                        continue;
                    }

                    CRFDatum<List<String>, CRFLabel> d = MakeDatum(document, j + p, featureFactory);
                    List<List<String>> features = d.AsFeatures();
                    for (int k = p, fSize = features.Count; k < fSize; k++)
                    {
                        ICollection<String> cliqueFeatures = features[k];
                        foreach (string feature in cliqueFeatures)
                        {
                            int index = featureIndex.IndexOf(feature);
                            if (index >= 0)
                            {
                                rowHeaders.Add(feature + '[' + (-p) + ']');
                                double[] values = new double[labelIndices[0].Size()];
                                foreach (CRFLabel label in labelIndices[k])
                                {
                                    int[] l = label.GetLabel();
                                    double v = weights[index][labelIndices[k].IndexOf(label)];
                                    values[l[l.Length - 1 - p]] += v;
                                }

                                foreach (double value in values)
                                {
                                    line.Add(nf.Format(value));
                                }

                                lines.Add(line.ToArray());
                                line = new List<String>();
                            }
                        }
                    }

                    Console.Out.WriteLine(StringUtils.MakeAsciiTable(lines.ToArray(), rowHeaders.ToArray(), columnHeaders, 0, 1, true));
                    Console.Out.WriteLine();
                }
            }

            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }
        }

        public virtual Tuple<int[][][][], int[][], double[][][][]> DocumentsToDataAndLabels(ICollection<List<IN>> documents)
        {
            List<int[][][]> data = new List<int[][][]>();
            List<double[][][]> featureVal = new List<double[][][]>();
            List<int[]> labels = new List<int[]>();
            int numDatums = 0;
            foreach (List<IN> doc in documents)
            {
                Tuple<int[][][], int[], double[][][]> docTriple = DocumentToDataAndLabels(doc, true);
                data.Add(docTriple.Item1);
                labels.Add(docTriple.Item2);
                if (flags.useEmbedding)
                    featureVal.Add(docTriple.Item3);
                numDatums += doc.Count;
            }

            Console.Error.WriteLine(@"numClasses: " + classIndex.Size() + ' ' + classIndex);
            Console.Error.WriteLine(@"numDocuments: " + data.Count);
            Console.Error.WriteLine(@"numDatums: " + numDatums);
            Console.Error.WriteLine(@"numFeatures: " + featureIndex.Size());
            PrintFeatures();
            double[][][][] featureValArr = null;
            if (flags.useEmbedding)
                featureValArr = featureVal.ToArray();
            return new Tuple<int[][][][], int[][], double[][][][]>(data.ToArray(), labels.ToArray(), featureValArr);
        }

        public virtual List<Tuple<int[][][], int[], double[][][]>> DocumentsToDataAndLabelsList(ICollection<List<IN>> documents)
        {
            int numDatums = 0;
            List<Tuple<int[][][], int[], double[][][]>> docList = new List<Tuple<int[][][], int[], double[][][]>>();
            foreach (List<IN> doc in documents)
            {
                Tuple<int[][][], int[], double[][][]> docTriple = DocumentToDataAndLabels(doc);
                docList.Add(docTriple);
                numDatums += doc.Count;
            }

            Console.Error.WriteLine(@"numClasses: " + classIndex.Size() + ' ' + classIndex);
            Console.Error.WriteLine(@"numDocuments: " + docList.Count);
            Console.Error.WriteLine(@"numDatums: " + numDatums);
            Console.Error.WriteLine(@"numFeatures: " + featureIndex.Size());
            return docList;
        }

        protected virtual void PrintFeatures()
        {
            if (flags.printFeatures == null)
            {
                return;
            }

            try
            {
                string enc = flags.inputEncoding;
                if (flags.inputEncoding == null)
                {
                    Console.Error.WriteLine(@"flags.inputEncoding doesn't exist, Use UTF-8 as default");
                    enc = @"UTF-8";
                }

                using (var fs = new FileStream("features-" + flags.printFeatures + ".txt", FileMode.OpenOrCreate, FileAccess.Write))
                using (var pw = new StreamWriter(fs, Encoding.GetEncoding(enc)))
                {
                    foreach (string feat in featureIndex)
                    {
                        pw.WriteLine(feat);
                    }
                }
            }
            catch (IOException ioe)
            {
                //ioe.PrintStackTrace();
            }
        }

        protected virtual void MakeAnswerArraysAndTagIndex(ICollection<List<IN>> ob)
        {
            ISet<String>[] featureIndices = new HashSet<string>[windowSize];
            for (int i = 0; i < windowSize; i++)
            {
                featureIndices[i] = new HashSet<string>();
            }

            labelIndices = new List<IIndex<CRFLabel>>(windowSize);
            for (int i = 0; i < windowSize; i++)
            {
                labelIndices.Add(new HashIndex<CRFLabel>());
            }

            IIndex<CRFLabel> labelIndex = labelIndices.Get(windowSize - 1);
            classIndex = new HashIndex<String>();
            classIndex.Add(flags.backgroundSymbol);
            ISet<String>[] seenBackgroundFeatures = new HashSet<string>[2];
            seenBackgroundFeatures[0] = new HashSet<string>();
            seenBackgroundFeatures[1] = new HashSet<string>();
            int wordCount = 0;
            foreach (List<IN> doc in ob)
            {
                if (flags.useReverse)
                {
                    Collections.Reverse(doc);
                }

                foreach (IN token in doc)
                {
                    wordCount++;
                    string ans = token.Get<string>(typeof(CoreAnnotations.AnswerAnnotation));
                    if (ans == null || ans.Equals(@""))
                    {
                        throw new ArgumentException(@"Word " + wordCount + " (\"" + token.Get<string>(typeof(CoreAnnotations.TextAnnotation)) + "\") has a blank answer");
                    }

                    classIndex.Add(ans);
                }

                for (int j = 0, docSize = doc.Size(); j < docSize; j++)
                {
                    CRFDatum<List<String>, CRFLabel> d = MakeDatum(doc, j, featureFactory);
                    labelIndex.Add(d.Label());
                    List<List<String>> features = d.AsFeatures();
                    for (int k = 0, fSize = features.Size(); k < fSize; k++)
                    {
                        ICollection<String> cliqueFeatures = features.Get(k);
                        if (k < 2 && flags.removeBackgroundSingletonFeatures)
                        {
                            string ans = doc.Get(j).Get<string>(typeof(CoreAnnotations.AnswerAnnotation));
                            bool background = ans.Equals(flags.backgroundSymbol);
                            if (k == 1 && j > 0 && background)
                            {
                                ans = doc.Get(j - 1).Get<string>(typeof(CoreAnnotations.AnswerAnnotation));
                                background = ans.Equals(flags.backgroundSymbol);
                            }

                            if (background)
                            {
                                foreach (string f in cliqueFeatures)
                                {
                                    if (!featureIndices[k].Contains(f))
                                    {
                                        if (seenBackgroundFeatures[k].Contains(f))
                                        {
                                            seenBackgroundFeatures[k].Remove(f);
                                            featureIndices[k].Add(f);
                                        }
                                        else
                                        {
                                            seenBackgroundFeatures[k].Add(f);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                seenBackgroundFeatures[k].RemoveAll(cliqueFeatures);
                                featureIndices[k].AddAll(cliqueFeatures);
                            }
                        }
                        else
                        {
                            featureIndices[k].AddAll(cliqueFeatures);
                        }
                    }
                }

                if (flags.useReverse)
                {
                    Collections.Reverse(doc);
                }
            }

            int numFeatures = 0;
            for (int i = 0; i < windowSize; i++)
            {
                numFeatures += featureIndices[i].Size();
            }

            featureIndex = new HashIndex<String>();
            map = new int[numFeatures];
            if (flags.groupByFeatureTemplate)
            {
                templateGroupIndex = new HashIndex<String>();
                featureIndexToTemplateIndex = new HashMap<int, int>();
            }

            Matcher m = null;
            string groupSuffix = null;
            for (int i = 0; i < windowSize; i++)
            {
                IIndex<int> featureIndexMap = new HashIndex<int>();
                featureIndex.AddAll(featureIndices[i]);
                foreach (string str in featureIndices[i])
                {
                    int index = featureIndex.IndexOf(str);
                    map[index] = i;
                    featureIndexMap.Add(index);
                    if (flags.groupByFeatureTemplate)
                    {
                        m = suffixPatt.Matcher(str);
                        groupSuffix = @"NoTemplate";
                        if (m.Matches())
                        {
                            groupSuffix = m.Group(1);
                        }

                        groupSuffix += @"-c:" + i;
                        int groupIndex = templateGroupIndex.IndexOf(groupSuffix, true);
                        featureIndexToTemplateIndex.Put(index, groupIndex);
                    }
                }

                if (i == 0)
                {
                    nodeFeatureIndicesMap = featureIndexMap;
                    Console.Error.WriteLine(@"setting nodeFeatureIndicesMap, size=" + nodeFeatureIndicesMap.Size());
                }
                else
                {
                    edgeFeatureIndicesMap = featureIndexMap;
                    Console.Error.WriteLine(@"setting edgeFeatureIndicesMap, size=" + edgeFeatureIndicesMap.Size());
                }
            }

            if (flags.numOfFeatureSlices > 0)
            {
                Console.Error.WriteLine(@"Taking " + flags.numOfFeatureSlices + @" out of " + flags.totalFeatureSlice + @" slices of node features for training");
                PruneNodeFeatureIndices(flags.totalFeatureSlice, flags.numOfFeatureSlices);
            }

            if (flags.useObservedSequencesOnly)
            {
                for (int i = 0, liSize = labelIndex.Size(); i < liSize; i++)
                {
                    CRFLabel label = labelIndex.Get(i);
                    for (int j = windowSize - 2; j >= 0; j--)
                    {
                        label = label.GetOneSmallerLabel();
                        labelIndices.Get(j).Add(label);
                    }
                }
            }
            else
            {
                for (int i = 0; i < labelIndices.Size(); i++)
                {
                    labelIndices.Set(i, AllLabels(i + 1, classIndex));
                }
            }

            if (VERBOSE)
            {
                for (int i = 0, fiSize = featureIndex.Size(); i < fiSize; i++)
                {
                    Console.Out.WriteLine(i + @": " + featureIndex.Get(i));
                }
            }
        }

        protected static IIndex<CRFLabel> AllLabels(int window, IIndex<String> classIndex)
        {
            int[] label = new int[window];
            int numClasses = classIndex.Size();
            IIndex<CRFLabel> labelIndex = new HashIndex<CRFLabel>();
        
            while (true)
            {
                CRFLabel l = new CRFLabel(label);
                labelIndex.Add(l);
                int[] label1 = new int[window];
                Array.Copy(label, 0, label1, 0, label.Length);
                label = label1;

                bool shouldBreakWhile = false;

                for (int j = 0; j < label.Length; j++)
                {
                    label[j]++;
                    if (label[j] >= numClasses)
                    {
                        label[j] = 0;
                        if (j == label.Length - 1)
                        {
                            shouldBreakWhile = true;
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                if (shouldBreakWhile)
                    break;
            }

            return labelIndex;
        }

        public virtual CRFDatum<List<String>, CRFLabel> MakeDatum(List<IN> info, int loc, FeatureFactory<IN> featureFactory)
        {
            PaddedList<IN> pInfo = new PaddedList<IN>(info, pad);
            List<List<String>> features = new List<List<String>>();
            List<double[]> featureVals = new List<double[]>();
            ISet<Clique> done = new HashSet<Clique>();
            for (int i = 0; i < windowSize; i++)
            {
                List<String> featuresC = new List<String>();
                List<Clique> windowCliques = FeatureFactory<IN>.GetCliques(i, 0);
                windowCliques.RemoveAll(done);
                done.AddAll(windowCliques);
                double[] featureValArr = null;
                if (flags.useEmbedding && i == 0)
                {
                    List<double[]> embeddingList = new List<double[]>();
                    int concatEmbeddingLen = 0;
                    string currentWord = null;
                    for (int currLoc = loc - 2; currLoc <= loc + 2; currLoc++)
                    {
                        double[] embedding = null;
                        if (currLoc >= 0 && currLoc < info.Size())
                        {
                            currentWord = info.Get(loc).Get<string>(typeof(CoreAnnotations.TextAnnotation));
                            string word = currentWord.ToLower();
                            word = word.ReplaceAll(@"(-)?\\d+(\\.\\d*)?", @"0");
                            if (embeddings.ContainsKey(word))
                                embedding = embeddings.Get(word);
                            else
                                embedding = embeddings.Get(@"UNKNOWN");
                        }
                        else
                        {
                            embedding = embeddings.Get(@"PADDING");
                        }

                        for (int e = 0; e < embedding.Length; e++)
                        {
                            featuresC.Add(@"EMBEDDING-(" + (currLoc - loc) + @")-" + e);
                        }

                        if (flags.addCapitalFeatures)
                        {
                            int numOfCapitalFeatures = 4;
                            double[] newEmbedding = new double[embedding.Length + numOfCapitalFeatures];
                            int currLen = embedding.Length;
                            Array.Copy(embedding, 0, newEmbedding, 0, currLen);
                            for (int e = 0; e < numOfCapitalFeatures; e++)
                                featuresC.Add(@"CAPITAL-(" + (currLoc - loc) + @")-" + e);
                            if (currLoc >= 0 && currLoc < info.Size())
                            {
                                if (currentWord.ToUpper().Equals(currentWord))
                                    newEmbedding[currLen] = 1;
                                else
                                {
                                    currLen += 1;
                                    if (currentWord.ToLower().Equals(currentWord))
                                        newEmbedding[currLen] = 1;
                                    else
                                    {
                                        currLen += 1;
                                        if (char.IsUpper(currentWord[0]))
                                            newEmbedding[currLen] = 1;
                                        else
                                        {
                                            currLen += 1;
                                            string remainder = currentWord.Substring(1);
                                            if (!remainder.ToLower().Equals(remainder))
                                                newEmbedding[currLen] = 1;
                                        }
                                    }
                                }
                            }

                            embedding = newEmbedding;
                        }

                        embeddingList.Add(embedding);
                        concatEmbeddingLen += embedding.Length;
                    }

                    double[] concatEmbedding = new double[concatEmbeddingLen];
                    int currPos = 0;
                    foreach (double[] em in embeddingList)
                    {
                        Array.Copy(em, 0, concatEmbedding, currPos, em.Length);
                        currPos += em.Length;
                    }

                    if (flags.prependEmbedding)
                    {
                        int additionalFeatureCount = 0;
                        foreach (Clique c in windowCliques)
                        {
                            ICollection<String> fCol = featureFactory.GetCliqueFeatures(pInfo, loc, c);
                            featuresC.AddRange(fCol);
                            additionalFeatureCount += fCol.Size();
                        }

                        featureValArr = new double[concatEmbedding.Length + additionalFeatureCount];
                        Array.Copy(concatEmbedding, 0, featureValArr, 0, concatEmbedding.Length);
                        Arrays.Fill(featureValArr, concatEmbedding.Length, featureValArr.Length, 1.0);
                    }
                    else
                    {
                        featureValArr = concatEmbedding;
                    }

                    if (flags.addBiasToEmbedding)
                    {
                        featuresC.Add(@"BIAS-FEATURE");
                        double[] newFeatureValArr = new double[featureValArr.Length + 1];
                        Array.Copy(featureValArr, 0, newFeatureValArr, 0, featureValArr.Length);
                        newFeatureValArr[newFeatureValArr.Length - 1] = 1;
                        featureValArr = newFeatureValArr;
                    }
                }
                else
                {
                    foreach (Clique c in windowCliques)
                    {
                        featuresC.AddRange(featureFactory.GetCliqueFeatures(pInfo, loc, c));
                    }
                }

                features.Add(featuresC);
                featureVals.Add(featureValArr);
            }

            int[] labels = new int[windowSize];
            for (int i = 0; i < windowSize; i++)
            {
                string answer = pInfo.Get(loc + i - windowSize + 1).Get<string>(typeof(CoreAnnotations.AnswerAnnotation));
                labels[i] = classIndex.IndexOf(answer);
            }

            PrintFeatureLists(pInfo.Get(loc), features);
            CRFDatum<List<String>, CRFLabel> d = new CRFDatum<List<String>, CRFLabel>(features, new CRFLabel(labels), featureVals);
            return d;
        }

        public class TestSequenceModel : ISequenceModel
        {
            private readonly int window;
            private readonly int numClasses;
            private readonly CRFCliqueTree cliqueTree;
            private readonly int[] tags;
            private readonly int[] backgroundTag;
            public TestSequenceModel(CRFCliqueTree cliqueTree)
            {
                this.cliqueTree = cliqueTree;
                this.window = cliqueTree.Window();
                this.numClasses = cliqueTree.GetNumClasses();
                tags = new int[numClasses];
                for (int i = 0; i < tags.Length; i++)
                {
                    tags[i] = i;
                }

                backgroundTag = new int[] { cliqueTree.BackgroundIndex() };
            }

            public int Length()
            {
                return cliqueTree.Length();
            }

            public int LeftWindow()
            {
                return window - 1;
            }

            public int RightWindow()
            {
                return 0;
            }

            public int[] GetPossibleValues(int pos)
            {
                if (pos < window - 1)
                {
                    return backgroundTag;
                }

                return tags;
            }

            public double ScoreOf(int[] tags, int pos)
            {
                int[] previous = new int[window - 1];
                int realPos = pos - window + 1;
                for (int i = 0; i < window - 1; i++)
                {
                    previous[i] = tags[realPos + i];
                }

                return cliqueTree.CondLogProbGivenPrevious(realPos, tags[pos], previous);
            }

            public double[] ScoresOf(int[] tags, int pos)
            {
                int realPos = pos - window + 1;
                double[] scores = new double[numClasses];
                int[] previous = new int[window - 1];
                for (int i = 0; i < window - 1; i++)
                {
                    previous[i] = tags[realPos + i];
                }

                for (int i = 0; i < numClasses; i++)
                {
                    scores[i] = cliqueTree.CondLogProbGivenPrevious(realPos, i, previous);
                }

                return scores;
            }

            public double ScoreOf(int[] sequence)
            {
                throw new NotSupportedException();
            }
        }

        public override List<IN> Classify(List<IN> document)
        {
            if (flags.doGibbs)
            {
                try
                {
                    return ClassifyGibbs(document);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(@"Error running testGibbs inference!");
                    //e.PrintStackTrace();
                    return null;
                }
            }
            else if (flags.crfType.EqualsIgnoreCase(@"maxent"))
            {
                return ClassifyMaxEnt(document);
            }
            else
            {
                throw new Exception(@"Unsupported inference type: " + flags.crfType);
            }
        }

        private List<IN> Classify(List<IN> document, Tuple<int[][][], int[], double[][][]> documentDataAndLabels)
        {
            if (flags.doGibbs)
            {
                try
                {
                    return ClassifyGibbs(document, documentDataAndLabels);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(@"Error running testGibbs inference!");
                    //e.PrintStackTrace();
                    return null;
                }
            }
            else if (flags.crfType.EqualsIgnoreCase(@"maxent"))
            {
                return ClassifyMaxEnt(document, documentDataAndLabels);
            }
            else
            {
                throw new Exception(@"Unsupported inference type: " + flags.crfType);
            }
        }

        virtual void ClassifyAndWriteAnswers(ICollection<List<IN>> documents, IList<Tuple<int[][][], int[], double[][][]>> documentDataAndLabels, TextWriter printWriter, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            Timing timer = new Timing();
            Counter<String> entityTP = new ClassicCounter<String>();
            Counter<String> entityFP = new ClassicCounter<String>();
            Counter<String> entityFN = new ClassicCounter<String>();
            bool resultsCounted = true;
            int numWords = 0;
            int numDocs = 0;
            foreach (List<IN> doc in documents)
            {
                Classify(doc, documentDataAndLabels.Get(numDocs));
                numWords += doc.Size();
                WriteAnswers(doc, printWriter, readerAndWriter);
                resultsCounted = resultsCounted && CountResults(doc, entityTP, entityFP, entityFN);
                numDocs++;
            }

            long millis = timer.Stop();
            double wordspersec = numWords / (((double)millis) / 1000);
            NumberFormatInfo nf = new NumberFormatInfo { NumberDecimalDigits = 2 };
            if (!flags.suppressTestDebug)
                Console.Error.WriteLine(StringUtils.GetShortClassName(this) + @" tagged " + numWords + @" words in " + numDocs + @" documents at " + nf.Format(wordspersec) + @" words per second.");
            if (resultsCounted && !flags.suppressTestDebug)
            {
                PrintResults(entityTP, entityFP, entityFN);
            }
        }

        public override ISequenceModel GetSequenceModel(IList<IN> doc)
        {
            Tuple<int[][][], int[], double[][][]> p = DocumentToDataAndLabels(doc);
            return GetSequenceModel(p);
        }

        private ISequenceModel GetSequenceModel(Tuple<int[][][], int[], double[][][]> documentDataAndLabels)
        {
            return new TestSequenceModel(GetCliqueTree(documentDataAndLabels));
        }

        private ICliquePotentialFunction GetCliquePotentialFunction()
        {
            if (cliquePotentialFunction == null)
            {
                if (flags.nonLinearCRF)
                {
                    if (flags.secondOrderNonLinear)
                        cliquePotentialFunction = new NonLinearSecondOrderCliquePotentialFunction(inputLayerWeights4Edge, outputLayerWeights4Edge, inputLayerWeights, outputLayerWeights, flags);
                    else
                        cliquePotentialFunction = new NonLinearCliquePotentialFunction(linearWeights, inputLayerWeights, outputLayerWeights, flags);
                }
                else
                {
                    cliquePotentialFunction = new LinearCliquePotentialFunction(weights);
                }
            }

            return cliquePotentialFunction;
        }

        public virtual void UpdateWeights(double[] x)
        {
            cliquePotentialFunction = cliquePotentialFunctionHelper.GetCliquePotentialFunction(x);
        }

        public virtual List<IN> ClassifyMaxEnt(List<IN> document)
        {
            if (document.IsEmpty())
            {
                return document;
            }

            ISequenceModel model = GetSequenceModel(document);
            return ClassifyMaxEnt(document, model);
        }

        private List<IN> ClassifyMaxEnt(IList<IN> document, Tuple<int[][][], int[], double[][][]> documentDataAndLabels)
        {
            if (document.IsEmpty())
            {
                return document;
            }

            ISequenceModel model = GetSequenceModel(documentDataAndLabels);
            return ClassifyMaxEnt(document, model);
        }

        private List<IN> ClassifyMaxEnt(IList<IN> document, ISequenceModel model)
        {
            if (document.IsEmpty())
            {
                return document;
            }

            if (flags.inferenceType == null)
            {
                flags.inferenceType = @"Viterbi";
            }

            BestSequenceFinder tagInference;
            if (flags.inferenceType.EqualsIgnoreCase(@"Viterbi"))
            {
                tagInference = new ExactBestSequenceFinder();
            }
            else if (flags.inferenceType.EqualsIgnoreCase(@"Beam"))
            {
                tagInference = new BeamBestSequenceFinder(flags.beamSize);
            }
            else
            {
                throw new Exception(@"Unknown inference type: " + flags.inferenceType + @". Your options are Viterbi|Beam.");
            }

            int[] bestSequence = tagInference.BestSequence(model);
            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            for (int j = 0, docSize = document.Size(); j < docSize; j++)
            {
                IN wi = document.Get(j);
                string guess = classIndex.Get(bestSequence[j + windowSize - 1]);
                wi.Set(typeof(CoreAnnotations.AnswerAnnotation), guess);
            }

            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            return document;
        }

        public virtual List<IN> ClassifyGibbs(List<IN> document)
        {
            Tuple<int[][][], int[], double[][][]> p = DocumentToDataAndLabels(document);
            return ClassifyGibbs(document, p);
        }

        public virtual List<IN> ClassifyGibbs(List<IN> document, Tuple<int[][][], int[], double[][][]> documentDataAndLabels)
        {
            List<IN> newDocument = document;
            if (flags.useReverse)
            {
                Collections.Reverse(document);
                newDocument = new List<IN>(document);
                Collections.Reverse(document);
            }

            CRFCliqueTree<string> cliqueTree = GetCliqueTree(documentDataAndLabels);
            ISequenceModel model = cliqueTree;
            ISequenceListener listener = cliqueTree;
            ISequenceModel priorModel = null;
            SequenceListener priorListener = null;
            if (flags.useNERPrior)
            {
                EntityCachingAbstractSequencePrior<IN> prior = new EmpiricalNERPrior<IN>(flags.backgroundSymbol, classIndex, newDocument);
                priorModel = prior;
                priorListener = prior;
            }
            else if (flags.useNERPriorBIO)
            {
                EntityCachingAbstractSequencePriorBIO<IN> prior = new EmpiricalNERPriorBIO<IN>(flags.backgroundSymbol, classIndex, tagIndex, newDocument, entityMatrices, flags);
                priorModel = prior;
                priorListener = prior;
            }
            else if (flags.useAcqPrior)
            {
                EntityCachingAbstractSequencePrior<IN> prior = new AcquisitionsPrior<IN>(flags.backgroundSymbol, classIndex, newDocument);
                priorModel = prior;
                priorListener = prior;
            }
            else if (flags.useSemPrior)
            {
                EntityCachingAbstractSequencePrior<IN> prior = new SeminarsPrior<IN>(flags.backgroundSymbol, classIndex, newDocument);
                priorModel = prior;
                priorListener = prior;
            }
            else if (flags.useUniformPrior)
            {
                UniformPrior<IN> uniPrior = new UniformPrior<IN>(flags.backgroundSymbol, classIndex, newDocument);
                priorModel = uniPrior;
                priorListener = uniPrior;
            }
            else
            {
                throw new Exception(@"no prior specified");
            }

            model = new FactoredSequenceModel(model, priorModel);
            listener = new FactoredSequenceListener(listener, priorListener);
            SequenceGibbsSampler sampler = new SequenceGibbsSampler(0, 0, listener);
            int[] sequence = new int[cliqueTree.Length()];
            if (flags.initViterbi)
            {
                TestSequenceModel testSequenceModel = new TestSequenceModel(cliqueTree);
                ExactBestSequenceFinder tagInference = new ExactBestSequenceFinder();
                int[] bestSequence = tagInference.BestSequence(testSequenceModel);
                Array.Copy(bestSequence, windowSize - 1, sequence, 0, sequence.Length);
            }
            else
            {
                int[] initialSequence = SequenceGibbsSampler.GetRandomSequence(model);
                Array.Copy(initialSequence, 0, sequence, 0, sequence.Length);
            }

            sampler.verbose = 0;
            if (flags.annealingType.EqualsIgnoreCase(@"linear"))
            {
                sequence = sampler.FindBestUsingAnnealing(model, CoolingSchedule.GetLinearSchedule(1.0, flags.numSamples), sequence);
            }
            else if (flags.annealingType.EqualsIgnoreCase(@"exp") || flags.annealingType.EqualsIgnoreCase(@"exponential"))
            {
                sequence = sampler.FindBestUsingAnnealing(model, CoolingSchedule.GetExponentialSchedule(1.0, flags.annealingRate, flags.numSamples), sequence);
            }
            else
            {
                throw new Exception(@"No annealing type specified");
            }

            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            for (int j = 0, dsize = newDocument.Size(); j < dsize; j++)
            {
                IN wi = document.Get(j);
                if (wi == null)
                    throw new Exception(@"");
                if (classIndex == null)
                    throw new Exception(@"");
                wi.Set(typeof(CoreAnnotations.AnswerAnnotation), classIndex.Get(sequence[j]));
            }

            if (flags.useReverse)
            {
                Collections.Reverse(document);
            }

            return document;
        }

        public virtual IList<IN> ClassifyGibbsUsingPrior(IList<IN> sentence, ISequenceModel[] priorModels, ISequenceListener[] priorListeners, double[] modelWts)
        {
            if ((priorModels.Length + 1) != modelWts.Length)
                throw new Exception(@"modelWts array should be longer than the priorModels array by 1 unit since it also includes the weight of the CRF model at position 0.");
            Tuple<int[][][], int[], double[][][]> p = DocumentToDataAndLabels(sentence);
            List<IN> newDocument = sentence;
            if (flags.useReverse)
            {
                Collections.Reverse(sentence);
                newDocument = new List<IN>(sentence);
                Collections.Reverse(sentence);
            }

            CRFCliqueTree<String> cliqueTree = GetCliqueTree(p);
            ISequenceModel model = cliqueTree;
            ISequenceListener listener = cliqueTree;
            ISequenceModel[] models = new ISequenceModel[priorModels.Length + 1];
            models[0] = model;
            for (int i = 1; i < models.Length; i++)
                models[i] = priorModels[i - 1];
            model = new FactoredSequenceModel(models, modelWts);
            ISequenceListener[] listeners = new ISequenceListener[priorListeners.Length + 1];
            listeners[0] = listener;
            for (int i = 1; i < listeners.Length; i++)
                listeners[i] = priorListeners[i - 1];
            listener = new FactoredSequenceListener(listeners);
            SequenceGibbsSampler sampler = new SequenceGibbsSampler(0, 0, listener);
            int[] sequence = new int[cliqueTree.Length()];
            if (flags.initViterbi)
            {
                TestSequenceModel testSequenceModel = new TestSequenceModel(cliqueTree);
                ExactBestSequenceFinder tagInference = new ExactBestSequenceFinder();
                int[] bestSequence = tagInference.BestSequence(testSequenceModel);
                Array.Copy(bestSequence, windowSize - 1, sequence, 0, sequence.Length);
            }
            else
            {
                int[] initialSequence = SequenceGibbsSampler.GetRandomSequence(model);
                Array.Copy(initialSequence, 0, sequence, 0, sequence.Length);
            }

            SequenceGibbsSampler.verbose = 0;
            if (flags.annealingType.EqualsIgnoreCase(@"linear"))
            {
                sequence = sampler.FindBestUsingAnnealing(model, CoolingSchedule.GetLinearSchedule(1.0, flags.numSamples), sequence);
            }
            else if (flags.annealingType.EqualsIgnoreCase(@"exp") || flags.annealingType.EqualsIgnoreCase(@"exponential"))
            {
                sequence = sampler.FindBestUsingAnnealing(model, CoolingSchedule.GetExponentialSchedule(1.0, flags.annealingRate, flags.numSamples), sequence);
            }
            else
            {
                throw new Exception(@"No annealing type specified");
            }

            if (flags.useReverse)
            {
                Collections.Reverse(sentence);
            }

            for (int j = 0, dsize = newDocument.size(); j < dsize; j++)
            {
                IN wi = sentence.Get(j);
                if (wi == null)
                    throw new Exception(@"");
                if (classIndex == null)
                    throw new Exception(@"");
                wi.Set(typeof(CoreAnnotations.AnswerAnnotation), classIndex.Get(sequence[j]));
            }

            if (flags.useReverse)
            {
                Collections.Reverse(sentence);
            }

            return sentence;
        }

        public virtual IList<IN> ClassifyGibbsUsingPrior(IList<IN> sentence, ISequenceModel priorModel, ISequenceListener priorListener, double model1Wt, double model2Wt)
        {
            Tuple<int[][][], int[], double[][][]> p = DocumentToDataAndLabels(sentence);
            IList<IN> newDocument = sentence;
            if (flags.useReverse)
            {
                newDocument = new List<IN>(sentence);
                Collections.Reverse(newDocument);
            }

            CRFCliqueTree<String> cliqueTree = GetCliqueTree(p);
            SequenceModel model = cliqueTree;
            SequenceListener listener = cliqueTree;
            model = new FactoredSequenceModel(model, priorModel, model1Wt, model2Wt);
            listener = new FactoredSequenceListener(listener, priorListener);
            SequenceGibbsSampler sampler = new SequenceGibbsSampler(0, 0, listener);
            int[] sequence = new int[cliqueTree.Length()];
            if (flags.initViterbi)
            {
                TestSequenceModel testSequenceModel = new TestSequenceModel(cliqueTree);
                ExactBestSequenceFinder tagInference = new ExactBestSequenceFinder();
                int[] bestSequence = tagInference.BestSequence(testSequenceModel);
                Array.Copy(bestSequence, windowSize - 1, sequence, 0, sequence.length);
            }
            else
            {
                int[] initialSequence = SequenceGibbsSampler.GetRandomSequence(model);
                Array.Copy(initialSequence, 0, sequence, 0, sequence.length);
            }

            SequenceGibbsSampler.verbose = 0;
            if (flags.annealingType.EqualsIgnoreCase(@"linear"))
            {
                sequence = sampler.FindBestUsingAnnealing(model, CoolingSchedule.GetLinearSchedule(1.0, flags.numSamples), sequence);
            }
            else if (flags.annealingType.EqualsIgnoreCase(@"exp") || flags.annealingType.EqualsIgnoreCase(@"exponential"))
            {
                sequence = sampler.FindBestUsingAnnealing(model, CoolingSchedule.GetExponentialSchedule(1.0, flags.annealingRate, flags.numSamples), sequence);
            }
            else
            {
                throw new Exception(@"No annealing type specified");
            }

            if (flags.useReverse)
            {
                Collections.Reverse(sentence);
            }

            for (int j = 0, dsize = newDocument.size(); j < dsize; j++)
            {
                IN wi = sentence.Get(j);
                if (wi == null)
                    throw new Exception(@"");
                if (classIndex == null)
                    throw new Exception(@"");
                wi.Set(typeof(CoreAnnotations.AnswerAnnotation), classIndex.Get(sequence[j]));
            }

            if (flags.useReverse)
            {
                Collections.Reverse(sentence);
            }

            return sentence;
        }

        public override void PrintProbsDocument(List<IN> document)
        {
            Triple<int[][][], int[], double[][][]> p = DocumentToDataAndLabels(document);
            CRFCliqueTree<String> cliqueTree = GetCliqueTree(p);
            for (int i = 0; i < cliqueTree.Length(); i++)
            {
                IN wi = document.Get(i);
                Console.Out.Write(wi.Get(typeof(CoreAnnotations.TextAnnotation)) + '\n');
                for (Iterator<String> iter = classIndex.iterator(); iter.HasNext(); )
                {
                    string label = iter.Next();
                    int index = classIndex.IndexOf(label);
                    double prob = cliqueTree.Prob(i, index);
                    Console.Out.Write(label + '=' + prob);
                    if (iter.HasNext())
                    {
                        Console.Out.Write(@"\t");
                    }
                    else
                    {
                        Console.Out.Write(@"\n");
                    }
                }
            }
        }

        public virtual void PrintFirstOrderProbs(string filename, DocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = false;
            ObjectBank<List<IN>> docs = MakeObjectBankFromFile(filename, readerAndWriter);
            PrintFirstOrderProbsDocuments(docs);
        }

        public virtual void PrintFactorTable(string filename, DocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = false;
            ObjectBank<List<IN>> docs = MakeObjectBankFromFile(filename, readerAndWriter);
            PrintFactorTableDocuments(docs);
        }

        public virtual void PrintFirstOrderProbsDocuments(ObjectBank<List<IN>> documents)
        {
            foreach (List<IN> doc in documents)
            {
                PrintFirstOrderProbsDocument(doc);
                Console.Out.WriteLine();
            }
        }

        public virtual void PrintFactorTableDocuments(ObjectBank<List<IN>> documents)
        {
            foreach (List<IN> doc in documents)
            {
                PrintFactorTableDocument(doc);
                Console.Out.WriteLine();
            }
        }

        public virtual List<CRFCliqueTree<String>> GetCliqueTrees(string filename, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = false;
            List<CRFCliqueTree<String>> cts = new List<CRFCliqueTree<String>>();
            ObjectBank<List<IN>> docs = MakeObjectBankFromFile(filename, readerAndWriter);
            foreach (List<IN> doc in docs)
            {
                cts.Add(GetCliqueTree(doc));
            }

            return cts;
        }

        public virtual CRFCliqueTree<String> GetCliqueTree(Tuple<int[][][], int[], double[][][]> p)
        {
            int[][][] data = p.First();
            double[][][] featureVal = p.Third();
            return CRFCliqueTree.GetCalibratedCliqueTree(data, labelIndices, classIndex.Size(), classIndex, flags.backgroundSymbol, GetCliquePotentialFunction(), featureVal);
        }

        public virtual CRFCliqueTree<String> GetCliqueTree(List<IN> document)
        {
            Triple<int[][][], int[], double[][][]> p = DocumentToDataAndLabels(document);
            return GetCliqueTree(p);
        }

        public virtual void PrintFactorTableDocument(List<IN> document)
        {
            CRFCliqueTree<String> cliqueTree = GetCliqueTree(document);
            FactorTable[] factorTables = cliqueTree.GetFactorTables();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < factorTables.length; i++)
            {
                IN wi = document.Get(i);
                sb.Append(wi.Get(typeof(CoreAnnotations.TextAnnotation)));
                sb.Append(@"\t");
                FactorTable table = factorTables[i];
                double totalMass = table.TotalMass();
                for (int j = 0; j < table.Size(); j++)
                {
                    int[] arr = table.ToArray(j);
                    sb.Append(classIndex.Get(arr[0]));
                    sb.Append(@":");
                    sb.Append(classIndex.Get(arr[1]));
                    sb.Append(@":");
                    sb.Append(table.GetValue(j) - totalMass);
                    sb.Append(@" ");
                }

                sb.Append(@"\n");
            }

            Console.Out.Write(sb.ToString());
        }

        public virtual void PrintFirstOrderProbsDocument(List<IN> document)
        {
            CRFCliqueTree<String> cliqueTree = GetCliqueTree(document);
            for (int i = 0; i < cliqueTree.Length(); i++)
            {
                IN wi = document.Get(i);
                Console.Out.Write(wi.Get(typeof(CoreAnnotations.TextAnnotation)) + '\n');
                for (Iterator<String> iter = classIndex.iterator(); iter.HasNext(); )
                {
                    string label = iter.Next();
                    int index = classIndex.IndexOf(label);
                    if (i == 0)
                    {
                        double prob = cliqueTree.Prob(i, index);
                        Console.Out.Write(label + '=' + prob);
                        if (iter.HasNext())
                        {
                            Console.Out.Write(@"\t");
                        }
                        else
                        {
                            Console.Out.Write(@"\n");
                        }
                    }
                    else
                    {
                        for (Iterator<String> iter1 = classIndex.iterator(); iter1.HasNext(); )
                        {
                            string label1 = iter1.Next();
                            int index1 = classIndex.IndexOf(label1);
                            double prob = cliqueTree.Prob(i, new int
                        {
                        index1, index
                        }

                            );
                            Console.Out.Write(label1 + '_' + label + '=' + prob);
                            if (iter.HasNext() || iter1.HasNext())
                            {
                                Console.Out.Write(@"\t");
                            }
                            else
                            {
                                Console.Out.Write(@"\n");
                            }
                        }
                    }
                }
            }
        }

        public override void Train(Collection<List<IN>> docs, DocumentReaderAndWriter<IN> readerAndWriter)
        {
            Timing timer = new Timing();
            timer.Start();
            if (flags.numOfSlices > 0)
            {
                Console.Error.WriteLine(@"Taking " + flags.numOfSlices + @" out of " + flags.totalDataSlice + @" slices of data for training");
                List<List<IN>> docsToShuffle = new List<List<IN>>();
                foreach (List<IN> doc in docs)
                {
                    docsToShuffle.Add(doc);
                }

                Collections.Shuffle(docsToShuffle, random);
                int cutOff = (int)(docsToShuffle.Size() / (flags.totalDataSlice + 0.0) * flags.numOfSlices);
                docs = docsToShuffle.SubList(0, cutOff);
            }

            MakeAnswerArraysAndTagIndex(docs);
            long elapsedMs = timer.Stop();
            Console.Error.WriteLine(@"Time to convert docs to feature indices: " + Timing.ToSecondsString(elapsedMs) + @" seconds");
            if (flags.exportFeatures != null)
            {
                timer.Start();
                CRFFeatureExporter<IN> featureExporter = new CRFFeatureExporter<IN>(this);
                featureExporter.PrintFeatures(flags.exportFeatures, docs);
                elapsedMs = timer.Stop();
                Console.Error.WriteLine(@"Time to export features: " + Timing.ToSecondsString(elapsedMs) + @" seconds");
            }

            for (int i = 0; i <= flags.numTimesPruneFeatures; i++)
            {
                timer.Start();
                Triple<int[][][][], int[][], double[][][][]> dataAndLabelsAndFeatureVals = DocumentsToDataAndLabels(docs);
                elapsedMs = timer.Stop();
                Console.Error.WriteLine(@"Time to convert docs to data/labels: " + Timing.ToSecondsString(elapsedMs) + @" seconds");
                Evaluator[] evaluators = null;
                if (flags.evaluateIters > 0 || flags.terminateOnEvalImprovement)
                {
                    List<Evaluator> evaluatorList = new List<Evaluator>();
                    if (flags.useMemoryEvaluator)
                        evaluatorList.Add(new MemoryEvaluator());
                    if (flags.evaluateTrain)
                    {
                        CRFClassifierEvaluator<IN> crfEvaluator = new CRFClassifierEvaluator<IN>(@"Train set", this);
                        List<Triple<int[][][], int[], double[][][]>> trainDataAndLabels = new List<Triple<int[][][], int[], double[][][]>>();
                        int[][][][] data = dataAndLabelsAndFeatureVals.First();
                        int[][] labels = dataAndLabelsAndFeatureVals.Second();
                        double[][][][] featureVal = dataAndLabelsAndFeatureVals.Third();
                        for (int j = 0; j < data.length; j++)
                        {
                            Triple<int[][][], int[], double[][][]> p = new Triple<int[][][], int[], double[][][]>(data[j], labels[j], featureVal[j]);
                            trainDataAndLabels.Add(p);
                        }

                        crfEvaluator.SetTestData(docs, trainDataAndLabels);
                        if (flags.evalCmd.Length() > 0)
                            crfEvaluator.SetEvalCmd(flags.evalCmd);
                        evaluatorList.Add(crfEvaluator);
                    }

                    if (flags.testFile != null)
                    {
                        CRFClassifierEvaluator<IN> crfEvaluator = new CRFClassifierEvaluator<IN>(@"Test set (" + flags.testFile + @")", this);
                        ObjectBank<List<IN>> testObjBank = MakeObjectBankFromFile(flags.testFile, readerAndWriter);
                        List<List<IN>> testDocs = new List<List<IN>>();
                        foreach (List<IN> doc in testObjBank)
                        {
                            testDocs.Add(doc);
                        }

                        List<Triple<int[][][], int[], double[][][]>> testDataAndLabels = DocumentsToDataAndLabelsList(testDocs);
                        crfEvaluator.SetTestData(testDocs, testDataAndLabels);
                        if (flags.evalCmd.Length() > 0)
                            crfEvaluator.SetEvalCmd(flags.evalCmd);
                        evaluatorList.Add(crfEvaluator);
                    }

                    if (flags.testFiles != null)
                    {
                        String[] testFiles = flags.testFiles.Split(@",");
                        foreach (string testFile in testFiles)
                        {
                            CRFClassifierEvaluator<IN> crfEvaluator = new CRFClassifierEvaluator<IN>(@"Test set (" + testFile + @")", this);
                            ObjectBank<List<IN>> testObjBank = MakeObjectBankFromFile(testFile, readerAndWriter);
                            List<Triple<int[][][], int[], double[][][]>> testDataAndLabels = DocumentsToDataAndLabelsList(testObjBank);
                            crfEvaluator.SetTestData(testObjBank, testDataAndLabels);
                            if (flags.evalCmd.Length() > 0)
                                crfEvaluator.SetEvalCmd(flags.evalCmd);
                            evaluatorList.Add(crfEvaluator);
                        }
                    }

                    evaluators = new Evaluator[evaluatorList.Size()];
                    evaluatorList.ToArray(evaluators);
                }

                if (flags.numTimesPruneFeatures == i)
                {
                    docs = null;
                }

                File featIndexFile = null;
                int numFeatures = featureIndex.Size();
                if (flags.saveFeatureIndexToDisk)
                {
                    try
                    {
                        Console.Error.WriteLine(@"Writing feature index to temporary file.");
                        featIndexFile = IOUtils.WriteObjectToTempFile(featureIndex, @"featIndex" + i + @".tmp");
                        featureIndex = null;
                    }
                    catch (IOException e)
                    {
                        throw new Exception(@"Could not open temporary feature index file for writing.");
                    }
                }

                int[][][][] data = dataAndLabelsAndFeatureVals.First();
                int[][] labels = dataAndLabelsAndFeatureVals.Second();
                double[][][][] featureVals = dataAndLabelsAndFeatureVals.Third();
                if (flags.loadProcessedData != null)
                {
                    List<List<CRFDatum<Collection<String>, String>>> processedData = LoadProcessedData(flags.loadProcessedData);
                    if (processedData != null)
                    {
                        int[][][][] allData = new int[data.length + processedData.Size()];
                        double[][][][] allFeatureVals = new double[featureVals.length + processedData.Size()];
                        int[][] allLabels = new int[labels.length + processedData.Size()];
                        Array.Copy(data, 0, allData, 0, data.length);
                        Array.Copy(labels, 0, allLabels, 0, labels.length);
                        Array.Copy(featureVals, 0, allFeatureVals, 0, featureVals.length);
                        AddProcessedData(processedData, allData, allLabels, allFeatureVals, data.length);
                        data = allData;
                        labels = allLabels;
                        featureVals = allFeatureVals;
                    }
                }

                if (flags.nonLinearCRF)
                {
                    if (flags.secondOrderNonLinear)
                    {
                        CRFNonLinearSecondOrderLogConditionalObjectiveFunction func = new CRFNonLinearSecondOrderLogConditionalObjectiveFunction(data, labels, windowSize, classIndex, labelIndices, map, flags, nodeFeatureIndicesMap.Size(), edgeFeatureIndicesMap.Size());
                        cliquePotentialFunctionHelper = func;
                        double[] allWeights = TrainWeightsUsingNonLinearCRF(func, evaluators);
                        Tuple<double[][], double[][], double[][], double[][]> params_renamed = func.SeparateWeights(allWeights);
                        this.inputLayerWeights4Edge = params_renamed.First();
                        this.outputLayerWeights4Edge = params_renamed.Second();
                        this.inputLayerWeights = params_renamed.Third();
                        this.outputLayerWeights = params_renamed.Fourth();
                        Console.Error.WriteLine(@"Edge Output Layer Weights:");
                        for (int ii = 0; ii < outputLayerWeights4Edge.length; ii++)
                        {
                            System.err.Print(@"[ ");
                            for (int jj = 0; jj < outputLayerWeights4Edge[ii].length; jj++)
                            {
                                System.err.Print(outputLayerWeights4Edge[ii][jj] + @" ");
                            }

                            Console.Error.WriteLine(@"]");
                        }

                        Console.Error.WriteLine(@"Node Output Layer Weights:");
                        for (int ii = 0; ii < outputLayerWeights.length; ii++)
                        {
                            System.err.Print(@"[ ");
                            for (int jj = 0; jj < outputLayerWeights[ii].length; jj++)
                            {
                                System.err.Print(outputLayerWeights[ii][jj] + @" ");
                            }

                            Console.Error.WriteLine(@"]");
                        }
                    }
                    else
                    {
                        CRFNonLinearLogConditionalObjectiveFunction func = new CRFNonLinearLogConditionalObjectiveFunction(data, labels, windowSize, classIndex, labelIndices, map, flags, nodeFeatureIndicesMap.Size(), edgeFeatureIndicesMap.Size(), featureVals);
                        if (flags.useAdaGradFOBOS)
                        {
                            func.gradientsOnly = true;
                        }

                        cliquePotentialFunctionHelper = func;
                        double[] allWeights = TrainWeightsUsingNonLinearCRF(func, evaluators);
                        Tuple<double[][], double[][], double[][]> params_renamed = func.SeparateWeights(allWeights);
                        this.linearWeights = params_renamed.First();
                        this.inputLayerWeights = params_renamed.Second();
                        this.outputLayerWeights = params_renamed.Third();
                        if (flags.printWeights)
                        {
                            Console.Error.WriteLine(@"Linear Layer Weights:");
                            for (int ii = 0; ii < linearWeights.length; ii++)
                            {
                                System.err.Print(@"[ ");
                                for (int jj = 0; jj < linearWeights[ii].length; jj++)
                                {
                                    System.err.Print(linearWeights[ii][jj] + @" ");
                                }

                                Console.Error.WriteLine(@"]");
                            }

                            Console.Error.WriteLine(@"Input Layer Weights:");
                            for (int ii = 0; ii < inputLayerWeights.length; ii++)
                            {
                                System.err.Print(@"[ ");
                                for (int jj = 0; jj < inputLayerWeights[ii].length; jj++)
                                {
                                    System.err.Print(inputLayerWeights[ii][jj] + @" ");
                                }

                                Console.Error.WriteLine(@"]");
                            }

                            Console.Error.WriteLine(@"Output Layer Weights:");
                            for (int ii = 0; ii < outputLayerWeights.length; ii++)
                            {
                                System.err.Print(@"[ ");
                                for (int jj = 0; jj < outputLayerWeights[ii].length; jj++)
                                {
                                    System.err.Print(outputLayerWeights[ii][jj] + @" ");
                                }

                                Console.Error.WriteLine(@"]");
                            }
                        }
                    }
                }
                else
                {
                    double[] oneDimWeights = null;
                    if (flags.useFloat)
                    {
                        oneDimWeights = TrainWeightsUsingFloatCRF(data, labels, i);
                    }
                    else if (flags.numLopExpert > 1)
                    {
                        oneDimWeights = TrainWeightsUsingLopCRF(numFeatures, data, labels, evaluators, i);
                    }
                    else
                    {
                        oneDimWeights = TrainWeightsUsingDoubleCRF(data, labels, evaluators, i, featureVals);
                    }

                    this.weights = CRFLogConditionalObjectiveFunction.To2D(oneDimWeights, labelIndices, map);
                }

                if (flags.saveFeatureIndexToDisk)
                {
                    try
                    {
                        Console.Error.WriteLine(@"Reading temporary feature index file.");
                        featureIndex = (IIndex<String>)IOUtils.ReadObjectFromFile(featIndexFile);
                    }
                    catch (Exception e)
                    {
                        throw new Exception(@"Could not open temporary feature index file for reading.");
                    }
                }

                if (i != flags.numTimesPruneFeatures)
                {
                    DropFeaturesBelowThreshold(flags.featureDiffThresh);
                    Console.Error.WriteLine(@"Removing features with weight below " + flags.featureDiffThresh + @" and retraining...");
                }
            }
        }

        protected virtual double[] TrainWeightsUsingFloatCRF(int[][][][] data, int[][] labels, int pruneFeatureItr)
        {
            CRFLogConditionalObjectiveFloatFunction func = new CRFLogConditionalObjectiveFloatFunction(data, labels, featureIndex, windowSize, classIndex, labelIndices, map, flags.backgroundSymbol, flags.sigma);
            cliquePotentialFunctionHelper = func;
            QNMinimizer minimizer;
            if (flags.interimOutputFreq != 0)
            {
                FloatFunction monitor = new ResultStoringFloatMonitor(flags.interimOutputFreq, flags.serializeTo);
                minimizer = new QNMinimizer(monitor);
            }
            else
            {
                minimizer = new QNMinimizer();
            }

            if (pruneFeatureItr == 0)
            {
                minimizer.SetM(flags.QNsize);
            }
            else
            {
                minimizer.SetM(flags.QNsize2);
            }

            float[] initialWeights;
            if (flags.initialWeights == null)
            {
                initialWeights = func.Initial();
            }
            else
            {
                try
                {
                    Console.Error.WriteLine(@"Reading initial weights from file " + flags.initialWeights);
                    DataInputStream dis = new DataInputStream(new BufferedInputStream(new GZIPInputStream(new FileInputStream(flags.initialWeights))));
                    initialWeights = ConvertByteArray.ReadFloatArr(dis);
                }
                catch (IOException e)
                {
                    throw new Exception(@"Could not read from float initial weight file " + flags.initialWeights);
                }
            }

            Console.Error.WriteLine(@"numWeights: " + initialWeights.length);
            float[] weights = minimizer.Minimize(func, (float)flags.tolerance, initialWeights);
            return ArrayMath.FloatArrayToDoubleArray(weights);
        }

        protected virtual void PruneNodeFeatureIndices(int totalNumOfFeatureSlices, int numOfFeatureSlices)
        {
            int numOfNodeFeatures = nodeFeatureIndicesMap.Size();
            int beginIndex = 0;
            int endIndex = Math.Min((int)(numOfNodeFeatures / (totalNumOfFeatureSlices + 0.0) * numOfFeatureSlices), numOfNodeFeatures);
            List<int> nodeFeatureOriginalIndices = nodeFeatureIndicesMap.ObjectsList();
            List<int> edgeFeatureOriginalIndices = edgeFeatureIndicesMap.ObjectsList();
            IIndex<int> newNodeFeatureIndex = new HashIndex<int>();
            IIndex<int> newEdgeFeatureIndex = new HashIndex<int>();
            IIndex<String> newFeatureIndex = new HashIndex<String>();
            for (int i = beginIndex; i < endIndex; i++)
            {
                int oldIndex = nodeFeatureOriginalIndices.Get(i);
                string f = featureIndex.Get(oldIndex);
                int index = newFeatureIndex.IndexOf(f, true);
                newNodeFeatureIndex.Add(index);
            }

            foreach (int edgeFIndex in edgeFeatureOriginalIndices)
            {
                string f = featureIndex.Get(edgeFIndex);
                int index = newFeatureIndex.IndexOf(f, true);
                newEdgeFeatureIndex.Add(index);
            }

            nodeFeatureIndicesMap = newNodeFeatureIndex;
            edgeFeatureIndicesMap = newEdgeFeatureIndex;
            int[] newMap = new int[newFeatureIndex.Size()];
            for (int i = 0; i < newMap.length; i++)
            {
                int index = featureIndex.IndexOf(newFeatureIndex.Get(i));
                newMap[i] = map[index];
            }

            map = newMap;
            featureIndex = newFeatureIndex;
        }

        protected virtual int[][][][] CreatePartialDataForLOP(int lopIter, int[][][][] data)
        {
            int[] oldFeatures = null;
            int oldFeatureIndex = -1;
            List<int> newFeatureList = new List<int>(1000);
            Set<int> featureIndicesSet = featureIndicesSetArray.Get(lopIter);
            int[][][][] newData = new int[data.length];
            for (int i = 0; i < data.length; i++)
            {
                newData[i] = new int[data[i].length];
                for (int j = 0; j < data[i].length; j++)
                {
                    newData[i][j] = new int[data[i][j].length];
                    for (int k = 0; k < data[i][j].length; k++)
                    {
                        oldFeatures = data[i][j][k];
                        newFeatureList.Clear();
                        for (int l = 0; l < oldFeatures.length; l++)
                        {
                            oldFeatureIndex = oldFeatures[l];
                            if (featureIndicesSet.Contains(oldFeatureIndex))
                            {
                                newFeatureList.Add(oldFeatureIndex);
                            }
                        }

                        newData[i][j][k] = new int[newFeatureList.Size()];
                        for (int l = 0; l < newFeatureList.Size(); ++l)
                        {
                            newData[i][j][k][l] = newFeatureList.Get(l);
                        }
                    }
                }
            }

            return newData;
        }

        protected virtual void GetFeatureBoundaryIndices(int numFeatures, int numLopExpert)
        {
            int interval = numFeatures / numLopExpert;
            featureIndicesSetArray = new List<Set<int>>(numLopExpert);
            featureIndicesListArray = new List<List<int>>(numLopExpert);
            for (int i = 0; i < numLopExpert; i++)
            {
                featureIndicesSetArray.Add(Generics.NewHashSet(interval));
                featureIndicesListArray.Add(Generics.NewArrayList(interval));
            }

            if (flags.randomLopFeatureSplit)
            {
                for (int fIndex = 0; fIndex < numFeatures; fIndex++)
                {
                    int lopIter = random.NextInt(numLopExpert);
                    featureIndicesSetArray.Get(lopIter).Add(fIndex);
                    featureIndicesListArray.Get(lopIter).Add(fIndex);
                }
            }
            else
            {
                for (int lopIter = 0; lopIter < numLopExpert; lopIter++)
                {
                    int beginIndex = lopIter * interval;
                    int endIndex = (lopIter + 1) * interval;
                    if (lopIter == numLopExpert - 1)
                    {
                        endIndex = numFeatures;
                    }

                    for (int fIndex = beginIndex; fIndex < endIndex; fIndex++)
                    {
                        featureIndicesSetArray.Get(lopIter).Add(fIndex);
                        featureIndicesListArray.Get(lopIter).Add(fIndex);
                    }
                }
            }

            for (int lopIter = 0; lopIter < numLopExpert; lopIter++)
            {
                Collections.Sort(featureIndicesListArray.Get(lopIter));
            }
        }

        protected virtual double[] TrainWeightsUsingLopCRF(int numFeatures, int[][][][] data, int[][] labels, Evaluator[] evaluators, int pruneFeatureItr)
        {
            int numLopExpert = flags.numLopExpert;
            double[][] lopExpertWeights = new double[numLopExpert];
            GetFeatureBoundaryIndices(numFeatures, numLopExpert);
            if (flags.initialLopWeights != null)
            {
                try
                {
                    Console.Error.WriteLine(@"Reading initial LOP weights from file " + flags.initialLopWeights + @" ...");
                    BufferedReader br = IOUtils.ReaderFromString(flags.initialLopWeights);
                    List<double[]> listOfWeights = new List<double[]>(numLopExpert);
                    for (string line; (line = br.ReadLine()) != null; )
                    {
                        line = line.Trim();
                        String[] parts = line.Split(@"\t");
                        double[] wArr = new double[parts.length];
                        for (int i = 0; i < parts.length; i++)
                        {
                            wArr[i] = Double.ParseDouble(parts[i]);
                        }

                        listOfWeights.Add(wArr);
                    }

                    Console.Error.WriteLine(@"Done!");
                    for (int i = 0; i < numLopExpert; i++)
                        lopExpertWeights[i] = listOfWeights.Get(i);
                }
                catch (IOException e)
                {
                    throw new Exception(@"Could not read from double initial LOP weights file " + flags.initialLopWeights);
                }
            }
            else
            {
                for (int lopIter = 0; lopIter < numLopExpert; lopIter++)
                {
                    int[][][][] partialData = CreatePartialDataForLOP(lopIter, data);
                    if (flags.randomLopWeights)
                    {
                        lopExpertWeights[lopIter] = InitWeightsUsingDoubleCRF(partialData, labels, evaluators, pruneFeatureItr);
                    }
                    else
                    {
                        lopExpertWeights[lopIter] = TrainWeightsUsingDoubleCRF(partialData, labels, evaluators, pruneFeatureItr, null);
                    }
                }

                if (flags.includeFullCRFInLOP)
                {
                    double[][] newLopExpertWeights = new double[numLopExpert + 1];
                    Array.Copy(lopExpertWeights, 0, newLopExpertWeights, 0, lopExpertWeights.length);
                    if (flags.randomLopWeights)
                    {
                        newLopExpertWeights[numLopExpert] = InitWeightsUsingDoubleCRF(data, labels, evaluators, pruneFeatureItr);
                    }
                    else
                    {
                        newLopExpertWeights[numLopExpert] = TrainWeightsUsingDoubleCRF(data, labels, evaluators, pruneFeatureItr, null);
                    }

                    Set<int> newSet = Generics.NewHashSet(numFeatures);
                    List<int> newList = new List<int>(numFeatures);
                    for (int fIndex = 0; fIndex < numFeatures; fIndex++)
                    {
                        newSet.Add(fIndex);
                        newList.Add(fIndex);
                    }

                    featureIndicesSetArray.Add(newSet);
                    featureIndicesListArray.Add(newList);
                    numLopExpert += 1;
                    lopExpertWeights = newLopExpertWeights;
                }
            }

            CRFLogConditionalObjectiveFunctionForLOP func = new CRFLogConditionalObjectiveFunctionForLOP(data, labels, lopExpertWeights, windowSize, classIndex, labelIndices, map, flags.backgroundSymbol, numLopExpert, featureIndicesSetArray, featureIndicesListArray, flags.backpropLopTraining);
            cliquePotentialFunctionHelper = func;
            Minimizer minimizer = GetMinimizer(0, evaluators);
            double[] initialScales;
            if (flags.initialLopScales == null)
            {
                initialScales = func.Initial();
            }
            else
            {
                try
                {
                    Console.Error.WriteLine(@"Reading initial LOP scales from file " + flags.initialLopScales);
                    DataInputStream dis = new DataInputStream(new BufferedInputStream(new GZIPInputStream(new FileInputStream(flags.initialLopScales))));
                    initialScales = ConvertByteArray.ReadDoubleArr(dis);
                }
                catch (IOException e)
                {
                    throw new Exception(@"Could not read from double initial LOP scales file " + flags.initialLopScales);
                }
            }

            double[] learnedParams = minimizer.Minimize(func, flags.tolerance, initialScales);
            double[] rawScales = func.SeparateLopScales(learnedParams);
            double[] lopScales = ArrayMath.Softmax(rawScales);
            Console.Error.WriteLine(@"After SoftMax Transformation, learned scales are:");
            for (int lopIter = 0; lopIter < numLopExpert; lopIter++)
            {
                Console.Error.WriteLine(@"lopScales[" + lopIter + @"] = " + lopScales[lopIter]);
            }

            double[][] learnedLopExpertWeights = lopExpertWeights;
            if (flags.backpropLopTraining)
            {
                learnedLopExpertWeights = func.SeparateLopExpertWeights(learnedParams);
            }

            return CRFLogConditionalObjectiveFunctionForLOP.CombineAndScaleLopWeights(numLopExpert, learnedLopExpertWeights, lopScales);
        }

        protected virtual double[] InitWeightsUsingDoubleCRF(int[][][][] data, int[][] labels, Evaluator[] evaluators, int pruneFeatureItr)
        {
            CRFLogConditionalObjectiveFunction func = new CRFLogConditionalObjectiveFunction(data, labels, windowSize, classIndex, labelIndices, map, flags.priorType, flags.backgroundSymbol, flags.sigma, null);
            return func.Initial();
        }

        protected virtual double[] TrainWeightsUsingNonLinearCRF(AbstractCachingDiffFunction func, Evaluator[] evaluators)
        {
            Minimizer minimizer = GetMinimizer(0, evaluators);
            double[] initialWeights;
            if (flags.initialWeights == null)
            {
                initialWeights = func.Initial();
            }
            else
            {
                try
                {
                    Console.Error.WriteLine(@"Reading initial weights from file " + flags.initialWeights);
                    DataInputStream dis = new DataInputStream(new BufferedInputStream(new GZIPInputStream(new FileInputStream(flags.initialWeights))));
                    initialWeights = ConvertByteArray.ReadDoubleArr(dis);
                }
                catch (IOException e)
                {
                    throw new Exception(@"Could not read from double initial weight file " + flags.initialWeights);
                }
            }

            Console.Error.WriteLine(@"numWeights: " + initialWeights.length);
            if (flags.testObjFunction)
            {
                StochasticDiffFunctionTester tester = new StochasticDiffFunctionTester(func);
                if (tester.TestSumOfBatches(initialWeights, 0.0))
                {
                    Console.Error.WriteLine(@"Testing complete... exiting");
                    System.Exit(1);
                }
                else
                {
                    Console.Error.WriteLine(@"Testing failed....exiting");
                    System.Exit(1);
                }
            }

            if (flags.checkGradient)
            {
                if (func.GradientCheck())
                {
                    Console.Error.WriteLine(@"gradient check passed");
                }
                else
                {
                    throw new Exception(@"gradient check failed");
                }
            }

            return minimizer.Minimize(func, flags.tolerance, initialWeights);
        }

        protected virtual double[] TrainWeightsUsingDoubleCRF(int[][][][] data, int[][] labels, Evaluator[] evaluators, int pruneFeatureItr, double[][][][] featureVals)
        {
            CRFLogConditionalObjectiveFunction func = new CRFLogConditionalObjectiveFunction(data, labels, windowSize, classIndex, labelIndices, map, flags.priorType, flags.backgroundSymbol, flags.sigma, featureVals);
            cliquePotentialFunctionHelper = func;
            IDictionary<String, Set<int>> featureSets = null;
            if (flags.groupByOutputClass)
            {
                featureSets = new HashIDictionary<String, Set<int>>();
                if (flags.groupByFeatureTemplate)
                {
                    int pIndex = 0;
                    for (int fIndex = 0; fIndex < map.length; fIndex++)
                    {
                        int cliqueType = map[fIndex];
                        int numCliqueTypeOutputClass = labelIndices.Get(map[fIndex]).Size();
                        for (int cliqueOutClass = 0; cliqueOutClass < numCliqueTypeOutputClass; cliqueOutClass++)
                        {
                            string name = @"c:" + cliqueType + @"-o:" + cliqueOutClass + @"-g:" + featureIndexToTemplateIndex.Get(fIndex);
                            if (featureSets.ContainsKey(name))
                            {
                                featureSets.Get(name).Add(pIndex);
                            }
                            else
                            {
                                Set<int> newSet = new HashSet<int>();
                                newSet.Add(pIndex);
                                featureSets.Put(name, newSet);
                            }

                            pIndex++;
                        }
                    }
                }
                else
                {
                    int pIndex = 0;
                    foreach (int cliqueType in map)
                    {
                        int numCliqueTypeOutputClass = labelIndices.Get(cliqueType).Size();
                        for (int cliqueOutClass = 0; cliqueOutClass < numCliqueTypeOutputClass; cliqueOutClass++)
                        {
                            string name = @"c:" + cliqueType + @"-o:" + cliqueOutClass;
                            if (featureSets.ContainsKey(name))
                            {
                                featureSets.Get(name).Add(pIndex);
                            }
                            else
                            {
                                Set<int> newSet = new HashSet<int>();
                                newSet.Add(pIndex);
                                featureSets.Put(name, newSet);
                            }

                            pIndex++;
                        }
                    }
                }
            }
            else if (flags.groupByFeatureTemplate)
            {
                featureSets = new HashIDictionary<String, Set<int>>();
                int pIndex = 0;
                for (int fIndex = 0; fIndex < map.length; fIndex++)
                {
                    int cliqueType = map[fIndex];
                    int numCliqueTypeOutputClass = labelIndices.Get(map[fIndex]).Size();
                    for (int cliqueOutClass = 0; cliqueOutClass < numCliqueTypeOutputClass; cliqueOutClass++)
                    {
                        string name = @"c:" + cliqueType + @"-g:" + featureIndexToTemplateIndex.Get(fIndex);
                        if (featureSets.ContainsKey(name))
                        {
                            featureSets.Get(name).Add(pIndex);
                        }
                        else
                        {
                            Set<int> newSet = new HashSet<int>();
                            newSet.Add(pIndex);
                            featureSets.Put(name, newSet);
                        }

                        pIndex++;
                    }
                }
            }

            if (featureSets != null)
            {
                int[][] fg = new int[featureSets.Size()];
                Console.Error.WriteLine(@"After feature grouping, total of " + fg.length + @" groups");
                int count = 0;
                foreach (Set<int> aSet in featureSets.Values())
                {
                    fg[count] = new int[aSet.Size()];
                    int i = 0;
                    foreach (int val in aSet)
                        fg[count][i++] = val;
                    count++;
                }

                func.SetFeatureGrouping(fg);
            }

            Minimizer minimizer = GetMinimizer(pruneFeatureItr, evaluators);
            double[] initialWeights;
            if (flags.initialWeights == null)
            {
                initialWeights = func.Initial();
            }
            else
            {
                try
                {
                    Console.Error.WriteLine(@"Reading initial weights from file " + flags.initialWeights);
                    DataInputStream dis = new DataInputStream(new BufferedInputStream(new GZIPInputStream(new FileInputStream(flags.initialWeights))));
                    initialWeights = ConvertByteArray.ReadDoubleArr(dis);
                }
                catch (IOException e)
                {
                    throw new Exception(@"Could not read from double initial weight file " + flags.initialWeights);
                }
            }

            Console.Error.WriteLine(@"numWeights: " + initialWeights.length);
            if (flags.testObjFunction)
            {
                StochasticDiffFunctionTester tester = new StochasticDiffFunctionTester(func);
                if (tester.TestSumOfBatches(initialWeights, 0.0))
                {
                    Console.Error.WriteLine(@"Testing complete... exiting");
                    System.Exit(1);
                }
                else
                {
                    Console.Error.WriteLine(@"Testing failed....exiting");
                    System.Exit(1);
                }
            }

            if (flags.checkGradient)
            {
                if (func.GradientCheck())
                {
                    Console.Error.WriteLine(@"gradient check passed");
                }
                else
                {
                    throw new Exception(@"gradient check failed");
                }
            }

            return minimizer.Minimize(func, flags.tolerance, initialWeights);
        }

        protected virtual Minimizer GetMinimizer()
        {
            return GetMinimizer(0, null);
        }

        protected virtual Minimizer GetMinimizer(int featurePruneIteration, Evaluator[] evaluators)
        {
            Minimizer<DiffFunction> minimizer = null;
            if (flags.useQN)
            {
                int QNmem;
                if (featurePruneIteration == 0)
                {
                    QNmem = flags.QNsize;
                }
                else
                {
                    QNmem = flags.QNsize2;
                }

                if (flags.interimOutputFreq != 0)
                {
                    Function monitor = new ResultStoringMonitor(flags.interimOutputFreq, flags.serializeTo);
                    minimizer = new QNMinimizer(monitor, QNmem, flags.useRobustQN);
                }
                else
                {
                    minimizer = new QNMinimizer(QNmem, flags.useRobustQN);
                }

                ((QNMinimizer)minimizer).TerminateOnEvalImprovement(flags.terminateOnEvalImprovement);
                ((QNMinimizer)minimizer).SetTerminateOnEvalImprovementNumOfEpoch(flags.terminateOnEvalImprovementNumOfEpoch);
                ((QNMinimizer)minimizer).SuppressTestPrompt(flags.suppressTestDebug);
                if (flags.useOWLQN)
                {
                    ((QNMinimizer)minimizer).UseOWLQN(flags.useOWLQN, flags.priorLambda);
                }
            }
            else if (flags.useInPlaceSGD)
            {
                StochasticInPlaceMinimizer<DiffFunction> sgdMinimizer = new StochasticInPlaceMinimizer<DiffFunction>(flags.sigma, flags.SGDPasses, flags.tuneSampleSize, flags.stochasticBatchSize);
                if (flags.useSGDtoQN)
                {
                    QNMinimizer qnMinimizer;
                    int QNmem;
                    if (featurePruneIteration == 0)
                    {
                        QNmem = flags.QNsize;
                    }
                    else
                    {
                        QNmem = flags.QNsize2;
                    }

                    if (flags.interimOutputFreq != 0)
                    {
                        Function monitor = new ResultStoringMonitor(flags.interimOutputFreq, flags.serializeTo);
                        qnMinimizer = new QNMinimizer(monitor, QNmem, flags.useRobustQN);
                    }
                    else
                    {
                        qnMinimizer = new QNMinimizer(QNmem, flags.useRobustQN);
                    }

                    minimizer = new HybridMinimizer(sgdMinimizer, qnMinimizer, flags.SGDPasses);
                }
                else
                {
                    minimizer = sgdMinimizer;
                }
            }
            else if (flags.useAdaGradFOBOS)
            {
                minimizer = new SGDWithAdaGradAndFOBOS(flags.initRate, flags.priorLambda, flags.SGDPasses, flags.tuneSampleSize, flags.stochasticBatchSize, flags.priorType, flags.priorAlpha);
                ((SGDWithAdaGradAndFOBOS)minimizer).TerminateOnEvalImprovement(flags.terminateOnEvalImprovement);
                ((SGDWithAdaGradAndFOBOS)minimizer).SetTerminateOnEvalImprovementNumOfEpoch(flags.terminateOnEvalImprovementNumOfEpoch);
                ((SGDWithAdaGradAndFOBOS)minimizer).SuppressTestPrompt(flags.suppressTestDebug);
            }
            else if (flags.useSGDtoQN)
            {
                minimizer = new SGDToQNMinimizer(flags.initialGain, flags.stochasticBatchSize, flags.SGDPasses, flags.QNPasses, flags.SGD2QNhessSamples, flags.QNsize, flags.outputIterationsToFile);
            }
            else if (flags.useSMD)
            {
                minimizer = new SMDMinimizer(flags.initialGain, flags.stochasticBatchSize, flags.stochasticMethod, flags.SGDPasses);
            }
            else if (flags.useSGD)
            {
                minimizer = new SGDMinimizer(flags.initialGain, flags.stochasticBatchSize);
            }
            else if (flags.useScaledSGD)
            {
                minimizer = new ScaledSGDMinimizer(flags.initialGain, flags.stochasticBatchSize, flags.SGDPasses, flags.scaledSGDMethod);
            }
            else if (flags.l1reg > 0.0)
            {
                minimizer = ReflectionLoading.LoadByReflection(@"edu.stanford.nlp.optimization.OWLQNMinimizer", flags.l1reg);
            }

            if (minimizer is HasEvaluators)
            {
                ((HasEvaluators)minimizer).SetEvaluators(flags.evaluateIters, evaluators);
            }

            if (minimizer == null)
            {
                throw new Exception(@"No minimizer assigned!");
            }

            return minimizer;
        }

        protected virtual List<CRFDatum<T, string>> ExtractDatumSequence<T>(int[][][] allData, int beginPosition, int endPosition, List<IN> labeledWordInfos)
            where T : ICollection<string>
        {
            List<CRFDatum<T, string>> result = new List<CRFDatum<T, string>>();
            int beginContext = beginPosition - windowSize + 1;
            if (beginContext < 0)
            {
                beginContext = 0;
            }

            for (int position = beginContext; position < beginPosition; position++)
            {
                List<Collection<String>> cliqueFeatures = new List<ICollection<String>>();
                List<double[]> featureVals = new List<double[]>();
                for (int i = 0; i < windowSize; i++)
                {
                    cliqueFeatures.Add(Collections.EmptyList());
                    featureVals.Add(null);
                }

                CRFDatum<ICollection<String>, String> datum = new CRFDatum<ICollection<String>, String>(cliqueFeatures, labeledWordInfos.Get(position).Get(typeof(CoreAnnotations.AnswerAnnotation)), featureVals);
                result.Add(datum);
            }

            for (int position = beginPosition; position <= endPosition; position++)
            {
                List<ICollection<String>> cliqueFeatures = new List<ICollection<String>>();
                List<double[]> featureVals = new List<double[]>();
                for (int i = 0; i < windowSize; i++)
                {
                    Collection<String> features = new List<String>();
                    for (int j = 0; j < allData[position][i].length; j++)
                    {
                        features.Add(featureIndex.Get(allData[position][i][j]));
                    }

                    cliqueFeatures.Add(features);
                    featureVals.Add(null);
                }

                CRFDatum<Collection<String>, String> datum = new CRFDatum<Collection<String>, String>(cliqueFeatures, labeledWordInfos.Get(position).Get(typeof(CoreAnnotations.AnswerAnnotation)), featureVals);
                result.Add(datum);
            }

            return result;
        }

        protected virtual void AddProcessedData(List<List<CRFDatum<Collection<String>, String>>> processedData, int[][][][] data, int[][] labels, double[][][][] featureVals, int offset)
        {
            for (int i = 0, pdSize = processedData.size(); i < pdSize; i++)
            {
                int dataIndex = i + offset;
                List<CRFDatum<Collection<String>, String>> document = processedData.Get(i);
                int dsize = document.Size();
                labels[dataIndex] = new int[dsize];
                data[dataIndex] = new int[dsize];
                if (featureVals != null)
                    featureVals[dataIndex] = new double[dsize];
                for (int j = 0; j < dsize; j++)
                {
                    CRFDatum<Collection<String>, String> crfDatum = document.Get(j);
                    labels[dataIndex][j] = classIndex.IndexOf(crfDatum.Label());
                    List<double[]> featureValList = null;
                    if (featureVals != null)
                        featureValList = crfDatum.AsFeatureVals();
                    List<Collection<String>> cliques = crfDatum.AsFeatures();
                    int csize = cliques.Size();
                    data[dataIndex][j] = new int[csize];
                    if (featureVals != null)
                        featureVals[dataIndex][j] = new double[csize];
                    for (int k = 0; k < csize; k++)
                    {
                        Collection<String> features = cliques.Get(k);
                        data[dataIndex][j][k] = new int[features.Size()];
                        if (featureVals != null)
                            featureVals[dataIndex][j][k] = featureValList.Get(k);
                        int m = 0;
                        try
                        {
                            foreach (string feature in features)
                            {
                                if (featureIndex == null)
                                {
                                    Console.Out.WriteLine(@"Feature is NULL!");
                                }

                                data[dataIndex][j][k][m] = featureIndex.IndexOf(feature);
                                m++;
                            }
                        }
                        catch (Exception e)
                        {
                            e.PrintStackTrace();
                            System.err.Printf(@"[index=%d, j=%d, k=%d, m=%d]%n", dataIndex, j, k, m);
                            Console.Error.WriteLine(@"data.length                    " + data.length);
                            Console.Error.WriteLine(@"data[dataIndex].length         " + data[dataIndex].length);
                            Console.Error.WriteLine(@"data[dataIndex][j].length      " + data[dataIndex][j].length);
                            Console.Error.WriteLine(@"data[dataIndex][j][k].length   " + data[dataIndex][j].length);
                            Console.Error.WriteLine(@"data[dataIndex][j][k][m]       " + data[dataIndex][j][k][m]);
                            return;
                        }
                    }
                }
            }
        }

        protected static void SaveProcessedData(List datums, string filename)
        {
            Console.Error.Write(@"Saving processed data of size " + datums.Size() + @" to serialized file...");
            ObjectOutputStream oos = null;
            try
            {
                oos = new ObjectOutputStream(new FileOutputStream(filename));
                oos.WriteObject(datums);
            }
            catch (IOException e)
            {
            }
            finally
            {
                IOUtils.CloseIgnoringExceptions(oos);
            }

            Console.Error.WriteLine(@"done.");
        }

        protected static List<List<CRFDatum<ICollection<String>, String>>> LoadProcessedData(string filename)
        {
            Console.Error.Write(@"Loading processed data from serialized file...");
            ObjectInputStream ois = null;
            List<List<CRFDatum<ICollection<String>, String>>> result = Collections.EmptyList();
            try
            {
                ois = new ObjectInputStream(new FileInputStream(filename));
                result = (List<List<CRFDatum<ICollection<String>, String>>>)ois.ReadObject();
            }
            catch (Exception e)
            {
                e.PrintStackTrace();
            }
            finally
            {
                IOUtils.CloseIgnoringExceptions(ois);
            }

            Console.Error.WriteLine(@"done. Got " + result.Size() + @" datums.");
            return result;
        }

        public virtual void LoadTextClassifier(string text, Properties props)
        {
            Console.Error.WriteLine(@"Loading Text Classifier from " + text);
            BufferedReader br = new BufferedReader(new InputStreamReader(new GZIPInputStream(new FileInputStream(text))));
            string line = br.ReadLine();
            String[] toks = line.Split(@"\\t");
            if (!toks[0].Equals(@"labelIndices.length="))
            {
                throw new Exception(@"format error");
            }

            int size = int.ParseInt(toks[1]);
            labelIndices = new List<Index<CRFLabel>>(size);
            for (int labelIndicesIdx = 0; labelIndicesIdx < size; labelIndicesIdx++)
            {
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                if (!(toks[0].StartsWith(@"labelIndices[") && toks[0].EndsWith(@"].size()=")))
                {
                    throw new Exception(@"format error");
                }

                int labelIndexSize = int.ParseInt(toks[1]);
                labelIndices.Add(new HashIndex<CRFLabel>());
                int count = 0;
                while (count < labelIndexSize)
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    int idx = int.ParseInt(toks[0]);
                    if (count != idx)
                    {
                        throw new Exception(@"format error");
                    }

                    String[] crflabelstr = toks[1].Split(@" ");
                    int[] crflabel = new int[crflabelstr.length];
                    for (int i = 0; i < crflabelstr.length; i++)
                    {
                        crflabel[i] = int.ParseInt(crflabelstr[i]);
                    }

                    CRFLabel crfL = new CRFLabel(crflabel);
                    labelIndices.Get(labelIndicesIdx).Add(crfL);
                    count++;
                }
            }

            System.err.Printf(@"DEBUG: labelIndices.length=\t%d%n", labelIndices.Size());
            for (int i = 0; i < labelIndices.Size(); i++)
            {
                System.err.Printf(@"DEBUG: labelIndices[%d].size()=\t%d%n", i, labelIndices.Get(i).Size());
                for (int j = 0; j < labelIndices.Get(i).Size(); j++)
                {
                    int[] label = labelIndices.Get(i).Get(j).GetLabel();
                    List<int> list = new List<int>();
                    foreach (int l in label)
                    {
                        list.Add(l);
                    }

                    System.err.Printf(@"DEBUG: %d\t%s%n", j, StringUtils.Join(list, @" "));
                }
            }

            line = br.ReadLine();
            toks = line.Split(@"\\t");
            if (!toks[0].Equals(@"classIndex.size()="))
            {
                throw new Exception(@"format error");
            }

            int classIndexSize = int.ParseInt(toks[1]);
            classIndex = new HashIndex<String>();
            int count = 0;
            while (count < classIndexSize)
            {
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                int idx = int.ParseInt(toks[0]);
                if (count != idx)
                {
                    throw new Exception(@"format error");
                }

                classIndex.Add(toks[1]);
                count++;
            }

            System.err.Printf(@"DEBUG: classIndex.size()=\t%d%n", classIndex.Size());
            for (int i = 0; i < classIndex.Size(); i++)
            {
                System.err.Printf(@"DEBUG: %d\t%s%n", i, classIndex.Get(i));
            }

            line = br.ReadLine();
            toks = line.Split(@"\\t");
            if (!toks[0].Equals(@"featureIndex.size()="))
            {
                throw new Exception(@"format error");
            }

            int featureIndexSize = int.ParseInt(toks[1]);
            featureIndex = new HashIndex<String>();
            count = 0;
            while (count < featureIndexSize)
            {
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                int idx = int.ParseInt(toks[0]);
                if (count != idx)
                {
                    throw new Exception(@"format error");
                }

                featureIndex.Add(toks[1]);
                count++;
            }

            System.err.Printf(@"DEBUG: featureIndex.size()=\t%d%n", featureIndex.Size());
            line = br.ReadLine();
            if (!line.Equals(@"<flags>"))
            {
                throw new Exception(@"format error");
            }

            Properties p = new Properties();
            line = br.ReadLine();
            while (!line.Equals(@"</flags>"))
            {
                String[] keyValue = line.Split(@"=");
                p.SetProperty(keyValue[0], keyValue[1]);
                line = br.ReadLine();
            }

            flags = new SeqClassifierFlags(p);
            Console.Error.WriteLine(@"DEBUG: <flags>");
            System.err.Print(flags.ToString());
            Console.Error.WriteLine(@"DEBUG: </flags>");
            if (flags.useEmbedding)
            {
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                if (!toks[0].Equals(@"embeddings.size()="))
                {
                    throw new Exception(@"format error in embeddings");
                }

                int embeddingSize = int.ParseInt(toks[1]);
                embeddings = Generics.NewHashMap(embeddingSize);
                count = 0;
                while (count < embeddingSize)
                {
                    line = br.ReadLine().Trim();
                    toks = line.Split(@"\\t");
                    string word = toks[0];
                    double[] arr = ArrayUtils.ToDoubleArray(toks[1].Split(@" "));
                    embeddings.Put(word, arr);
                    count++;
                }
            }

            if (flags.nonLinearCRF)
            {
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                if (!toks[0].Equals(@"nodeFeatureIndicesMap.size()="))
                {
                    throw new Exception(@"format error in nodeFeatureIndicesMap");
                }

                int nodeFeatureIndicesMapSize = int.ParseInt(toks[1]);
                nodeFeatureIndicesMap = new HashIndex<int>();
                count = 0;
                while (count < nodeFeatureIndicesMapSize)
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    int idx = int.ParseInt(toks[0]);
                    if (count != idx)
                    {
                        throw new Exception(@"format error");
                    }

                    nodeFeatureIndicesMap.Add(int.ParseInt(toks[1]));
                    count++;
                }

                System.err.Printf(@"DEBUG: nodeFeatureIndicesMap.size()=\t%d%n", nodeFeatureIndicesMap.Size());
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                if (!toks[0].Equals(@"edgeFeatureIndicesMap.size()="))
                {
                    throw new Exception(@"format error");
                }

                int edgeFeatureIndicesMapSize = int.ParseInt(toks[1]);
                edgeFeatureIndicesMap = new HashIndex<int>();
                count = 0;
                while (count < edgeFeatureIndicesMapSize)
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    int idx = int.ParseInt(toks[0]);
                    if (count != idx)
                    {
                        throw new Exception(@"format error");
                    }

                    edgeFeatureIndicesMap.Add(int.ParseInt(toks[1]));
                    count++;
                }

                System.err.Printf(@"DEBUG: edgeFeatureIndicesMap.size()=\t%d%n", edgeFeatureIndicesMap.Size());
                int weightsLength = -1;
                if (flags.secondOrderNonLinear)
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    if (!toks[0].Equals(@"inputLayerWeights4Edge.length="))
                    {
                        throw new Exception(@"format error");
                    }

                    weightsLength = int.ParseInt(toks[1]);
                    inputLayerWeights4Edge = new double[weightsLength];
                    count = 0;
                    while (count < weightsLength)
                    {
                        line = br.ReadLine();
                        toks = line.Split(@"\\t");
                        int weights2Length = int.ParseInt(toks[0]);
                        inputLayerWeights4Edge[count] = new double[weights2Length];
                        String[] weightsValue = toks[1].Split(@" ");
                        if (weights2Length != weightsValue.length)
                        {
                            throw new Exception(@"weights format error");
                        }

                        for (int i2 = 0; i2 < weights2Length; i2++)
                        {
                            inputLayerWeights4Edge[count][i2] = Double.ParseDouble(weightsValue[i2]);
                        }

                        count++;
                    }

                    System.err.Printf(@"DEBUG: double[%d][] inputLayerWeights4Edge loaded%n", weightsLength);
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    if (!toks[0].Equals(@"outputLayerWeights4Edge.length="))
                    {
                        throw new Exception(@"format error");
                    }

                    weightsLength = int.ParseInt(toks[1]);
                    outputLayerWeights4Edge = new double[weightsLength];
                    count = 0;
                    while (count < weightsLength)
                    {
                        line = br.ReadLine();
                        toks = line.Split(@"\\t");
                        int weights2Length = int.ParseInt(toks[0]);
                        outputLayerWeights4Edge[count] = new double[weights2Length];
                        String[] weightsValue = toks[1].Split(@" ");
                        if (weights2Length != weightsValue.length)
                        {
                            throw new Exception(@"weights format error");
                        }

                        for (int i2 = 0; i2 < weights2Length; i2++)
                        {
                            outputLayerWeights4Edge[count][i2] = Double.ParseDouble(weightsValue[i2]);
                        }

                        count++;
                    }

                    System.err.Printf(@"DEBUG: double[%d][] outputLayerWeights loaded%n", weightsLength);
                }
                else
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    if (!toks[0].Equals(@"linearWeights.length="))
                    {
                        throw new Exception(@"format error");
                    }

                    weightsLength = int.ParseInt(toks[1]);
                    linearWeights = new double[weightsLength];
                    count = 0;
                    while (count < weightsLength)
                    {
                        line = br.ReadLine();
                        toks = line.Split(@"\\t");
                        int weights2Length = int.ParseInt(toks[0]);
                        linearWeights[count] = new double[weights2Length];
                        String[] weightsValue = toks[1].Split(@" ");
                        if (weights2Length != weightsValue.length)
                        {
                            throw new Exception(@"weights format error");
                        }

                        for (int i2 = 0; i2 < weights2Length; i2++)
                        {
                            linearWeights[count][i2] = Double.ParseDouble(weightsValue[i2]);
                        }

                        count++;
                    }

                    System.err.Printf(@"DEBUG: double[%d][] linearWeights loaded%n", weightsLength);
                }

                line = br.ReadLine();
                toks = line.Split(@"\\t");
                if (!toks[0].Equals(@"inputLayerWeights.length="))
                {
                    throw new Exception(@"format error");
                }

                weightsLength = int.ParseInt(toks[1]);
                inputLayerWeights = new double[weightsLength];
                count = 0;
                while (count < weightsLength)
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    int weights2Length = int.ParseInt(toks[0]);
                    inputLayerWeights[count] = new double[weights2Length];
                    String[] weightsValue = toks[1].Split(@" ");
                    if (weights2Length != weightsValue.length)
                    {
                        throw new Exception(@"weights format error");
                    }

                    for (int i2 = 0; i2 < weights2Length; i2++)
                    {
                        inputLayerWeights[count][i2] = Double.ParseDouble(weightsValue[i2]);
                    }

                    count++;
                }

                System.err.Printf(@"DEBUG: double[%d][] inputLayerWeights loaded%n", weightsLength);
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                if (!toks[0].Equals(@"outputLayerWeights.length="))
                {
                    throw new Exception(@"format error");
                }

                weightsLength = int.ParseInt(toks[1]);
                outputLayerWeights = new double[weightsLength];
                count = 0;
                while (count < weightsLength)
                {
                    line = br.ReadLine();
                    toks = line.Split(@"\\t");
                    int weights2Length = int.ParseInt(toks[0]);
                    outputLayerWeights[count] = new double[weights2Length];
                    String[] weightsValue = toks[1].Split(@" ");
                    if (weights2Length != weightsValue.length)
                    {
                        throw new Exception(@"weights format error");
                    }

                    for (int i2 = 0; i2 < weights2Length; i2++)
                    {
                        outputLayerWeights[count][i2] = Double.ParseDouble(weightsValue[i2]);
                    }

                    count++;
                }

                System.err.Printf(@"DEBUG: double[%d][] outputLayerWeights loaded%n", weightsLength);
            }

            line = br.ReadLine();
            String[] featureFactoryName = line.Split(@" ");
            if (!featureFactoryName[0].Equals(@"<featureFactory>") || !featureFactoryName[2].Equals(@"</featureFactory>"))
            {
                throw new Exception(@"format error");
            }

            featureFactory = (edu.stanford.nlp.sequences.FeatureFactory<IN>)Class.ForName(featureFactoryName[1]).NewInstance();
            featureFactory.Init(flags);
            Reinit();
            line = br.ReadLine();
            String[] windowSizeName = line.Split(@" ");
            if (!windowSizeName[0].Equals(@"<windowSize>") || !windowSizeName[2].Equals(@"</windowSize>"))
            {
                throw new Exception(@"format error");
            }

            windowSize = int.ParseInt(windowSizeName[1]);
            line = br.ReadLine();
            toks = line.Split(@"\\t");
            if (!toks[0].Equals(@"weights.length="))
            {
                throw new Exception(@"format error");
            }

            int weightsLength = int.ParseInt(toks[1]);
            weights = new double[weightsLength];
            count = 0;
            while (count < weightsLength)
            {
                line = br.ReadLine();
                toks = line.Split(@"\\t");
                int weights2Length = int.ParseInt(toks[0]);
                weights[count] = new double[weights2Length];
                String[] weightsValue = toks[1].Split(@" ");
                if (weights2Length != weightsValue.length)
                {
                    throw new Exception(@"weights format error");
                }

                for (int i2 = 0; i2 < weights2Length; i2++)
                {
                    weights[count][i2] = Double.ParseDouble(weightsValue[i2]);
                }

                count++;
            }

            System.err.Printf(@"DEBUG: double[%d][] weights loaded%n", weightsLength);
            line = br.ReadLine();
            if (line != null)
            {
                throw new Exception(@"weights format error");
            }
        }

        public virtual void SerializeTextClassifier(string serializePath)
        {
            System.err.Print(@"Serializing Text classifier to " + serializePath + @"...");
            try
            {
                PrintWriter pw = new PrintWriter(new GZIPOutputStream(new FileOutputStream(serializePath)));
                pw.Printf(@"labelIndices.length=\t%d%n", labelIndices.Size());
                for (int i = 0; i < labelIndices.Size(); i++)
                {
                    pw.Printf(@"labelIndices[%d].size()=\t%d%n", i, labelIndices.Get(i).Size());
                    for (int j = 0; j < labelIndices.Get(i).Size(); j++)
                    {
                        int[] label = labelIndices.Get(i).Get(j).GetLabel();
                        List<int> list = new List<int>();
                        foreach (int l in label)
                        {
                            list.Add(l);
                        }

                        pw.Printf(@"%d\t%s%n", j, StringUtils.Join(list, @" "));
                    }
                }

                pw.Printf(@"classIndex.size()=\t%d%n", classIndex.Size());
                for (int i = 0; i < classIndex.Size(); i++)
                {
                    pw.Printf(@"%d\t%s%n", i, classIndex.Get(i));
                }

                pw.Printf(@"featureIndex.size()=\t%d%n", featureIndex.Size());
                for (int i = 0; i < featureIndex.Size(); i++)
                {
                    pw.Printf(@"%d\t%s%n", i, featureIndex.Get(i));
                }

                pw.Println(@"<flags>");
                pw.Print(flags.ToString());
                pw.Println(@"</flags>");
                if (flags.useEmbedding)
                {
                    pw.Printf(@"embeddings.size()=\t%d%n", embeddings.Size());
                    foreach (string word in embeddings.KeySet())
                    {
                        double[] arr = embeddings.Get(word);
                        Double[] arrUnboxed = new Double[arr.length];
                        for (int i = 0; i < arr.length; i++)
                            arrUnboxed[i] = arr[i];
                        pw.Printf(@"%s\t%s%n", word, StringUtils.Join(arrUnboxed, @" "));
                    }
                }

                if (flags.nonLinearCRF)
                {
                    pw.Printf(@"nodeFeatureIndicesMap.size()=\t%d%n", nodeFeatureIndicesMap.Size());
                    for (int i = 0; i < nodeFeatureIndicesMap.Size(); i++)
                    {
                        pw.Printf(@"%d\t%d%n", i, nodeFeatureIndicesMap.Get(i));
                    }

                    pw.Printf(@"edgeFeatureIndicesMap.size()=\t%d%n", edgeFeatureIndicesMap.Size());
                    for (int i = 0; i < edgeFeatureIndicesMap.Size(); i++)
                    {
                        pw.Printf(@"%d\t%d%n", i, edgeFeatureIndicesMap.Get(i));
                    }

                    if (flags.secondOrderNonLinear)
                    {
                        pw.Printf(@"inputLayerWeights4Edge.length=\t%d%n", inputLayerWeights4Edge.length);
                        foreach (double[] ws in inputLayerWeights4Edge)
                        {
                            List<Double> list = new List<Double>();
                            foreach (double w in ws)
                            {
                                list.Add(w);
                            }

                            pw.Printf(@"%d\t%s%n", ws.length, StringUtils.Join(list, @" "));
                        }

                        pw.Printf(@"outputLayerWeights4Edge.length=\t%d%n", outputLayerWeights4Edge.length);
                        foreach (double[] ws in outputLayerWeights4Edge)
                        {
                            List<Double> list = new List<Double>();
                            foreach (double w in ws)
                            {
                                list.Add(w);
                            }

                            pw.Printf(@"%d\t%s%n", ws.length, StringUtils.Join(list, @" "));
                        }
                    }
                    else
                    {
                        pw.Printf(@"linearWeights.length=\t%d%n", linearWeights.length);
                        foreach (double[] ws in linearWeights)
                        {
                            List<Double> list = new List<Double>();
                            foreach (double w in ws)
                            {
                                list.Add(w);
                            }

                            pw.Printf(@"%d\t%s%n", ws.length, StringUtils.Join(list, @" "));
                        }
                    }

                    pw.Printf(@"inputLayerWeights.length=\t%d%n", inputLayerWeights.length);
                    foreach (double[] ws in inputLayerWeights)
                    {
                        List<Double> list = new List<Double>();
                        foreach (double w in ws)
                        {
                            list.Add(w);
                        }

                        pw.Printf(@"%d\t%s%n", ws.length, StringUtils.Join(list, @" "));
                    }

                    pw.Printf(@"outputLayerWeights.length=\t%d%n", outputLayerWeights.length);
                    foreach (double[] ws in outputLayerWeights)
                    {
                        List<Double> list = new List<Double>();
                        foreach (double w in ws)
                        {
                            list.Add(w);
                        }

                        pw.Printf(@"%d\t%s%n", ws.length, StringUtils.Join(list, @" "));
                    }
                }

                pw.Printf(@"<featureFactory> %s </featureFactory>%n", featureFactory.GetType().GetName());
                pw.Printf(@"<windowSize> %d </windowSize>%n", windowSize);
                pw.Printf(@"weights.length=\t%d%n", weights.length);
                foreach (double[] ws in weights)
                {
                    List<Double> list = new List<Double>();
                    foreach (double w in ws)
                    {
                        list.Add(w);
                    }

                    pw.Printf(@"%d\t%s%n", ws.length, StringUtils.Join(list, @" "));
                }

                pw.Close();
                Console.Error.WriteLine(@"done.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Failed");
                e.PrintStackTrace();
            }
        }

        public override void SerializeClassifier(string serializePath)
        {
            System.err.Print(@"Serializing classifier to " + serializePath + @"...");
            ObjectOutputStream oos = null;
            try
            {
                oos = IOUtils.WriteStreamFromString(serializePath);
                SerializeClassifier(oos);
                Console.Error.WriteLine(@"done.");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Failed");
                e.PrintStackTrace();
            }
            finally
            {
                IOUtils.CloseIgnoringExceptions(oos);
            }
        }

        public virtual void SerializeClassifier(ObjectOutputStream oos)
        {
            try
            {
                oos.WriteObject(labelIndices);
                oos.WriteObject(classIndex);
                oos.WriteObject(featureIndex);
                oos.WriteObject(flags);
                if (flags.useEmbedding)
                    oos.WriteObject(embeddings);
                if (flags.nonLinearCRF)
                {
                    oos.WriteObject(nodeFeatureIndicesMap);
                    oos.WriteObject(edgeFeatureIndicesMap);
                    if (flags.secondOrderNonLinear)
                    {
                        oos.WriteObject(inputLayerWeights4Edge);
                        oos.WriteObject(outputLayerWeights4Edge);
                    }
                    else
                    {
                        oos.WriteObject(linearWeights);
                    }

                    oos.WriteObject(inputLayerWeights);
                    oos.WriteObject(outputLayerWeights);
                }

                oos.WriteObject(featureFactory);
                oos.WriteInt(windowSize);
                oos.WriteObject(weights);
                oos.WriteObject(knownLCWords);
            }
            catch (IOException e)
            {
                throw new RuntimeIOException(e);
            }
        }

        public override void LoadClassifier(ObjectInputStream ois, Properties props)
        {
            Object o = ois.ReadObject();
            if (o is List)
            {
                labelIndices = (List<Index<CRFLabel>>)o;
            }
            else
            {
                Index<CRFLabel>[] indexArray = (Index<CRFLabel>[])o;
                labelIndices = new List<Index<CRFLabel>>(indexArray.length);
                for (int i = 0; i < indexArray.length; ++i)
                {
                    labelIndices.Add(indexArray[i]);
                }
            }

            classIndex = (Index<String>)ois.ReadObject();
            featureIndex = (Index<String>)ois.ReadObject();
            flags = (SeqClassifierFlags)ois.ReadObject();
            if (flags.useEmbedding)
            {
                embeddings = (IDictionary<String, double[]>)ois.ReadObject();
            }

            if (flags.nonLinearCRF)
            {
                nodeFeatureIndicesMap = (Index<int>)ois.ReadObject();
                edgeFeatureIndicesMap = (Index<int>)ois.ReadObject();
                if (flags.secondOrderNonLinear)
                {
                    inputLayerWeights4Edge = (double[][])ois.ReadObject();
                    outputLayerWeights4Edge = (double[][])ois.ReadObject();
                }
                else
                {
                    linearWeights = (double[][])ois.ReadObject();
                }

                inputLayerWeights = (double[][])ois.ReadObject();
                outputLayerWeights = (double[][])ois.ReadObject();
            }

            featureFactory = (edu.stanford.nlp.sequences.FeatureFactory)ois.ReadObject();
            if (props != null)
            {
                flags.SetProperties(props, false);
            }

            Reinit();
            windowSize = ois.ReadInt();
            weights = (double[][])ois.ReadObject();
            knownLCWords = (Set<String>)ois.ReadObject();
            if (VERBOSE)
            {
                Console.Error.WriteLine(@"windowSize=" + windowSize);
                Console.Error.WriteLine(@"flags=\n" + flags);
            }
        }

        public virtual void LoadDefaultClassifier()
        {
            LoadJarClassifier(DEFAULT_CLASSIFIER, null);
        }

        public virtual void LoadTagIndex()
        {
            if (flags.useNERPriorBIO)
            {
                if (tagIndex == null)
                {
                    tagIndex = new HashIndex<String>();
                    foreach (string tag in classIndex.ObjectsList())
                    {
                        String[] parts = tag.Split(@"-");
                        if (parts.length > 1)
                            tagIndex.Add(parts[parts.length - 1]);
                    }

                    tagIndex.Add(flags.backgroundSymbol);
                }

                if (entityMatrices == null)
                    entityMatrices = BisequenceEmpiricalNERPrior.ReadEntityMatrices(flags.entityMatrix, tagIndex);
            }
        }

        public virtual void LoadDefaultClassifier(Properties props)
        {
            LoadJarClassifier(DEFAULT_CLASSIFIER, props);
        }

        public static CRFClassifier<IN> GetDefaultClassifier()
        {
            CRFClassifier<IN> crf = new CRFClassifier<IN>();
            crf.LoadDefaultClassifier();
            return crf;
        }

        public static CRFClassifier<IN> GetDefaultClassifier(Properties props)
        {
            CRFClassifier<IN> crf = new CRFClassifier<IN>();
            crf.LoadDefaultClassifier(props);
            return crf;
        }

        public static CRFClassifier<IN> GetJarClassifier(string resourceName, Properties props)
        {
            CRFClassifier<IN> crf = new CRFClassifier<IN>();
            crf.LoadJarClassifier(resourceName, props);
            return crf;
        }

        public static CRFClassifier<IN> GetClassifier(File file)
        {
            CRFClassifier<IN> crf = new CRFClassifier<IN>();
            crf.LoadClassifier(file);
            return crf;
        }

        public static CRFClassifier<T> GetClassifier<T>(Stream in_renamed)
            where T : ICoreMap
        {
            CRFClassifier<T> crf = new CRFClassifier<ICoreMap>();
            crf.LoadClassifier(in_renamed);
            return crf;
        }

        public static CRFClassifier<CoreLabel> GetClassifierNoExceptions(string loadPath)
        {
            CRFClassifier<CoreLabel> crf = new CRFClassifier<CoreLabel>();
            crf.LoadClassifierNoExceptions(loadPath);
            return crf;
        }

        public static CRFClassifier<CoreLabel> GetClassifier(string loadPath)
        {
            CRFClassifier<CoreLabel> crf = new CRFClassifier<CoreLabel>();
            crf.LoadClassifier(loadPath);
            return crf;
        }

        public static CRFClassifier<T> GetClassifier<T>(string loadPath, Properties props)
            where T : ICoreMap
        {
            CRFClassifier<T> crf = new CRFClassifier<ICoreMap>();
            crf.LoadClassifier(loadPath, props);
            return crf;
        }

        public static void Main(String[] args)
        {
            StringUtils.PrintErrInvocationString(@"CRFClassifier", args);
            Properties props = StringUtils.ArgsToProperties(args);
            CRFClassifier<CoreLabel> crf = new CRFClassifier<CoreLabel>(props);
            string testFile = crf.flags.testFile;
            string testFiles = crf.flags.testFiles;
            string textFile = crf.flags.textFile;
            string textFiles = crf.flags.textFiles;
            string loadPath = crf.flags.loadClassifier;
            string loadTextPath = crf.flags.loadTextClassifier;
            string serializeTo = crf.flags.serializeTo;
            string serializeToText = crf.flags.serializeToText;
            if (crf.flags.useEmbedding && crf.flags.embeddingWords != null && crf.flags.embeddingVectors != null)
            {
                Console.Error.WriteLine(@"Reading Embedding Files");
                BufferedReader br = IOUtils.ReaderFromString(crf.flags.embeddingWords);
                string line = null;
                List<String> wordList = new List<String>();
                while ((line = br.ReadLine()) != null)
                {
                    wordList.Add(line.Trim());
                }

                Console.Error.WriteLine(@"Found a dictionary of size " + wordList.Size());
                br = new BufferedReader(new InputStreamReader(new FileInputStream(new File(crf.flags.embeddingVectors))));
                crf.embeddings = Generics.NewHashMap();
                double[] vector = null;
                int count = 0;
                while ((line = br.ReadLine()) != null)
                {
                    vector = ArrayUtils.ToDoubleArray(line.Trim().Split(@" "));
                    crf.embeddings.Put(wordList.Get(count++), vector);
                }

                Console.Error.WriteLine(@"Found " + count + @" matching embeddings of dimension " + vector.length);
            }

            if (loadPath != null)
            {
                crf.LoadClassifierNoExceptions(loadPath, props);
            }
            else if (loadTextPath != null)
            {
                Console.Error.WriteLine(@"Warning: this is now only tested for Chinese Segmenter");
                Console.Error.WriteLine(@"(Sun Dec 23 00:59:39 2007) (pichuan)");
                try
                {
                    crf.LoadTextClassifier(loadTextPath, props);
                }
                catch (Exception e)
                {
                    throw new Exception(@"error loading " + loadTextPath, e);
                }
            }
            else if (crf.flags.loadJarClassifier != null)
            {
                crf.LoadJarClassifier(crf.flags.loadJarClassifier, props);
            }
            else if (crf.flags.trainFile != null || crf.flags.trainFileList != null)
            {
                crf.Train();
            }
            else
            {
                crf.LoadDefaultClassifier();
            }

            crf.LoadTagIndex();
            if (serializeTo != null)
            {
                crf.SerializeClassifier(serializeTo);
            }

            if (serializeToText != null)
            {
                crf.SerializeTextClassifier(serializeToText);
            }

            if (testFile != null)
            {
                DocumentReaderAndWriter<CoreLabel> readerAndWriter = crf.DefaultReaderAndWriter();
                if (crf.flags.searchGraphPrefix != null)
                {
                    crf.ClassifyAndWriteViterbiSearchGraph(testFile, crf.flags.searchGraphPrefix, crf.MakeReaderAndWriter());
                }
                else if (crf.flags.printFirstOrderProbs)
                {
                    crf.PrintFirstOrderProbs(testFile, readerAndWriter);
                }
                else if (crf.flags.printFactorTable)
                {
                    crf.PrintFactorTable(testFile, readerAndWriter);
                }
                else if (crf.flags.printProbs)
                {
                    crf.PrintProbs(testFile, readerAndWriter);
                }
                else if (crf.flags.printProbs)
                {
                    crf.PrintProbs(testFile, readerAndWriter);
                }
                else if (crf.flags.useKBest)
                {
                    int k = crf.flags.kBest;
                    crf.ClassifyAndWriteAnswersKBest(testFile, k, readerAndWriter);
                }
                else if (crf.flags.printLabelValue)
                {
                    crf.PrintLabelInformation(testFile, readerAndWriter);
                }
                else
                {
                    crf.ClassifyAndWriteAnswers(testFile, readerAndWriter);
                }
            }

            if (testFiles != null)
            {
                List<File> files = new List<File>();
                foreach (string filename in testFiles.Split(@","))
                {
                    files.Add(new File(filename));
                }

                crf.ClassifyFilesAndWriteAnswers(files, crf.DefaultReaderAndWriter());
            }

            if (textFile != null)
            {
                crf.ClassifyAndWriteAnswers(textFile);
            }

            if (textFiles != null)
            {
                List<File> files = new List<File>();
                foreach (string filename in textFiles.Split(@","))
                {
                    files.Add(new File(filename));
                }

                crf.ClassifyFilesAndWriteAnswers(files);
            }

            if (crf.flags.readStdin)
            {
                crf.ClassifyStdin();
            }
        }

        public override IList<IN> ClassifyWithGlobalInformation(IList<IN> tokenSeq, ICoreMap doc, ICoreMap sent)
        {
            return Classify(tokenSeq);
        }

        public virtual void WriteWeights(PrintStream p)
        {
            foreach (string feature in featureIndex)
            {
                int index = featureIndex.IndexOf(feature);
                double[] v = weights[index];
                Index<CRFLabel> l = this.labelIndices.Get(0);
                p.Println(feature + @"\t\t");
                foreach (CRFLabel label in l)
                {
                    p.Print(label.ToString(classIndex) + @":" + v[l.IndexOf(label)] + @"\t");
                }

                p.Println();
            }
        }

        public virtual IDictionary<String, Counter<String>> TopWeights()
        {
            IDictionary<String, Counter<String>> w = new HashIDictionary<String, Counter<String>>();
            foreach (string feature in featureIndex)
            {
                int index = featureIndex.IndexOf(feature);
                double[] v = weights[index];
                Index<CRFLabel> l = this.labelIndices.Get(0);
                foreach (CRFLabel label in l)
                {
                    if (!w.ContainsKey(label.ToString(classIndex)))
                        w.Put(label.ToString(classIndex), new ClassicCounter<String>());
                    w.Get(label.ToString(classIndex)).SetCount(feature, v[l.IndexOf(label)]);
                }
            }

            return w;
        }
    }
}
