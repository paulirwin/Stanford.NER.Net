using Stanford.NER.Net.FSM;
using Stanford.NER.Net.Ling;
using Stanford.NER.Net.ObjectBank;
using Stanford.NER.Net.Process;
using Stanford.NER.Net.Sequences;
using Stanford.NER.Net.Stats;
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

namespace Stanford.NER.Net.IE
{
    public abstract class AbstractSequenceClassifier<IN> : IFunction<String, String>
        where IN : ICoreMap
    {
        public SeqClassifierFlags flags;
        public IIndex<String> classIndex;
        public FeatureFactory<IN> featureFactory;
        protected IN pad;
        private ICoreTokenFactory<IN> tokenFactory;
        protected int windowSize;
        protected ISet<String> knownLCWords = Collections.NewSetFromMap(new ConcurrentHashMap<String, Boolean>());
        private IDocumentReaderAndWriter<IN> defaultReaderAndWriter;

        public virtual IDocumentReaderAndWriter<IN> DefaultReaderAndWriter()
        {
            return defaultReaderAndWriter;
        }

        private readonly AtomicInteger threadCompletionCounter = new AtomicInteger(0);
        private IDocumentReaderAndWriter<IN> plainTextReaderAndWriter;

        public virtual IDocumentReaderAndWriter<IN> PlainTextReaderAndWriter()
        {
            return plainTextReaderAndWriter;
        }

        //public AbstractSequenceClassifier(Properties props)
        //    : this(new SeqClassifierFlags(props))
        //{
        //}

        public AbstractSequenceClassifier(SeqClassifierFlags flags)
        {
            this.flags = flags;
            this.featureFactory = new MetaClass(flags.featureFactory).CreateInstance<FeatureFactory<IN>>(flags.featureFactoryArgs);
            if (flags.tokenFactory == null)
            {
                tokenFactory = (ICoreTokenFactory<IN>)new CoreLabelTokenFactory();
            }
            else
            {
                this.tokenFactory = new MetaClass(flags.tokenFactory).CreateInstance<ICoreTokenFactory<IN>>(flags.tokenFactoryArgs);
            }

            pad = tokenFactory.MakeToken();
            windowSize = flags.maxLeft + 1;
            Reinit();
        }

        protected void Reinit()
        {
            pad.Set(typeof(CoreAnnotations.AnswerAnnotation), flags.backgroundSymbol);
            pad.Set(typeof(CoreAnnotations.GoldAnswerAnnotation), flags.backgroundSymbol);
            featureFactory.Init(flags);
            defaultReaderAndWriter = MakeReaderAndWriter();
            if (flags.readerAndWriter != null && flags.readerAndWriter.Equals(flags.plainTextDocumentReaderAndWriter))
            {
                plainTextReaderAndWriter = defaultReaderAndWriter;
            }
            else
            {
                plainTextReaderAndWriter = MakePlainTextReaderAndWriter();
            }
        }

        public virtual IDocumentReaderAndWriter<IN> MakeReaderAndWriter()
        {
            IDocumentReaderAndWriter<IN> readerAndWriter;
            try
            {
                readerAndWriter = ReflectionLoading.LoadByReflection(flags.readerAndWriter);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(@"Error loading flags.readerAndWriter: '%s'", flags.readerAndWriter), e);
            }

            readerAndWriter.Init(flags);
            return readerAndWriter;
        }

        public virtual IDocumentReaderAndWriter<IN> MakePlainTextReaderAndWriter()
        {
            string readerClassName = flags.plainTextDocumentReaderAndWriter;
            if (readerClassName == null)
            {
                readerClassName = SeqClassifierFlags.DEFAULT_PLAIN_TEXT_READER;
            }

            IDocumentReaderAndWriter<IN> readerAndWriter;
            try
            {
                readerAndWriter = ReflectionLoading.LoadByReflection(readerClassName);
            }
            catch (Exception e)
            {
                throw new Exception(String.Format(@"Error loading flags.plainTextDocumentReaderAndWriter: '%s'", flags.plainTextDocumentReaderAndWriter), e);
            }

            readerAndWriter.Init(flags);
            return readerAndWriter;
        }

        public virtual string BackgroundSymbol()
        {
            return flags.backgroundSymbol;
        }

        public virtual ISet<String> Labels()
        {
            return new HashSet<string>(classIndex.ObjectsList());
        }

        public virtual List<IN> ClassifySentence(IEnumerable<IHasWord> sentence)
        {
            List<IN> document = new List<IN>();
            int i = 0;
            foreach (IHasWord word in sentence)
            {
                IN wi;
                if (word is ICoreMap)
                {
                    wi = tokenFactory.MakeToken((IN)word);
                }
                else
                {
                    wi = tokenFactory.MakeToken();
                    wi.Set(typeof(CoreAnnotations.TextAnnotation), word.Word());
                }

                wi.Set(typeof(CoreAnnotations.PositionAnnotation), i.ToString());
                wi.Set(typeof(CoreAnnotations.AnswerAnnotation), BackgroundSymbol());
                document.Add(wi);
                i++;
            }

            ObjectBankWrapper<IN> wrapper = new ObjectBankWrapper<IN>(flags, null, knownLCWords);
            wrapper.ProcessDocument(document);
            Classify(document);
            return document;
        }

        public virtual List<IN> ClassifySentenceWithGlobalInformation(IEnumerable<IHasWord> tokenSequence, ICoreMap doc, ICoreMap sentence)
        {
            List<IN> document = new List<IN>();
            int i = 0;
            foreach (IHasWord word in tokenSequence)
            {
                IN wi;
                if (word is ICoreMap)
                {
                    wi = tokenFactory.MakeToken((IN)word);
                }
                else
                {
                    wi = tokenFactory.MakeToken();
                    wi.Set(typeof(CoreAnnotations.TextAnnotation), word.Word());
                }

                wi.Set(typeof(CoreAnnotations.PositionAnnotation), i.ToString());
                wi.Set(typeof(CoreAnnotations.AnswerAnnotation), BackgroundSymbol());
                document.Add(wi);
                i++;
            }

            ObjectBankWrapper<IN> wrapper = new ObjectBankWrapper<IN>(flags, null, knownLCWords);
            wrapper.ProcessDocument(document);
            ClassifyWithGlobalInformation(document, doc, sentence);
            return document;
        }

        public virtual ISequenceModel GetSequenceModel(List<IN> doc)
        {
            throw new NotSupportedException();
        }

        public virtual ISampler<List<IN>> GetSampler(List<IN> input)
        {
            return new AnonymousSampler(this, input);
        }

        private sealed class AnonymousSampler : ISampler<List<IN>>
        {
            public AnonymousSampler(AbstractSequenceClassifier<IN> parent, List<IN> input)
            {
                this.parent = parent;
                this.input = input;
                model = parent.GetSequenceModel(input);
            }

            private readonly AbstractSequenceClassifier<IN> parent;
            private readonly List<IN> input;

            ISequenceModel model;
            SequenceSampler sampler = new SequenceSampler();
            public override List<IN> DrawSample()
            {
                int[] sampleArray = sampler.BestSequence(model);
                List<IN> sample = new List<IN>();
                int i = 0;
                foreach (IN word in input)
                {
                    IN newWord = parent.tokenFactory.MakeToken(word);
                    newWord.Set(typeof(CoreAnnotations.AnswerAnnotation), parent.classIndex.Get(sampleArray[i++]));
                    sample.Add(newWord);
                }

                return sample;
            }
        }

        public virtual Counter<List<IN>> ClassifyKBest(List<IN> doc, Type answerField, int k)
        {
            if (doc.Count == 0)
            {
                return new ClassicCounter<List<IN>>();
            }

            ObjectBankWrapper<IN> obw = new ObjectBankWrapper<IN>(flags, null, knownLCWords);
            doc = obw.ProcessDocument(doc);
            ISequenceModel model = GetSequenceModel(doc);
            KBestSequenceFinder tagInference = new KBestSequenceFinder();
            Counter<int[]> bestSequences = tagInference.KBestSequences(model, k);
            Counter<List<IN>> kBest = new ClassicCounter<List<IN>>();
            foreach (int[] seq in bestSequences.KeySet())
            {
                List<IN> kth = new List<IN>();
                int pos = model.LeftWindow();
                foreach (IN fi in doc)
                {
                    IN newFL = tokenFactory.MakeToken(fi);
                    string guess = classIndex.Get(seq[pos]);
                    fi.Remove(typeof(CoreAnnotations.AnswerAnnotation));
                    newFL.Set(answerField, guess);
                    pos++;
                    kth.Add(newFL);
                }

                kBest.SetCount(kth, bestSequences.GetCount(seq));
            }

            return kBest;
        }

        public virtual DFSA<String, int> GetViterbiSearchGraph(List<IN> doc, Type answerField)
        {
            if (doc.Count == 0)
            {
                return new DFSA<String, int>(null);
            }

            ObjectBankWrapper<IN> obw = new ObjectBankWrapper<IN>(flags, null, knownLCWords);
            doc = obw.ProcessDocument(doc);
            ISequenceModel model = GetSequenceModel(doc);
            return ViterbiSearchGraphBuilder.GetGraph(model, classIndex);
        }

        public virtual List<List<IN>> Classify(string str)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromString(str, plainTextReaderAndWriter);
            List<List<IN>> result = new List<List<IN>>();
            foreach (List<IN> document in documents)
            {
                Classify(document);
                List<IN> sentence = new List<IN>();
                foreach (IN wi in document)
                {
                    sentence.Add(wi);
                }

                result.Add(sentence);
            }

            return result;
        }

        public virtual List<List<IN>> ClassifyRaw(string str, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromString(str, readerAndWriter);
            List<List<IN>> result = new List<List<IN>>();
            foreach (List<IN> document in documents)
            {
                Classify(document);
                List<IN> sentence = new List<IN>();
                foreach (IN wi in document)
                {
                    sentence.Add(wi);
                }

                result.Add(sentence);
            }

            return result;
        }

        public virtual List<List<IN>> ClassifyFile(string filename)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFile(filename, plainTextReaderAndWriter);
            List<List<IN>> result = new List<List<IN>>();
            foreach (List<IN> document in documents)
            {
                Classify(document);
                List<IN> sentence = new List<IN>();
                foreach (IN wi in document)
                {
                    sentence.Add(wi);
                }

                result.Add(sentence);
            }

            return result;
        }

        public override string Apply(string in_renamed)
        {
            return ClassifyWithInlineXML(in_renamed);
        }

        public virtual string ClassifyToString(string sentences, string outputFormat, bool preserveSpacing)
        {
            PlainTextDocumentReaderAndWriter.OutputStyle outFormat = PlainTextDocumentReaderAndWriter.OutputStyle.FromShortName(outputFormat);
            ObjectBank<List<IN>> documents = MakeObjectBankFromString(sentences, plainTextReaderAndWriter);
            StringBuilder sb = new StringBuilder();
            foreach (List<IN> doc in documents)
            {
                List<IN> docOutput = Classify(doc);
                if (plainTextReaderAndWriter is PlainTextDocumentReaderAndWriter)
                {
                    sb.Append(((PlainTextDocumentReaderAndWriter<IN>)plainTextReaderAndWriter).GetAnswers(docOutput, outFormat, preserveSpacing));
                }
                else
                {
                    StringWriter sw = new StringWriter();
                    TextWriter pw = sw;
                    plainTextReaderAndWriter.PrintAnswers(docOutput, pw);
                    pw.Flush();
                    sb.Append(sw.ToString());
                    sb.Append(@"\n");
                }
            }

            return sb.ToString();
        }

        public virtual string ClassifyWithInlineXML(string sentences)
        {
            return ClassifyToString(sentences, @"inlineXML", true);
        }

        public virtual string ClassifyToString(string sentences)
        {
            return ClassifyToString(sentences, @"slashTags", true);
        }

        public virtual List<Tuple<String, int, int>> ClassifyToCharacterOffsets(string sentences)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromString(sentences, plainTextReaderAndWriter);
            List<Tuple<String, int, int>> entities = new List<Tuple<String, int, int>>();
            foreach (List<IN> doc in documents)
            {
                string prevEntityType = flags.backgroundSymbol;
                Tuple<String, int, int> prevEntity = null;
                Classify(doc);
                foreach (IN fl in doc)
                {
                    string guessedAnswer = fl.Get(typeof(CoreAnnotations.AnswerAnnotation));
                    if (guessedAnswer.Equals(flags.backgroundSymbol))
                    {
                        if (prevEntity != null)
                        {
                            entities.Add(prevEntity);
                            prevEntity = null;
                        }
                    }
                    else
                    {
                        if (!guessedAnswer.Equals(prevEntityType))
                        {
                            if (prevEntity != null)
                            {
                                entities.Add(prevEntity);
                            }

                            prevEntity = new Tuple<String, int, int>(guessedAnswer, fl.Get(typeof(CoreAnnotations.CharacterOffsetBeginAnnotation)), fl.Get(typeof(CoreAnnotations.CharacterOffsetEndAnnotation)));
                        }
                        else
                        {
                            prevEntity.SetThird(fl.Get(typeof(CoreAnnotations.CharacterOffsetEndAnnotation)));
                        }
                    }

                    prevEntityType = guessedAnswer;
                }

                if (prevEntity != null)
                {
                    entities.Add(prevEntity);
                }
            }

            return entities;
        }

        public virtual IList<String> SegmentString(string sentence)
        {
            return SegmentString(sentence, defaultReaderAndWriter);
        }

        public virtual IList<String> SegmentString(string sentence, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            ObjectBank<IList<IN>> docs = MakeObjectBankFromString(sentence, readerAndWriter);
            StringWriter stringWriter = new StringWriter();
            TextWriter stringPrintWriter = stringWriter;
            foreach (IList<IN> doc in docs)
            {
                Classify(doc);
                readerAndWriter.PrintAnswers(doc, stringPrintWriter);
                stringPrintWriter.WriteLine();
            }

            stringPrintWriter.Close();
            string segmented = stringWriter.ToString();
            Regex r = new Regex("\\s");
            return r.Split(segmented);
        }

        public abstract IList<IN> Classify(IList<IN> document);
        public abstract IList<IN> ClassifyWithGlobalInformation(IList<IN> tokenSequence, ICoreMap document, ICoreMap sentence);

        public virtual void Train()
        {
            if (flags.trainFiles != null)
            {
                Train(flags.baseTrainDir, flags.trainFiles, defaultReaderAndWriter);
            }
            else if (flags.trainFileList != null)
            {
                String[] files = flags.trainFileList.Split(@",");
                Train(files, defaultReaderAndWriter);
            }
            else
            {
                Train(flags.trainFile, defaultReaderAndWriter);
            }
        }

        public virtual void Train(string filename)
        {
            Train(filename, defaultReaderAndWriter);
        }

        public virtual void Train(string filename, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = true;
            Train(MakeObjectBankFromFile(filename, readerAndWriter), readerAndWriter);
        }

        public virtual void Train(string baseTrainDir, string trainFiles, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = true;
            Train(MakeObjectBankFromFiles(baseTrainDir, trainFiles, readerAndWriter), readerAndWriter);
        }

        public virtual void Train(String[] trainFileList, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = true;
            Train(MakeObjectBankFromFiles(trainFileList, readerAndWriter), readerAndWriter);
        }

        public virtual void Train(ICollection<List<IN>> docs)
        {
            Train(docs, defaultReaderAndWriter);
        }

        public abstract void Train(ICollection<List<IN>> docs, IDocumentReaderAndWriter<IN> readerAndWriter);

        public virtual ObjectBank<List<IN>> MakeObjectBankFromString(string string_renamed, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            if (flags.announceObjectBankEntries)
            {
                Console.Error.Write(@"Reading data using " + readerAndWriter.GetType());
                if (flags.inputEncoding == null)
                {
                    Console.Error.WriteLine(@"Getting data from " + string_renamed + @" (default encoding)");
                }
                else
                {
                    Console.Error.WriteLine(@"Getting data from " + string_renamed + @" (" + flags.inputEncoding + @" encoding)");
                }
            }

            return new ObjectBankWrapper<IN>(flags, new ObjectBank<List<IN>>(new ResettableReaderIteratorFactory(string_renamed), readerAndWriter), knownLCWords);
        }

        public virtual ObjectBank<List<IN>> MakeObjectBankFromFile(string filename, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            String[] fileAsArray = new[] { filename };

            return MakeObjectBankFromFiles(fileAsArray, readerAndWriter);
        }

        public virtual ObjectBank<List<IN>> MakeObjectBankFromFiles(String[] trainFileList, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            ICollection<FileInfo> files = new List<FileInfo>();
            foreach (string trainFile in trainFileList)
            {
                FileInfo f = new FileInfo(trainFile);
                files.Add(f);
            }

            return new ObjectBankWrapper<IN>(flags, new ObjectBank<List<IN>>(new ResettableReaderIteratorFactory(files, flags.inputEncoding), readerAndWriter), knownLCWords);
        }

        public virtual ObjectBank<List<IN>> MakeObjectBankFromFiles(string baseDir, string filePattern, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            FileInfo path = new FileInfo(baseDir);
            FileFilter filter = new RegExFileFilter(Pattern.Compile(filePattern));
            FileInfo[] origFiles = path.ListFiles(filter);
            ICollection<FileInfo> files = new List<FileInfo>();
            foreach (FileInfo file in origFiles)
            {
                if (file.IsFile())
                {
                    if (flags.announceObjectBankEntries)
                    {
                        Console.Error.WriteLine(@"Getting data from " + file + @" (" + flags.inputEncoding + @" encoding)");
                    }

                    files.Add(file);
                }
            }

            if (files.IsEmpty())
            {
                throw new Exception(@"No matching files: " + baseDir + '\n' + filePattern);
            }

            return new ObjectBankWrapper<IN>(flags, new ObjectBank<List<IN>>(new ResettableReaderIteratorFactory(files, flags.inputEncoding), readerAndWriter), knownLCWords);
        }

        public virtual ObjectBank<List<IN>> MakeObjectBankFromFiles(ICollection<FileInfo> files, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            if (files.IsEmpty())
            {
                throw new Exception(@"Attempt to make ObjectBank with empty file list");
            }

            return new ObjectBankWrapper<IN>(flags, new ObjectBank<List<IN>>(new ResettableReaderIteratorFactory(files, flags.inputEncoding), readerAndWriter), knownLCWords);
        }

        public virtual ObjectBank<List<IN>> MakeObjectBankFromReader(BufferedReader in_renamed, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            if (flags.announceObjectBankEntries)
            {
                Console.Error.WriteLine(@"Reading data using " + readerAndWriter.GetType());
            }

            return new ObjectBankWrapper<IN>(flags, new ObjectBank<List<IN>>(new ResettableReaderIteratorFactory(in_renamed), readerAndWriter), knownLCWords);
        }

        public virtual void PrintProbs(string filename, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            flags.ocrTrain = false;
            ObjectBank<List<IN>> docs = MakeObjectBankFromFile(filename, readerAndWriter);
            PrintProbsDocuments(docs);
        }

        public virtual void PrintProbsDocuments(ObjectBank<List<IN>> documents)
        {
            foreach (List<IN> doc in documents)
            {
                PrintProbsDocument(doc);
                Console.Out.WriteLine();
            }
        }

        public virtual void ClassifyStdin()
        {
            ClassifyStdin(plainTextReaderAndWriter);
        }

        public virtual void ClassifyStdin(IDocumentReaderAndWriter<IN> readerWriter)
        {
            BufferedReader istream = new BufferedReader(new InputStreamReader(Console.In, flags.inputEncoding));
            for (string line; (line = istream.ReadLine()) != null; )
            {
                ICollection<List<IN>> documents = MakeObjectBankFromString(line, readerWriter);
                if (flags.keepEmptySentences && documents.Size() == 0)
                {
                    documents = Collections.SingletonList(Collections.EmptyList());
                }

                ClassifyAndWriteAnswers(documents, readerWriter);
            }
        }

        public abstract void PrintProbsDocument(List<IN> document);
        public virtual void ClassifyAndWriteAnswers(string testFile)
        {
            ClassifyAndWriteAnswers(testFile, plainTextReaderAndWriter);
        }

        public virtual void ClassifyAndWriteAnswers(string testFile, IDocumentReaderAndWriter<IN> readerWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFile(testFile, readerWriter);
            ClassifyAndWriteAnswers(documents, readerWriter);
        }

        public virtual void ClassifyAndWriteAnswers(string testFile, Stream outStream, IDocumentReaderAndWriter<IN> readerWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFile(testFile, readerWriter);
            TextWriter pw = IOUtils.EncodedOutputStreamPrintWriter(outStream, flags.outputEncoding, true);
            ClassifyAndWriteAnswers(documents, pw, readerWriter);
        }

        public virtual void ClassifyAndWriteAnswers(string baseDir, string filePattern, IDocumentReaderAndWriter<IN> readerWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFiles(baseDir, filePattern, readerWriter);
            ClassifyAndWriteAnswers(documents, readerWriter);
        }

        public virtual void ClassifyFilesAndWriteAnswers(ICollection<FileInfo> testFiles)
        {
            ClassifyFilesAndWriteAnswers(testFiles, plainTextReaderAndWriter);
        }

        public virtual void ClassifyFilesAndWriteAnswers(ICollection<FileInfo> testFiles, IDocumentReaderAndWriter<IN> readerWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFiles(testFiles, readerWriter);
            ClassifyAndWriteAnswers(documents, readerWriter);
        }

        private void ClassifyAndWriteAnswers(ICollection<List<IN>> documents, IDocumentReaderAndWriter<IN> readerWriter)
        {
            ClassifyAndWriteAnswers(documents, IOUtils.EncodedOutputStreamPrintWriter(Console.Out, flags.outputEncoding, true), readerWriter);
        }

        public virtual void ClassifyAndWriteAnswers(ICollection<List<IN>> documents, TextWriter printWriter, IDocumentReaderAndWriter<IN> readerWriter)
        {
            Timing timer = new Timing();
            Counter<String> entityTP = new ClassicCounter<String>();
            Counter<String> entityFP = new ClassicCounter<String>();
            Counter<String> entityFN = new ClassicCounter<String>();
            bool resultsCounted = true;
            int numWords = 0;
            int numDocs = 0;
            ThreadsafeProcessor<List<IN>, List<IN>> threadProcessor = new AnonymousThreadsafeProcessor(this);
            MulticoreWrapper<List<IN>, List<IN>> wrapper = null;
            if (flags.multiThreadClassifier != 0)
            {
                wrapper = new MulticoreWrapper<List<IN>, List<IN>>(flags.multiThreadClassifier, threadProcessor);
            }

            foreach (List<IN> doc in documents)
            {
                numWords += doc.Size();
                numDocs++;
                if (flags.multiThreadClassifier != 0)
                {
                    wrapper.Put(doc);
                    while (wrapper.Peek())
                    {
                        List<IN> results = wrapper.Poll();
                        WriteAnswers(results, printWriter, readerWriter);
                        resultsCounted = resultsCounted && CountResults(results, entityTP, entityFP, entityFN);
                    }
                }
                else
                {
                    List<IN> results = threadProcessor.Process(doc);
                    WriteAnswers(results, printWriter, readerWriter);
                    resultsCounted = resultsCounted && CountResults(results, entityTP, entityFP, entityFN);
                }
            }

            if (flags.multiThreadClassifier != 0)
            {
                wrapper.Join();
                while (wrapper.Peek())
                {
                    List<IN> results = wrapper.Poll();
                    WriteAnswers(results, printWriter, readerWriter);
                    resultsCounted = resultsCounted && CountResults(results, entityTP, entityFP, entityFN);
                }
            }

            long millis = timer.Stop();
            double wordspersec = numWords / (((double)millis) / 1000);
            NumberFormatInfo nf = new NumberFormatInfo { NumberDecimalDigits = 2 };
            Console.Error.WriteLine(StringUtils.GetShortClassName(this) + @" tagged " + numWords + @" words in " + numDocs + @" documents at " + nf.Format(wordspersec) + @" words per second.");
            if (resultsCounted)
            {
                PrintResults(entityTP, entityFP, entityFN);
            }
        }

        private sealed class AnonymousThreadsafeProcessor : ThreadsafeProcessor
        {
            public AnonymousThreadsafeProcessor(AbstractSequenceClassifier parent)
            {
                this.parent = parent;
            }

            private readonly AbstractSequenceClassifier parent;
            public override List<IN> Process(List<IN> doc)
            {
                doc = Classify(doc);
                int completedNo = threadCompletionCounter.IncrementAndGet();
                if (flags.verboseMode)
                    Console.Error.WriteLine(completedNo + @" examples completed");
                return doc;
            }

            public override ThreadsafeProcessor<List<IN>, List<IN>> NewInstance()
            {
                return this;
            }
        }

        public virtual void ClassifyAndWriteAnswersKBest(string testFile, int k, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            ObjectBank<List<IN>> documents = MakeObjectBankFromFile(testFile, readerAndWriter);
            PrintWriter pw = IOUtils.EncodedOutputStreamPrintWriter(Console.Out, flags.outputEncoding, true);
            ClassifyAndWriteAnswersKBest(documents, k, pw, readerAndWriter);
        }

        public virtual void ClassifyAndWriteAnswersKBest(ObjectBank<List<IN>> documents, int k, TextWriter printWriter, DocumentReaderAndWriter<IN> readerAndWriter)
        {
            Timing timer = new Timing();
            int numWords = 0;
            int numSentences = 0;
            foreach (List<IN> doc in documents)
            {
                Counter<List<IN>> kBest = ClassifyKBest(doc, typeof(CoreAnnotations.AnswerAnnotation), k);
                numWords += doc.Size();
                List<List<IN>> sorted = Counters.ToSortedList(kBest);
                int n = 1;
                foreach (List<IN> l in sorted)
                {
                    Console.Out.WriteLine(@"<sentence id=" + numSentences + @" k=" + n + @" logProb=" + kBest.GetCount(l) + @" prob=" + Math.Exp(kBest.GetCount(l)) + '>');
                    WriteAnswers(l, printWriter, readerAndWriter);
                    Console.Out.WriteLine(@"</sentence>");
                    n++;
                }

                numSentences++;
            }

            long millis = timer.Stop();
            double wordspersec = numWords / (((double)millis) / 1000);
            NumberFormat nf = new DecimalFormat(@"0.00");
            Console.Error.WriteLine(this.GetType().GetName() + @" tagged " + numWords + @" words in " + numSentences + @" documents at " + nf.Format(wordspersec) + @" words per second.");
        }

        public virtual void ClassifyAndWriteViterbiSearchGraph(string testFile, string searchGraphPrefix, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            Timing timer = new Timing();
            ObjectBank<List<IN>> documents = MakeObjectBankFromFile(testFile, readerAndWriter);
            int numWords = 0;
            int numSentences = 0;
            foreach (List<IN> doc in documents)
            {
                DFSA<String, Integer> tagLattice = GetViterbiSearchGraph(doc, typeof(CoreAnnotations.AnswerAnnotation));
                numWords += doc.Size();
                PrintWriter latticeWriter = new PrintWriter(new FileOutputStream(searchGraphPrefix + '.' + numSentences + @".wlattice"));
                PrintWriter vsgWriter = new PrintWriter(new FileOutputStream(searchGraphPrefix + '.' + numSentences + @".lattice"));
                if (readerAndWriter is LatticeWriter)
                    ((LatticeWriter<IN, String, Integer>)readerAndWriter).PrintLattice(tagLattice, doc, latticeWriter);
                tagLattice.PrintAttFsmFormat(vsgWriter);
                latticeWriter.Close();
                vsgWriter.Close();
                numSentences++;
            }

            long millis = timer.Stop();
            double wordspersec = numWords / (((double)millis) / 1000);
            NumberFormat nf = new DecimalFormat(@"0.00");
            Console.Error.WriteLine(this.GetType().GetName() + @" tagged " + numWords + @" words in " + numSentences + @" documents at " + nf.Format(wordspersec) + @" words per second.");
        }

        public virtual void WriteAnswers(IList<IN> doc, TextWriter printWriter, IDocumentReaderAndWriter<IN> readerAndWriter)
        {
            if (flags.lowerNewgeneThreshold)
            {
                return;
            }

            if (flags.numRuns <= 1)
            {
                readerAndWriter.PrintAnswers(doc, printWriter);
                printWriter.Flush();
            }
        }

        public virtual bool CountResults(IList<IN> doc, Counter<String> entityTP, Counter<String> entityFP, Counter<String> entityFN)
        {
            string bg = (flags.evaluateBackground ? null : flags.backgroundSymbol);
            if (flags.entitySubclassification.EqualsIgnoreCase(@"iob2"))
            {
                bg = flags.backgroundSymbol;
                return CountResultsIOB2(doc, entityTP, entityFP, entityFN, bg);
            }
            else if (flags.iobTags)
            {
                bg = flags.backgroundSymbol;
                return CountResultsIOB(doc, entityTP, entityFP, entityFN, bg);
            }
            else
            {
                return CountResults(doc, entityTP, entityFP, entityFN, bg);
            }
        }

        public static bool CountResultsIOB2(List<ICoreMap> doc, Counter<String> entityTP, Counter<String> entityFP, Counter<String> entityFN, string background)
        {
            bool entityCorrect = true;
            string previousGold = background;
            string previousGuess = background;
            string previousGoldEntity = @"";
            string previousGuessEntity = @"";
            foreach (CoreMap word in doc)
            {
                string gold = word.Get(typeof(CoreAnnotations.GoldAnswerAnnotation));
                string guess = word.Get(typeof(CoreAnnotations.AnswerAnnotation));
                string goldEntity = (!gold.Equals(background)) ? gold.Substring(2) : @"";
                string guessEntity = (!guess.Equals(background)) ? guess.Substring(2) : @"";
                bool newGold = (!gold.Equals(background) && (!goldEntity.Equals(previousGoldEntity)) || gold.StartsWith(@"B-"));
                bool newGuess = (!guess.Equals(background) && (!guessEntity.Equals(previousGuessEntity)) || guess.StartsWith(@"B-"));
                bool goldEnded = (!previousGold.Equals(background) && (gold.StartsWith(@"B-") || !goldEntity.Equals(previousGoldEntity)));
                bool guessEnded = (!previousGuess.Equals(background) && (guess.StartsWith(@"B-") || !guessEntity.Equals(previousGuessEntity)));
                if (goldEnded && !guessEnded)
                {
                    entityFN.IncrementCount(previousGoldEntity, 1.0);
                    entityCorrect = gold.Equals(background) && guess.Equals(background);
                }

                if (goldEnded && guessEnded)
                {
                    if (entityCorrect)
                    {
                        entityTP.IncrementCount(previousGoldEntity, 1.0);
                    }
                    else
                    {
                        entityFN.IncrementCount(previousGoldEntity, 1.0);
                        entityFP.IncrementCount(previousGuessEntity, 1.0);
                    }

                    entityCorrect = gold.Equals(guess);
                }

                if (!goldEnded && guessEnded)
                {
                    entityCorrect = false;
                    entityFP.IncrementCount(previousGuessEntity, 1.0);
                }

                if (newGold && !newGuess)
                {
                    entityCorrect = false;
                }

                if (newGold && newGuess)
                {
                    entityCorrect = guessEntity.Equals(goldEntity);
                }

                if (!newGold && newGuess)
                {
                    entityCorrect = false;
                }

                previousGold = gold;
                previousGuess = guess;
                previousGoldEntity = goldEntity;
                previousGuessEntity = guessEntity;
            }

            if (!previousGold.Equals(background))
            {
                if (entityCorrect)
                {
                    entityTP.IncrementCount(previousGoldEntity, 1.0);
                }
                else
                {
                    entityFN.IncrementCount(previousGoldEntity, 1.0);
                }
            }

            if (!previousGuess.Equals(background))
            {
                if (!entityCorrect)
                {
                    entityFP.IncrementCount(previousGuessEntity, 1.0);
                }
            }

            return true;
        }

        public static bool CountResultsIOB(List<ICoreMap> doc, Counter<String> entityTP, Counter<String> entityFP, Counter<String> entityFN, string background)
        {
            foreach (CoreMap line in doc)
            {
                string gold = line.Get(typeof(CoreAnnotations.GoldAnswerAnnotation));
                string guess = line.Get(typeof(CoreAnnotations.AnswerAnnotation));
                if (gold == null)
                {
                    Console.Error.WriteLine(@"Blank gold answer");
                    return false;
                }

                if (guess == null)
                {
                    Console.Error.WriteLine(@"Blank guess");
                    return false;
                }

                if (!gold.Equals(background) && !gold.StartsWith(@"B-") && !gold.StartsWith(@"I-"))
                {
                    Console.Error.WriteLine(@"Unexpected gold answer " + gold);
                    return false;
                }

                if (!guess.Equals(background) && !guess.StartsWith(@"B-") && !guess.StartsWith(@"I-"))
                {
                    Console.Error.WriteLine(@"Unexpected guess " + guess);
                    return false;
                }
            }

            int index = 0;
            while (index < doc.Size())
            {
                index = TallyOneEntityIOB(doc, index, typeof(CoreAnnotations.GoldAnswerAnnotation), typeof(CoreAnnotations.AnswerAnnotation), entityTP, entityFN, background);
            }

            index = 0;
            while (index < doc.Size())
            {
                index = TallyOneEntityIOB(doc, index, typeof(CoreAnnotations.AnswerAnnotation), typeof(CoreAnnotations.GoldAnswerAnnotation), null, entityFP, background);
            }

            return true;
        }

        public static int TallyOneEntityIOB(List<ICoreMap> doc, int index, Type source, Type target, Counter<String> positive, Counter<String> negative, string background)
        {
            CoreMap line = doc.Get(index);
            string gold = line.Get(source);
            string guess = line.Get(target);
            if (gold.Equals(background))
            {
                return index + 1;
            }

            string entity = gold.Substring(2);
            bool correct = gold.Equals(guess);
            ++index;
            while (index < doc.Size())
            {
                line = doc.Get(index);
                gold = line.Get(source);
                guess = line.Get(target);
                if (!gold.Equals(@"I-" + entity))
                {
                    if (guess.Equals(@"I-" + entity))
                    {
                        correct = false;
                    }

                    break;
                }

                if (!gold.Equals(guess))
                {
                    correct = false;
                }

                ++index;
            }

            if (correct)
            {
                if (positive != null)
                {
                    positive.IncrementCount(entity, 1.0);
                }
            }
            else
            {
                negative.IncrementCount(entity, 1.0);
            }

            return index;
        }

        public static bool CountResults(List<ICoreMap> doc, Counter<String> entityTP, Counter<String> entityFP, Counter<String> entityFN, string background)
        {
            int index = 0;
            int goldIndex = 0, guessIndex = 0;
            string lastGold = background, lastGuess = background;
            foreach (CoreMap line in doc)
            {
                string gold = line.Get(typeof(CoreAnnotations.GoldAnswerAnnotation));
                string guess = line.Get(typeof(CoreAnnotations.AnswerAnnotation));
                if (gold == null || guess == null)
                    return false;
                if (lastGold != null && !lastGold.Equals(gold) && !lastGold.Equals(background))
                {
                    if (lastGuess.Equals(lastGold) && !lastGuess.Equals(guess) && goldIndex == guessIndex)
                    {
                        entityTP.IncrementCount(lastGold, 1.0);
                    }
                    else
                    {
                        entityFN.IncrementCount(lastGold, 1.0);
                    }
                }

                if (lastGuess != null && !lastGuess.Equals(guess) && !lastGuess.Equals(background))
                {
                    if (lastGuess.Equals(lastGold) && !lastGuess.Equals(guess) && goldIndex == guessIndex && !lastGold.Equals(gold))
                    {
                    }
                    else
                    {
                        entityFP.IncrementCount(lastGuess, 1.0);
                    }
                }

                if (lastGold == null || !lastGold.Equals(gold))
                {
                    lastGold = gold;
                    goldIndex = index;
                }

                if (lastGuess == null || !lastGuess.Equals(guess))
                {
                    lastGuess = guess;
                    guessIndex = index;
                }

                ++index;
            }

            if (lastGold != null && !lastGold.Equals(background))
            {
                if (lastGold.Equals(lastGuess) && goldIndex == guessIndex)
                {
                    entityTP.IncrementCount(lastGold, 1.0);
                }
                else
                {
                    entityFN.IncrementCount(lastGold, 1.0);
                }
            }

            if (lastGuess != null && !lastGuess.Equals(background))
            {
                if (lastGold.Equals(lastGuess) && goldIndex == guessIndex)
                {
                }
                else
                {
                    entityFP.IncrementCount(lastGuess, 1.0);
                }
            }

            return true;
        }

        public static void PrintResults(Counter<String> entityTP, Counter<String> entityFP, Counter<String> entityFN)
        {
            Set<String> entities = new TreeSet<String>();
            entities.AddAll(entityTP.KeySet());
            entities.AddAll(entityFP.KeySet());
            entities.AddAll(entityFN.KeySet());
            bool printedHeader = false;
            foreach (string entity in entities)
            {
                double tp = entityTP.GetCount(entity);
                double fp = entityFP.GetCount(entity);
                double fn = entityFN.GetCount(entity);
                printedHeader = PrintPRLine(entity, tp, fp, fn, printedHeader);
            }

            double tp = entityTP.TotalCount();
            double fp = entityFP.TotalCount();
            double fn = entityFN.TotalCount();
            printedHeader = PrintPRLine(@"Totals", tp, fp, fn, printedHeader);
        }

        private static bool PrintPRLine(string entity, double tp, double fp, double fn, bool printedHeader)
        {
            if (tp == 0.0 && (fp == 0.0 || fn == 0.0))
                return printedHeader;
            double precision = tp / (tp + fp);
            double recall = tp / (tp + fn);
            double f1 = ((precision == 0.0 || recall == 0.0) ? 0.0 : 2.0 / (1.0 / precision + 1.0 / recall));
            if (!printedHeader)
            {
                Console.Error.WriteLine(@"         Entity\tP\tR\tF1\tTP\tFP\tFN");
                printedHeader = true;
            }

            System.err.Format(@"%15s\t%.4f\t%.4f\t%.4f\t%.0f\t%.0f\t%.0f\n", entity, precision, recall, f1, tp, fp, fn);
            return printedHeader;
        }

        public abstract void SerializeClassifier(string serializePath);
        public virtual void LoadClassifierNoExceptions(InputStream in_renamed, Properties props)
        {
            try
            {
                LoadClassifier(in_renamed, props);
            }
            catch (IOException e)
            {
                throw new RuntimeIOException(e);
            }
            catch (ClassNotFoundException cnfe)
            {
                throw new Exception(cnfe);
            }
        }

        public virtual void LoadClassifier(InputStream in_renamed)
        {
            LoadClassifier(in_renamed, null);
        }

        public virtual void LoadClassifier(InputStream in_renamed, Properties props)
        {
            LoadClassifier(new ObjectInputStream(in_renamed), props);
        }

        public abstract void LoadClassifier(ObjectInputStream in_renamed, Properties props);
        private InputStream LoadStreamFromClasspath(string path)
        {
            InputStream istream = GetType().GetClassLoader().GetResourceAsStream(path);
            if (istream == null)
                return null;
            try
            {
                if (path.EndsWith(@".gz"))
                    istream = new GZIPInputStream(new BufferedInputStream(istream));
                else
                    istream = new BufferedInputStream(istream);
            }
            catch (IOException e)
            {
                Console.Error.WriteLine(@"CLASSPATH resource " + path + @" is not a GZIP stream!");
            }

            return istream;
        }

        public virtual void LoadClassifier(string loadPath)
        {
            LoadClassifier(loadPath, null);
        }

        public virtual void LoadClassifier(string loadPath, Properties props)
        {
            InputStream istream;
            if ((istream = LoadStreamFromClasspath(loadPath)) != null)
            {
                Timing.StartDoing(@"Loading classifier from " + loadPath);
                LoadClassifier(istream, props);
                istream.Close();
                Timing.EndDoing();
            }
            else
            {
                LoadClassifier(new FileInfo(loadPath), props);
            }
        }

        public virtual void LoadClassifierNoExceptions(string loadPath)
        {
            LoadClassifierNoExceptions(loadPath, null);
        }

        public virtual void LoadClassifierNoExceptions(string loadPath, Properties props)
        {
            InputStream istream;
            if ((istream = LoadStreamFromClasspath(loadPath)) != null)
            {
                Timing.StartDoing(@"Loading classifier from " + loadPath);
                LoadClassifierNoExceptions(istream, props);
                try
                {
                    istream.Close();
                }
                catch (IOException e)
                {
                    throw new Exception(e);
                }

                Timing.EndDoing();
            }
            else
            {
                LoadClassifierNoExceptions(new FileInfo(loadPath), props);
            }
        }

        public virtual void LoadClassifier(FileInfo file)
        {
            LoadClassifier(file, null);
        }

        public virtual void LoadClassifier(FileInfo file, Properties props)
        {
            Timing.StartDoing(@"Loading classifier from " + file.GetAbsolutePath());
            BufferedInputStream bis;
            if (file.GetName().EndsWith(@".gz"))
            {
                bis = new BufferedInputStream(new GZIPInputStream(new FileInputStream(file)));
            }
            else
            {
                bis = new BufferedInputStream(new FileInputStream(file));
            }

            LoadClassifier(bis, props);
            bis.Close();
            Timing.EndDoing();
        }

        public virtual void LoadClassifierNoExceptions(FileInfo file)
        {
            LoadClassifierNoExceptions(file, null);
        }

        public virtual void LoadClassifierNoExceptions(FileInfo file, Properties props)
        {
            try
            {
                LoadClassifier(file, props);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Error deserializing " + file.GetAbsolutePath());
                throw new Exception(e);
            }
        }

        public virtual void LoadJarClassifier(string modelName, Properties props)
        {
            Timing.StartDoing(@"Loading JAR-internal classifier " + modelName);
            try
            {
                InputStream istream = GetType().GetResourceAsStream(modelName);
                if (modelName.EndsWith(@".gz"))
                {
                    istream = new GZIPInputStream(istream);
                }

                istream = new BufferedInputStream(istream);
                LoadClassifier(istream, props);
                istream.Close();
                Timing.EndDoing();
            }
            catch (Exception e)
            {
                string msg = @"Error loading classifier from jar file (most likely you are not running this code from a jar file or the named classifier is not stored in the jar file)";
                throw new Exception(msg, e);
            }
        }

        private TextWriter cliqueWriter;
        private int writtenNum;
        protected virtual void PrintFeatures(IN wi, ICollection<String> features)
        {
            if (flags.printFeatures == null || writtenNum >= flags.printFeaturesUpto)
            {
                return;
            }

            if (cliqueWriter == null)
            {
                cliqueWriter = IOUtils.GetPrintWriterOrDie(@"feats-" + flags.printFeatures + @".txt");
                writtenNum = 0;
            }

            if (wi is CoreLabel)
            {
                cliqueWriter.Print(wi.Get(typeof(CoreAnnotations.TextAnnotation)) + ' ' + wi.Get(typeof(CoreAnnotations.PartOfSpeechAnnotation)) + ' ' + wi.Get(typeof(CoreAnnotations.GoldAnswerAnnotation)) + '\n');
            }
            else
            {
                cliqueWriter.Print(wi.Get(typeof(CoreAnnotations.TextAnnotation)) + wi.Get(typeof(CoreAnnotations.GoldAnswerAnnotation)) + '\n');
            }

            bool first = true;
            List<String> featsList = new List<String>(features);
            Collections.Sort(featsList);
            foreach (string feat in featsList)
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    cliqueWriter.Print(@" ");
                }

                cliqueWriter.Print(feat);
            }

            cliqueWriter.Println();
            writtenNum++;
        }

        protected virtual void PrintFeatureLists(IN wi, Collection<List<String>> features)
        {
            if (flags.printFeatures == null || writtenNum >= flags.printFeaturesUpto)
            {
                return;
            }

            if (cliqueWriter == null)
            {
                cliqueWriter = IOUtils.GetPrintWriterOrDie(@"feats-" + flags.printFeatures + @".txt");
                writtenNum = 0;
            }

            if (wi is CoreLabel)
            {
                cliqueWriter.Print(wi.Get(typeof(CoreAnnotations.TextAnnotation)) + ' ' + wi.Get(typeof(CoreAnnotations.PartOfSpeechAnnotation)) + ' ' + wi.Get(typeof(CoreAnnotations.GoldAnswerAnnotation)) + '\n');
            }
            else
            {
                cliqueWriter.Print(wi.Get(typeof(CoreAnnotations.TextAnnotation)) + wi.Get(typeof(CoreAnnotations.GoldAnswerAnnotation)) + '\n');
            }

            bool first = true;
            foreach (List<String> featList in features)
            {
                List<String> sortedFeatList = new List<String>(featList);
                Collections.Sort(sortedFeatList);
                foreach (string feat in sortedFeatList)
                {
                    if (first)
                    {
                        first = false;
                    }
                    else
                    {
                        cliqueWriter.Print(@" ");
                    }

                    cliqueWriter.Print(feat);
                }

                cliqueWriter.Print(@"  ");
            }

            cliqueWriter.Println();
            writtenNum++;
        }

        public virtual int WindowSize()
        {
            return windowSize;
        }
    }
}
