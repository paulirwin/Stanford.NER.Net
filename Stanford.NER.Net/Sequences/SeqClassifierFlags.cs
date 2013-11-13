using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public class SeqClassifierFlags
    {
        private static readonly long serialVersionUID = -7076671761070232567L;
        public static readonly string DEFAULT_BACKGROUND_SYMBOL = "O";
        private string stringRep = "";
        public bool useNGrams = false;
        public bool conjoinShapeNGrams = false;
        public bool lowercaseNGrams = false;
        public bool dehyphenateNGrams = false;
        public bool usePrev = false;
        public bool useNext = false;
        public bool useTags = false;
        public bool useWordPairs = false;
        public bool useGazettes = false;
        public bool useSequences = true;
        public bool usePrevSequences = false;
        public bool useNextSequences = false;
        public bool useLongSequences = false;
        public bool useBoundarySequences = false;
        public bool useTaggySequences = false;
        public bool useExtraTaggySequences = false;
        public bool dontExtendTaggy = false;
        public bool useTaggySequencesShapeInteraction = false;
        public bool strictlyZeroethOrder = false;
        public bool strictlyFirstOrder = false;
        public bool strictlySecondOrder = false;
        public bool strictlyThirdOrder = false;
        public string entitySubclassification = "IO";
        public bool retainEntitySubclassification = false;
        public bool useGazettePhrases = false;
        public bool makeConsistent = false;
        public bool useWordLabelCounts = false;
        public bool useViterbi = true;
        public int[] binnedLengths = null;
        public bool verboseMode = false;
        public bool useSum = false;
        public double tolerance = 0.0;
        public string printFeatures = null;
        public bool useSymTags = false;
        public bool useSymWordPairs = false;
        public string printClassifier = "WeightHistogram";
        public int printClassifierParam = 100;
        public bool intern = false;
        public bool intern2 = false;
        public bool selfTest = false;
        public bool sloppyGazette = false;
        public bool cleanGazette = false;
        public bool noMidNGrams = false;
        public int maxNGramLeng = -1;
        public bool useReverse = false;
        public bool greekifyNGrams = false;
        public bool useParenMatching = false;
        public bool useLemmas = false;
        public bool usePrevNextLemmas = false;
        public bool normalizeTerms = false;
        public bool normalizeTimex = false;
        public bool useNB = false;
        public bool useQN = true;
        public bool useFloat = false;
        public int QNsize = 25;
        public int QNsize2 = 25;
        public int maxIterations = -1;
        public int wordShape = WordShapeClassifier.NOWORDSHAPE;
        public bool useShapeStrings = false;
        public bool useTypeSeqs = false;
        public bool useTypeSeqs2 = false;
        public bool useTypeSeqs3 = false;
        public bool useDisjunctive = false;
        public int disjunctionWidth = 4;
        public bool useDisjunctiveShapeInteraction = false;
        public bool useDisjShape = false;
        public bool useWord = true;
        public bool useClassFeature = false;
        public bool useShapeConjunctions = false;
        public bool useWordTag = false;
        public bool useNPHead = false;
        public bool useNPGovernor = false;
        public bool useHeadGov = false;
        public bool useLastRealWord = false;
        public bool useNextRealWord = false;
        public bool useOccurrencePatterns = false;
        public bool useTypeySequences = false;
        public bool justify = false;
        public bool normalize = false;
        public string priorType = @"QUADRATIC";
        public double sigma = 1.0;
        public double epsilon = 0.0;
        public int beamSize = 30;
        public int maxLeft = 2;
        public int maxRight = 0;
        public bool usePosition = false;
        public bool useBeginSent = false;
        public bool useGazFeatures = false;
        public bool useMoreGazFeatures = false;
        public bool useAbbr = false;
        public bool useMinimalAbbr = false;
        public bool useAbbr1 = false;
        public bool useMinimalAbbr1 = false;
        public bool useMoreAbbr = false;
        public bool deleteBlankLines = false;
        public bool useGENIA = false;
        public bool useTOK = false;
        public bool useABSTR = false;
        public bool useABSTRFreqDict = false;
        public bool useABSTRFreq = false;
        public bool useFREQ = false;
        public bool useABGENE = false;
        public bool useWEB = false;
        public bool useWEBFreqDict = false;
        public bool useIsURL = false;
        public bool useURLSequences = false;
        public bool useIsDateRange = false;
        public bool useEntityTypes = false;
        public bool useEntityTypeSequences = false;
        public bool useEntityRule = false;
        public bool useOrdinal = false;
        public bool useACR = false;
        public bool useANTE = false;
        public bool useMoreTags = false;
        public bool useChunks = false;
        public bool useChunkySequences = false;
        public bool usePrevVB = false;
        public bool useNextVB = false;
        public bool useVB = false;
        public bool subCWGaz = false;
        public string documentReader = @"ColumnDocumentReader";
        public string map = @"word=0,tag=1,answer=2";
        public bool useWideDisjunctive = false;
        public int wideDisjunctionWidth = 10;
        public bool useRadical = false;
        public bool useBigramInTwoClique = false;
        public string morphFeatureFile = null;
        public bool useReverseAffix = false;
        public int charHalfWindow = 3;
        public bool useWord1 = false;
        public bool useWord2 = false;
        public bool useWord3 = false;
        public bool useWord4 = false;
        public bool useRad1 = false;
        public bool useRad2 = false;
        public bool useWordn = false;
        public bool useCTBPre1 = false;
        public bool useCTBSuf1 = false;
        public bool useASBCPre1 = false;
        public bool useASBCSuf1 = false;
        public bool usePKPre1 = false;
        public bool usePKSuf1 = false;
        public bool useHKPre1 = false;
        public bool useHKSuf1 = false;
        public bool useCTBChar2 = false;
        public bool useASBCChar2 = false;
        public bool useHKChar2 = false;
        public bool usePKChar2 = false;
        public bool useRule2 = false;
        public bool useDict2 = false;
        public bool useOutDict2 = false;
        public string outDict2 = @"/u/htseng/scr/chunking/segmentation/out.lexicon";
        public bool useDictleng = false;
        public bool useDictCTB2 = false;
        public bool useDictASBC2 = false;
        public bool useDictPK2 = false;
        public bool useDictHK2 = false;
        public bool useBig5 = false;
        public bool useNegDict2 = false;
        public bool useNegDict3 = false;
        public bool useNegDict4 = false;
        public bool useNegCTBDict2 = false;
        public bool useNegCTBDict3 = false;
        public bool useNegCTBDict4 = false;
        public bool useNegASBCDict2 = false;
        public bool useNegASBCDict3 = false;
        public bool useNegASBCDict4 = false;
        public bool useNegHKDict2 = false;
        public bool useNegHKDict3 = false;
        public bool useNegHKDict4 = false;
        public bool useNegPKDict2 = false;
        public bool useNegPKDict3 = false;
        public bool useNegPKDict4 = false;
        public bool usePre = false;
        public bool useSuf = false;
        public bool useRule = false;
        public bool useHk = false;
        public bool useMsr = false;
        public bool useMSRChar2 = false;
        public bool usePk = false;
        public bool useAs = false;
        public bool useFilter = false;
        public bool largeChSegFile = false;
        public bool useRad2b = false;
        public bool keepEnglishWhitespaces = false;
        public bool keepAllWhitespaces = false;
        public bool sighanPostProcessing = false;
        public bool useChPos = false;
        public string normalizationTable;
        public string dictionary;
        public string serializedDictionary;
        public string dictionary2;
        public string normTableEncoding = @"GB18030";
        public string sighanCorporaDict = @"/u/nlp/data/chinese-segmenter/";
        public bool useWordShapeGaz = false;
        public string wordShapeGaz = null;
        public bool splitDocuments = true;
        public bool printXML = false;
        public bool useSeenFeaturesOnly = false;
        public string lastNameList = @"/u/nlp/data/dist.all.last";
        public string maleNameList = @"/u/nlp/data/dist.male.first";
        public string femaleNameList = @"/u/nlp/data/dist.female.first";
        public string trainFile = null;
        public string adaptFile = null;
        public string devFile = null;
        public string testFile = null;
        public string textFile = null;
        public string textFiles = null;
        public bool readStdin = false;
        public string outputFile = null;
        public string loadClassifier = null;
        public string loadTextClassifier = null;
        public string loadJarClassifier = null;
        public string loadAuxClassifier = null;
        public string serializeTo = null;
        public string serializeToText = null;
        public int interimOutputFreq = 0;
        public string initialWeights = null;
        public IList<String> gazettes = new List<String>();
        public string selfTrainFile = null;
        public string inputEncoding = @"UTF-8";
        public bool bioSubmitOutput = false;
        public int numRuns = 1;
        public string answerFile = null;
        public string altAnswerFile = null;
        public string dropGaz;
        public string printGazFeatures = null;
        public int numStartLayers = 1;
        public bool dump = false;
        public bool mergeTags;
        public bool splitOnHead;
        public int featureCountThreshold = 0;
        public double featureWeightThreshold = 0.0;
        public string featureFactory = @"edu.stanford.nlp.ie.NERFeatureFactory";
        public Object[] featureFactoryArgs = new Object[0];
        public string backgroundSymbol = DEFAULT_BACKGROUND_SYMBOL;
        public bool useObservedSequencesOnly = false;
        public int maxDocSize = 0;
        public bool printProbs = false;
        public bool printFirstOrderProbs = false;
        public bool saveFeatureIndexToDisk = false;
        public bool removeBackgroundSingletonFeatures = false;
        public bool doGibbs = false;
        public int numSamples = 100;
        public bool useNERPrior = false;
        public bool useAcqPrior = false;
        public bool useUniformPrior = false;
        public bool useMUCFeatures = false;
        public double annealingRate = 0.0;
        public string annealingType = null;
        public string loadProcessedData = null;
        public bool initViterbi = true;
        public bool useUnknown = false;
        public bool checkNameList = false;
        public bool useSemPrior = false;
        public bool useFirstWord = false;
        public bool useNumberFeature = false;
        public int ocrFold = 0;
        public bool ocrTrain = false;
        public string classifierType = @"MaxEnt";
        public string svmModelFile = null;
        public string inferenceType = @"Viterbi";
        public bool useLemmaAsWord = false;
        public string type = @"cmm";
        public string readerAndWriter = @"edu.stanford.nlp.sequences.ColumnDocumentReaderAndWriter";
        public IList<String> comboProps = new List<String>();
        public bool usePrediction = false;
        public bool useAltGazFeatures = false;
        public string gazFilesFile = null;
        public bool usePrediction2 = false;
        public string baseTrainDir = @".";
        public string baseTestDir = @".";
        public string trainFiles = null;
        public string trainFileList = null;
        public string testFiles = null;
        public string trainDirs = null;
        public string testDirs = null;
        public bool useOnlySeenWeights = false;
        public string predProp = null;
        public CoreLabel pad = new CoreLabel();
        public bool useObservedFeaturesOnly = false;
        public string distSimLexicon = null;
        public bool useDistSim = false;
        public int removeTopN = 0;
        public int numTimesRemoveTopN = 1;
        public double randomizedRatio = 1.0;
        public double removeTopNPercent = 0.0;
        public int purgeFeatures = -1;
        public bool booleanFeatures = false;
        public bool iobWrapper = false;
        public bool iobTags = false;
        public bool useSegmentation = false;
        public bool memoryThrift = false;
        public bool timitDatum = false;
        public string serializeDatasetsDir = null;
        public string loadDatasetsDir = null;
        public string pushDir = null;
        public bool purgeDatasets = false;
        public bool keepOBInMemory = true;
        public bool fakeDataset = false;
        public bool restrictTransitionsTimit = false;
        public int numDatasetsPerFile = 1;
        public bool useTitle = false;
        public bool lowerNewgeneThreshold = false;
        public bool useEitherSideWord = false;
        public bool useEitherSideDisjunctive = false;
        public bool twoStage = false;
        public string crfType = @"MaxEnt";
        public int featureThreshold = 1;
        public string featThreshFile = null;
        public double featureDiffThresh = 0.0;
        public int numTimesPruneFeatures = 0;
        public double newgeneThreshold = 0.0;
        public bool doAdaptation = false;
        public bool useInternal = true;
        public bool useExternal = true;
        public double selfTrainConfidenceThreshold = 0.9;
        public int selfTrainIterations = 1;
        public int selfTrainWindowSize = 1;
        public bool useHuber = false;
        public bool useQuartic = false;
        public double adaptSigma = 1.0;
        public int numFolds = 1;
        public int startFold = 1;
        public int endFold = 1;
        public bool cacheNGrams = false;
        public string outputFormat;
        public bool useSMD = false;
        public bool useSGDtoQN = false;
        public bool useStochasticQN = false;
        public bool useScaledSGD = false;
        public int scaledSGDMethod = 0;
        public int SGDPasses = -1;
        public int QNPasses = -1;
        public bool tuneSGD = false;
        public StochasticCalculateMethods stochasticMethod = StochasticCalculateMethods.NoneSpecified;
        public double initialGain = 0.1;
        public int stochasticBatchSize = 15;
        public bool useSGD = false;
        public double gainSGD = 0.1;
        public bool useHybrid = false;
        public int hybridCutoffIteration = 0;
        public bool outputIterationsToFile = false;
        public bool testObjFunction = false;
        public bool testVariance = false;
        public int SGD2QNhessSamples = 50;
        public bool testHessSamples = false;
        public int CRForder = 1;
        public int CRFwindow = 2;
        public bool estimateInitial = false;
        public string biasedTrainFile = null;
        public string confusionMatrix = null;
        public string outputEncoding = null;
        public bool useKBest = false;
        public string searchGraphPrefix = null;
        public double searchGraphPrune = Double.PositiveInfinity;
        public int kBest = 1;
        public bool useFeaturesC4gram;
        public bool useFeaturesC5gram;
        public bool useFeaturesC6gram;
        public bool useFeaturesCpC4gram;
        public bool useFeaturesCpC5gram;
        public bool useFeaturesCpC6gram;
        public bool useUnicodeType;
        public bool useUnicodeType4gram;
        public bool useUnicodeType5gram;
        public bool use4Clique;
        public bool useUnicodeBlock;
        public bool useShapeStrings1;
        public bool useShapeStrings3;
        public bool useShapeStrings4;
        public bool useShapeStrings5;
        public bool useGoodForNamesCpC;
        public bool useDictionaryConjunctions;
        public bool expandMidDot;
        public int printFeaturesUpto;
        public bool useDictionaryConjunctions3;
        public bool useWordUTypeConjunctions2;
        public bool useWordUTypeConjunctions3;
        public bool useWordShapeConjunctions2;
        public bool useWordShapeConjunctions3;
        public bool useMidDotShape;
        public bool augmentedDateChars;
        public bool suppressMidDotPostprocessing;
        public bool printNR;
        public string classBias = null;
        public bool printLabelValue;
        public bool useRobustQN = false;
        public bool combo = false;
        public bool useGenericFeatures = false;
        public bool verboseForTrueCasing = false;
        public string trainHierarchical = null;
        public string domain = null;
        public bool baseline = false;
        public string transferSigmas = null;
        public bool doFE = false;
        public bool restrictLabels = true;
        public bool announceObjectBankEntries = false;
        public bool usePos = false;
        public bool useAgreement = false;
        public bool useAccCase = false;
        public bool useInna = false;
        public bool useConcord = false;
        public bool useFirstNgram = false;
        public bool useLastNgram = false;
        public bool collapseNN = false;
        public bool useConjBreak = false;
        public bool useAuxPairs = false;
        public bool usePPVBPairs = false;
        public bool useAnnexing = false;
        public bool useTemporalNN = false;
        public bool usePath = false;
        public bool innaPPAttach = false;
        public bool markProperNN = false;
        public bool markMasdar = false;
        public bool useSVO = false;
        public int numTags = 3;
        public bool useTagsCpC = false;
        public bool useTagsCpCp2C = false;
        public bool useTagsCpCp2Cp3C = false;
        public bool useTagsCpCp2Cp3Cp4C = false;
        public double l1reg = 0.0;
        public string mixedCaseMapFile = @"";
        public string auxTrueCaseModels = @"";
        public bool use2W = false;
        public bool useLC = false;
        public bool useYetMoreCpCShapes = false;
        public bool useIfInteger = false;
        public string exportFeatures = null;
        public bool useInPlaceSGD = false;
        public bool useTopics = false;
        public int evaluateIters = 0;
        public string evalCmd = @"";
        public bool evaluateTrain = false;
        public int tuneSampleSize = -1;
        public bool usePhraseFeatures = false;
        public bool usePhraseWords = false;
        public bool usePhraseWordTags = false;
        public bool usePhraseWordSpecialTags = false;
        public bool useCommonWordsFeature = false;
        public bool useProtoFeatures = false;
        public bool useWordnetFeatures = false;
        public string tokenFactory = @"edu.stanford.nlp.process.CoreLabelTokenFactory";
        public Object[] tokenFactoryArgs = new Object[0];
        public string tokensAnnotationClassName = @"edu.stanford.nlp.ling.CoreAnnotations$TokensAnnotation";
        public string tokenizerOptions = null;
        public string tokenizerFactory = null;
        public bool useCorefFeatures = false;
        public string wikiFeatureDbFile = null;
        public bool useNoisyNonNoisyFeature = false;
        public bool useYear = false;
        public bool useSentenceNumber = false;
        public bool useLabelSource = false;
        public bool casedDistSim = false;
        public string distSimFileFormat = @"alexClark";
        public int distSimMaxBits = 8;
        public bool numberEquivalenceDistSim = false;
        public string unknownWordDistSimClass = @"null";
        public bool useNeighborNGrams = false;
        public IFunction<String, String> wordFunction = null;
        public static readonly string DEFAULT_PLAIN_TEXT_READER = @"edu.stanford.nlp.sequences.PlainTextDocumentReaderAndWriter";
        public string plainTextDocumentReaderAndWriter = DEFAULT_PLAIN_TEXT_READER;
        public bool useBagOfWords = false;
        public bool evaluateBackground = false;
        public int numLopExpert = 1;
        public string initialLopScales = null;
        public string initialLopWeights = null;
        public bool includeFullCRFInLOP = false;
        public bool backpropLopTraining = false;
        public bool randomLopWeights = false;
        public bool randomLopFeatureSplit = false;
        public bool nonLinearCRF = false;
        public bool secondOrderNonLinear = false;
        public int numHiddenUnits = -1;
        public bool useOutputLayer = true;
        public bool useHiddenLayer = true;
        public bool gradientDebug = false;
        public bool checkGradient = false;
        public bool useSigmoid = false;
        public bool skipOutputRegularization = false;
        public bool sparseOutputLayer = false;
        public bool tieOutputLayer = false;
        public bool blockInitialize = false;
        public bool softmaxOutputLayer = false;
        public string loadBisequenceClassifierEn = null;
        public string loadBisequenceClassifierCh = null;
        public string bisequenceClassifierPropEn = null;
        public string bisequenceClassifierPropCh = null;
        public string bisequenceTestFileEn = null;
        public string bisequenceTestFileCh = null;
        public string bisequenceTestOutputEn = null;
        public string bisequenceTestOutputCh = null;
        public string bisequenceTestAlignmentFile = null;
        public int bisequencePriorType = 1;
        public string bisequenceAlignmentPriorPenaltyCh = null;
        public string bisequenceAlignmentPriorPenaltyEn = null;
        public double alignmentPruneThreshold = 0.0;
        public bool factorInAlignmentProb = false;
        public bool useChromaticSampling = false;
        public bool useSequentialScanSampling = false;
        public int maxAllowedChromaticSize = 8;
        public bool keepEmptySentences = false;
        public bool useBilingualNERPrior = false;
        public int samplingSpeedUpThreshold = -1;
        public string entityMatrixCh = null;
        public string entityMatrixEn = null;
        public int multiThreadGibbs = 0;
        public bool matchNERIncentive = false;
        public bool useEmbedding = false;
        public bool prependEmbedding = false;
        public string embeddingWords = null;
        public string embeddingVectors = null;
        public bool transitionEdgeOnly = false;
        public double priorLambda = 0;
        public bool addCapitalFeatures = false;
        public int arbitraryInputLayerSize = -1;
        public bool noEdgeFeature = false;
        public bool terminateOnEvalImprovement = false;
        public int terminateOnEvalImprovementNumOfEpoch = 1;
        public bool useMemoryEvaluator = true;
        public bool suppressTestDebug = false;
        public bool useOWLQN = false;
        public bool printWeights = false;
        public int totalDataSlice = 10;
        public int numOfSlices = 0;
        public bool regularizeSoftmaxTieParam = false;
        public double softmaxTieLambda = 0;
        public int totalFeatureSlice = 10;
        public int numOfFeatureSlices = 0;
        public bool addBiasToEmbedding = false;
        public bool hardcodeSoftmaxOutputWeights = false;
        public bool useNERPriorBIO = false;
        public string entityMatrix = null;
        public int multiThreadClassifier = 0;
        public bool printFactorTable = false;
        public bool useAdaGradFOBOS = false;
        public double initRate = 0.1;
        public bool groupByFeatureTemplate = false;
        public bool groupByOutputClass = false;
        public double priorAlpha = 0;
        public string splitWordRegex = null;
        public bool groupByInput = false;
        public bool groupByHiddenUnit = false;
        public string unigramLM = null;
        public string bigramLM = null;
        public int wordSegBeamSize = 1000;
        public string vocabFile = null;
        public bool averagePerceptron = true;
        public string loadCRFSegmenterPath = null;
        public string loadPCTSegmenterPath = null;
        public string crfSegmenterProp = null;
        public string pctSegmenterProp = null;
        public string intermediateSegmenterOut = null;
        public string intermediateSegmenterModel = null;
        public int dualDecompMaxItr = 0;
        public double dualDecompInitialStepSize = 0.1;
        public bool dualDecompDebug = false;
        public bool useCWSWordFeatures = false;
        public bool useCWSWordFeaturesAll = false;
        public bool useCWSWordFeaturesBigram = false;
        public bool pctSegmenterLenAdjust = false;
        public bool useTrainLexicon = false;
        public List<String> phraseGazettes = null;
        //public Properties props = null;

        public SeqClassifierFlags()
        {
        }

        //public SeqClassifierFlags(Properties props)
        //{
        //    SetProperties(props, true);
        //}

        //public void SetProperties(Properties props)
        //{
        //    SetProperties(props, true);
        //}

        //public virtual void SetProperties(Properties props, bool printProps)
        //{
        //    this.props = props;
        //    StringBuilder sb = new StringBuilder(stringRep);
        //    for (Enumeration e = props.propertyNames(); e.HasMoreElements(); )
        //    {
        //        string key = (string)e.NextElement();
        //        string val = props.GetProperty(key);
        //        if (!(key.Length == 0 && val.Length == 0))
        //        {
        //            if (printProps)
        //            {
        //                Console.Error.WriteLine(key + '=' + val);
        //            }

        //            sb.Append(key).Append('=').Append(val).Append('\n');
        //        }

        //        if (key.EqualsIgnoreCase(@"macro"))
        //        {
        //            if (Boolean.Parse(val))
        //            {
        //                useObservedSequencesOnly = true;
        //                readerAndWriter = @"edu.stanford.nlp.sequences.CoNLLDocumentReaderAndWriter";
        //                useLongSequences = true;
        //                useTaggySequences = true;
        //                useNGrams = true;
        //                usePrev = true;
        //                useNext = true;
        //                useTags = true;
        //                useWordPairs = true;
        //                useSequences = true;
        //                usePrevSequences = true;
        //                noMidNGrams = true;
        //                useReverse = true;
        //                useTypeSeqs = true;
        //                useTypeSeqs2 = true;
        //                useTypeySequences = true;
        //                wordShape = WordShapeClassifier.WORDSHAPEDAN2USELC;
        //                useOccurrencePatterns = true;
        //                useLastRealWord = true;
        //                useNextRealWord = true;
        //                sigma = 3.0;
        //                normalize = true;
        //                normalizeTimex = true;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"goodCoNLL"))
        //        {
        //            if (Boolean.Parse(val))
        //            {
        //                readerAndWriter = @"edu.stanford.nlp.sequences.CoNLLDocumentReaderAndWriter";
        //                useObservedSequencesOnly = true;
        //                useLongSequences = true;
        //                useTaggySequences = true;
        //                useNGrams = true;
        //                usePrev = true;
        //                useNext = true;
        //                useTags = true;
        //                useWordPairs = true;
        //                useSequences = true;
        //                usePrevSequences = true;
        //                noMidNGrams = true;
        //                useReverse = false;
        //                useTypeSeqs = true;
        //                useTypeSeqs2 = true;
        //                useTypeySequences = true;
        //                wordShape = WordShapeClassifier.WORDSHAPEDAN2USELC;
        //                useOccurrencePatterns = true;
        //                useLastRealWord = true;
        //                useNextRealWord = true;
        //                sigma = 50.0;
        //                normalize = true;
        //                normalizeTimex = true;
        //                maxLeft = 2;
        //                useDisjunctive = true;
        //                disjunctionWidth = 4;
        //                useBoundarySequences = true;
        //                useLemmas = true;
        //                usePrevNextLemmas = true;
        //                inputEncoding = @"iso-8859-1";
        //                useQN = true;
        //                QNsize = 15;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"conllNoTags"))
        //        {
        //            if (Boolean.Parse(val))
        //            {
        //                readerAndWriter = @"edu.stanford.nlp.sequences.ColumnDocumentReaderAndWriter";
        //                map = @"word=0,answer=1";
        //                useObservedSequencesOnly = true;
        //                useLongSequences = true;
        //                useNGrams = true;
        //                usePrev = true;
        //                useNext = true;
        //                useWordPairs = true;
        //                useSequences = true;
        //                usePrevSequences = true;
        //                noMidNGrams = true;
        //                useReverse = false;
        //                useTypeSeqs = true;
        //                useTypeSeqs2 = true;
        //                useTypeySequences = true;
        //                wordShape = WordShapeClassifier.WORDSHAPEDAN2USELC;
        //                useLastRealWord = true;
        //                useNextRealWord = true;
        //                sigma = 20.0;
        //                adaptSigma = 20.0;
        //                normalize = true;
        //                normalizeTimex = true;
        //                maxLeft = 2;
        //                useDisjunctive = true;
        //                disjunctionWidth = 4;
        //                useBoundarySequences = true;
        //                inputEncoding = @"iso-8859-1";
        //                useQN = true;
        //                QNsize = 15;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"notags"))
        //        {
        //            if (Boolean.Parse(val))
        //            {
        //                useTags = false;
        //                useSymTags = false;
        //                useTaggySequences = false;
        //                useOccurrencePatterns = false;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"submit"))
        //        {
        //            if (Boolean.Parse(val))
        //            {
        //                useLongSequences = true;
        //                useTaggySequences = true;
        //                useNGrams = true;
        //                usePrev = true;
        //                useNext = true;
        //                useTags = true;
        //                useWordPairs = true;
        //                wordShape = WordShapeClassifier.WORDSHAPEDAN1;
        //                useSequences = true;
        //                usePrevSequences = true;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"binnedLengths"))
        //        {
        //            if (val != null)
        //            {
        //                String[] binnedLengthStrs = val.Split("[, ]+");
        //                binnedLengths = new int[binnedLengthStrs.Length];
        //                for (int i = 0; i < binnedLengths.Length; i++)
        //                {
        //                    binnedLengths[i] = int.Parse(binnedLengthStrs[i]);
        //                }
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"makeConsistent"))
        //        {
        //            makeConsistent = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"dump"))
        //        {
        //            dump = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNGrams"))
        //        {
        //            useNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNeighborNGrams"))
        //        {
        //            useNeighborNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"wordFunction"))
        //        {
        //            wordFunction = ReflectionLoading.LoadByReflection(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"conjoinShapeNGrams"))
        //        {
        //            conjoinShapeNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"lowercaseNGrams"))
        //        {
        //            lowercaseNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useIsURL"))
        //        {
        //            useIsURL = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useURLSequences"))
        //        {
        //            useURLSequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useEntityTypes"))
        //        {
        //            useEntityTypes = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useEntityRule"))
        //        {
        //            useEntityRule = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useOrdinal"))
        //        {
        //            useOrdinal = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useEntityTypeSequences"))
        //        {
        //            useEntityTypeSequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useIsDateRange"))
        //        {
        //            useIsDateRange = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"dehyphenateNGrams"))
        //        {
        //            dehyphenateNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"lowerNewgeneThreshold"))
        //        {
        //            lowerNewgeneThreshold = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePrev"))
        //        {
        //            usePrev = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNext"))
        //        {
        //            useNext = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTags"))
        //        {
        //            useTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordPairs"))
        //        {
        //            useWordPairs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useGazettes"))
        //        {
        //            useGazettes = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"wordShape"))
        //        {
        //            wordShape = WordShapeClassifier.LookupShaper(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useShapeStrings"))
        //        {
        //            useShapeStrings = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useGoodForNamesCpC"))
        //        {
        //            useGoodForNamesCpC = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictionaryConjunctions"))
        //        {
        //            useDictionaryConjunctions = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictionaryConjunctions3"))
        //        {
        //            useDictionaryConjunctions3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"expandMidDot"))
        //        {
        //            expandMidDot = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSequences"))
        //        {
        //            useSequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePrevSequences"))
        //        {
        //            usePrevSequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNextSequences"))
        //        {
        //            useNextSequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLongSequences"))
        //        {
        //            useLongSequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useBoundarySequences"))
        //        {
        //            useBoundarySequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTaggySequences"))
        //        {
        //            useTaggySequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useExtraTaggySequences"))
        //        {
        //            useExtraTaggySequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTaggySequencesShapeInteraction"))
        //        {
        //            useTaggySequencesShapeInteraction = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"strictlyZeroethOrder"))
        //        {
        //            strictlyZeroethOrder = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"strictlyFirstOrder"))
        //        {
        //            strictlyFirstOrder = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"strictlySecondOrder"))
        //        {
        //            strictlySecondOrder = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"strictlyThirdOrder"))
        //        {
        //            strictlyThirdOrder = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"dontExtendTaggy"))
        //        {
        //            dontExtendTaggy = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"entitySubclassification"))
        //        {
        //            entitySubclassification = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useGazettePhrases"))
        //        {
        //            useGazettePhrases = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"phraseGazettes"))
        //        {
        //            StringTokenizer st = new StringTokenizer(val, @" ,;\t");
        //            if (phraseGazettes == null)
        //            {
        //                phraseGazettes = new List<String>();
        //            }

        //            while (st.HasMoreTokens())
        //            {
        //                phraseGazettes.Add(st.NextToken());
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSum"))
        //        {
        //            useSum = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"verbose"))
        //        {
        //            verboseMode = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"verboseMode"))
        //        {
        //            verboseMode = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"tolerance"))
        //        {
        //            tolerance = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"maxIterations"))
        //        {
        //            maxIterations = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"exportFeatures"))
        //        {
        //            exportFeatures = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"printFeatures"))
        //        {
        //            printFeatures = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"printFeaturesUpto"))
        //        {
        //            printFeaturesUpto = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"lastNameList"))
        //        {
        //            lastNameList = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"maleNameList"))
        //        {
        //            maleNameList = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"femaleNameList"))
        //        {
        //            femaleNameList = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSymTags"))
        //        {
        //            useSymTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSymWordPairs"))
        //        {
        //            useSymWordPairs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"printClassifier"))
        //        {
        //            printClassifier = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"printClassifierParam"))
        //        {
        //            printClassifierParam = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"intern"))
        //        {
        //            intern = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"mergetags"))
        //        {
        //            mergeTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"iobtags"))
        //        {
        //            iobTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useViterbi"))
        //        {
        //            useViterbi = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"intern2"))
        //        {
        //            intern2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"selfTest"))
        //        {
        //            selfTest = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"sloppyGazette"))
        //        {
        //            sloppyGazette = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"cleanGazette"))
        //        {
        //            cleanGazette = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"noMidNGrams"))
        //        {
        //            noMidNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useReverse"))
        //        {
        //            useReverse = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"retainEntitySubclassification"))
        //        {
        //            retainEntitySubclassification = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLemmas"))
        //        {
        //            useLemmas = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePrevNextLemmas"))
        //        {
        //            usePrevNextLemmas = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"normalizeTerms"))
        //        {
        //            normalizeTerms = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"normalizeTimex"))
        //        {
        //            normalizeTimex = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNB"))
        //        {
        //            useNB = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useParenMatching"))
        //        {
        //            useParenMatching = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTypeSeqs"))
        //        {
        //            useTypeSeqs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTypeSeqs2"))
        //        {
        //            useTypeSeqs2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTypeSeqs3"))
        //        {
        //            useTypeSeqs3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDisjunctive"))
        //        {
        //            useDisjunctive = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"disjunctionWidth"))
        //        {
        //            disjunctionWidth = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDisjunctiveShapeInteraction"))
        //        {
        //            useDisjunctiveShapeInteraction = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWideDisjunctive"))
        //        {
        //            useWideDisjunctive = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"wideDisjunctionWidth"))
        //        {
        //            wideDisjunctionWidth = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDisjShape"))
        //        {
        //            useDisjShape = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTitle"))
        //        {
        //            useTitle = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"booleanFeatures"))
        //        {
        //            booleanFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useClassFeature"))
        //        {
        //            useClassFeature = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useShapeConjunctions"))
        //        {
        //            useShapeConjunctions = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordTag"))
        //        {
        //            useWordTag = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNPHead"))
        //        {
        //            useNPHead = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNPGovernor"))
        //        {
        //            useNPGovernor = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHeadGov"))
        //        {
        //            useHeadGov = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLastRealWord"))
        //        {
        //            useLastRealWord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNextRealWord"))
        //        {
        //            useNextRealWord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useOccurrencePatterns"))
        //        {
        //            useOccurrencePatterns = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTypeySequences"))
        //        {
        //            useTypeySequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"justify"))
        //        {
        //            justify = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"normalize"))
        //        {
        //            normalize = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"priorType"))
        //        {
        //            priorType = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"sigma"))
        //        {
        //            sigma = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"epsilon"))
        //        {
        //            epsilon = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"beamSize"))
        //        {
        //            beamSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"removeTopN"))
        //        {
        //            removeTopN = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"removeTopNPercent"))
        //        {
        //            removeTopNPercent = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"randomizedRatio"))
        //        {
        //            randomizedRatio = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numTimesRemoveTopN"))
        //        {
        //            numTimesRemoveTopN = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"maxLeft"))
        //        {
        //            maxLeft = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"maxRight"))
        //        {
        //            maxRight = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"maxNGramLeng"))
        //        {
        //            maxNGramLeng = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useGazFeatures"))
        //        {
        //            useGazFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAltGazFeatures"))
        //        {
        //            useAltGazFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMoreGazFeatures"))
        //        {
        //            useMoreGazFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAbbr"))
        //        {
        //            useAbbr = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMinimalAbbr"))
        //        {
        //            useMinimalAbbr = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAbbr1"))
        //        {
        //            useAbbr1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMinimalAbbr1"))
        //        {
        //            useMinimalAbbr1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"documentReader"))
        //        {
        //            Console.Error.WriteLine(@"You are using an outdated flag: -documentReader " + val);
        //            Console.Error.WriteLine(@"Please use -readerAndWriter instead.");
        //        }
        //        else if (key.EqualsIgnoreCase(@"deleteBlankLines"))
        //        {
        //            deleteBlankLines = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"answerFile"))
        //        {
        //            answerFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"altAnswerFile"))
        //        {
        //            altAnswerFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadClassifier") || key.EqualsIgnoreCase(@"model"))
        //        {
        //            loadClassifier = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadTextClassifier"))
        //        {
        //            loadTextClassifier = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadJarClassifier"))
        //        {
        //            loadJarClassifier = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadAuxClassifier"))
        //        {
        //            loadAuxClassifier = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"serializeTo"))
        //        {
        //            serializeTo = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"serializeToText"))
        //        {
        //            serializeToText = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"serializeDatasetsDir"))
        //        {
        //            serializeDatasetsDir = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadDatasetsDir"))
        //        {
        //            loadDatasetsDir = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"pushDir"))
        //        {
        //            pushDir = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"purgeDatasets"))
        //        {
        //            purgeDatasets = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"keepOBInMemory"))
        //        {
        //            keepOBInMemory = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"fakeDataset"))
        //        {
        //            fakeDataset = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numDatasetsPerFile"))
        //        {
        //            numDatasetsPerFile = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"trainFile"))
        //        {
        //            trainFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"biasedTrainFile"))
        //        {
        //            biasedTrainFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"classBias"))
        //        {
        //            classBias = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"confusionMatrix"))
        //        {
        //            confusionMatrix = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"adaptFile"))
        //        {
        //            adaptFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"devFile"))
        //        {
        //            devFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"testFile"))
        //        {
        //            testFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"outputFile"))
        //        {
        //            outputFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"textFile"))
        //        {
        //            textFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"readStdin"))
        //        {
        //            readStdin = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"initialWeights"))
        //        {
        //            initialWeights = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"interimOutputFreq"))
        //        {
        //            interimOutputFreq = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"inputEncoding"))
        //        {
        //            inputEncoding = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"outputEncoding"))
        //        {
        //            outputEncoding = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"gazette"))
        //        {
        //            useGazettes = true;
        //            StringTokenizer st = new StringTokenizer(val, @" ,;\t");
        //            if (gazettes == null)
        //            {
        //                gazettes = new List<String>();
        //            }

        //            while (st.HasMoreTokens())
        //            {
        //                gazettes.Add(st.NextToken());
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"useQN"))
        //        {
        //            useQN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"QNsize"))
        //        {
        //            QNsize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"QNsize2"))
        //        {
        //            QNsize2 = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"l1reg"))
        //        {
        //            useQN = false;
        //            l1reg = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFloat"))
        //        {
        //            useFloat = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"trainMap"))
        //        {
        //            Console.Error.WriteLine(@"trainMap and testMap are no longer valid options - please use map instead.");
        //            throw new Exception();
        //        }
        //        else if (key.EqualsIgnoreCase(@"testMap"))
        //        {
        //            Console.Error.WriteLine(@"trainMap and testMap are no longer valid options - please use map instead.");
        //            throw new Exception();
        //        }
        //        else if (key.EqualsIgnoreCase(@"map"))
        //        {
        //            map = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMoreAbbr"))
        //        {
        //            useMoreAbbr = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePrevVB"))
        //        {
        //            usePrevVB = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNextVB"))
        //        {
        //            useNextVB = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useVB"))
        //        {
        //            if (Boolean.Parse(val))
        //            {
        //                useVB = true;
        //                usePrevVB = true;
        //                useNextVB = true;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"useChunks"))
        //        {
        //            useChunks = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useChunkySequences"))
        //        {
        //            useChunkySequences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"greekifyNGrams"))
        //        {
        //            greekifyNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"restrictTransitionsTimit"))
        //        {
        //            restrictTransitionsTimit = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMoreTags"))
        //        {
        //            useMoreTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useBeginSent"))
        //        {
        //            useBeginSent = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePosition"))
        //        {
        //            usePosition = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useGenia"))
        //        {
        //            useGENIA = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAbstr"))
        //        {
        //            useABSTR = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWeb"))
        //        {
        //            useWEB = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAnte"))
        //        {
        //            useANTE = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAcr"))
        //        {
        //            useACR = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTok"))
        //        {
        //            useTOK = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAbgene"))
        //        {
        //            useABGENE = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAbstrFreqDict"))
        //        {
        //            useABSTRFreqDict = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAbstrFreq"))
        //        {
        //            useABSTRFreq = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFreq"))
        //        {
        //            useFREQ = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usewebfreqdict"))
        //        {
        //            useWEBFreqDict = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"bioSubmitOutput"))
        //        {
        //            bioSubmitOutput = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"subCWGaz"))
        //        {
        //            subCWGaz = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"splitOnHead"))
        //        {
        //            splitOnHead = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"featureCountThreshold"))
        //        {
        //            featureCountThreshold = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWord"))
        //        {
        //            useWord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"memoryThrift"))
        //        {
        //            memoryThrift = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"timitDatum"))
        //        {
        //            timitDatum = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"splitDocuments"))
        //        {
        //            Console.Error.WriteLine(@"You are using an outdated flag: -splitDocuments");
        //            Console.Error.WriteLine(@"Please use -maxDocSize -1 instead.");
        //            splitDocuments = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"featureWeightThreshold"))
        //        {
        //            featureWeightThreshold = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"backgroundSymbol"))
        //        {
        //            backgroundSymbol = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"featureFactory"))
        //        {
        //            featureFactory = val;
        //            if (featureFactory.EqualsIgnoreCase(@"SuperSimpleFeatureFactory"))
        //            {
        //                featureFactory = @"edu.stanford.nlp.sequences.SuperSimpleFeatureFactory";
        //            }
        //            else if (featureFactory.EqualsIgnoreCase(@"NERFeatureFactory"))
        //            {
        //                featureFactory = @"edu.stanford.nlp.ie.NERFeatureFactory";
        //            }
        //            else if (featureFactory.EqualsIgnoreCase(@"GazNERFeatureFactory"))
        //            {
        //                featureFactory = @"edu.stanford.nlp.sequences.GazNERFeatureFactory";
        //            }
        //            else if (featureFactory.EqualsIgnoreCase(@"IncludeAllFeatureFactory"))
        //            {
        //                featureFactory = @"edu.stanford.nlp.sequences.IncludeAllFeatureFactory";
        //            }
        //            else if (featureFactory.EqualsIgnoreCase(@"PhraseFeatureFactory"))
        //            {
        //                featureFactory = @"edu.stanford.nlp.article.extraction.PhraseFeatureFactory";
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"printXML"))
        //        {
        //            printXML = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSeenFeaturesOnly"))
        //        {
        //            useSeenFeaturesOnly = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useBagOfWords"))
        //        {
        //            useBagOfWords = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRadical"))
        //        {
        //            useRadical = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useBigramInTwoClique"))
        //        {
        //            useBigramInTwoClique = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useReverseAffix"))
        //        {
        //            useReverseAffix = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"charHalfWindow"))
        //        {
        //            charHalfWindow = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"purgeFeatures"))
        //        {
        //            purgeFeatures = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"ocrFold"))
        //        {
        //            ocrFold = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"morphFeatureFile"))
        //        {
        //            morphFeatureFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"svmModelFile"))
        //        {
        //            svmModelFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictleng"))
        //        {
        //            useDictleng = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDict2"))
        //        {
        //            useDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useOutDict2"))
        //        {
        //            useOutDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"outDict2"))
        //        {
        //            outDict2 = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictCTB2"))
        //        {
        //            useDictCTB2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictASBC2"))
        //        {
        //            useDictASBC2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictPK2"))
        //        {
        //            useDictPK2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDictHK2"))
        //        {
        //            useDictHK2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWord1"))
        //        {
        //            useWord1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWord2"))
        //        {
        //            useWord2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWord3"))
        //        {
        //            useWord3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWord4"))
        //        {
        //            useWord4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRad1"))
        //        {
        //            useRad1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRad2"))
        //        {
        //            useRad2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRad2b"))
        //        {
        //            useRad2b = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordn"))
        //        {
        //            useWordn = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCTBPre1"))
        //        {
        //            useCTBPre1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCTBSuf1"))
        //        {
        //            useCTBSuf1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useASBCPre1"))
        //        {
        //            useASBCPre1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useASBCSuf1"))
        //        {
        //            useASBCSuf1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHKPre1"))
        //        {
        //            useHKPre1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHKSuf1"))
        //        {
        //            useHKSuf1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePKPre1"))
        //        {
        //            usePKPre1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePKSuf1"))
        //        {
        //            usePKSuf1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCTBChar2"))
        //        {
        //            useCTBChar2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePrediction"))
        //        {
        //            usePrediction = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useASBCChar2"))
        //        {
        //            useASBCChar2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHKChar2"))
        //        {
        //            useHKChar2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePKChar2"))
        //        {
        //            usePKChar2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRule2"))
        //        {
        //            useRule2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useBig5"))
        //        {
        //            useBig5 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegDict2"))
        //        {
        //            useNegDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegDict3"))
        //        {
        //            useNegDict3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegDict4"))
        //        {
        //            useNegDict4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegCTBDict2"))
        //        {
        //            useNegCTBDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegCTBDict3"))
        //        {
        //            useNegCTBDict3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegCTBDict4"))
        //        {
        //            useNegCTBDict4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegASBCDict2"))
        //        {
        //            useNegASBCDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegASBCDict3"))
        //        {
        //            useNegASBCDict3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegASBCDict4"))
        //        {
        //            useNegASBCDict4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegPKDict2"))
        //        {
        //            useNegPKDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegPKDict3"))
        //        {
        //            useNegPKDict3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegPKDict4"))
        //        {
        //            useNegPKDict4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegHKDict2"))
        //        {
        //            useNegHKDict2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegHKDict3"))
        //        {
        //            useNegHKDict3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNegHKDict4"))
        //        {
        //            useNegHKDict4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePre"))
        //        {
        //            usePre = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSuf"))
        //        {
        //            useSuf = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRule"))
        //        {
        //            useRule = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAs"))
        //        {
        //            useAs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePk"))
        //        {
        //            usePk = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHk"))
        //        {
        //            useHk = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMsr"))
        //        {
        //            useMsr = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMSRChar2"))
        //        {
        //            useMSRChar2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFeaturesC4gram"))
        //        {
        //            useFeaturesC4gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFeaturesC5gram"))
        //        {
        //            useFeaturesC5gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFeaturesC6gram"))
        //        {
        //            useFeaturesC6gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFeaturesCpC4gram"))
        //        {
        //            useFeaturesCpC4gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFeaturesCpC5gram"))
        //        {
        //            useFeaturesCpC5gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFeaturesCpC6gram"))
        //        {
        //            useFeaturesCpC6gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useUnicodeType"))
        //        {
        //            useUnicodeType = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useUnicodeBlock"))
        //        {
        //            useUnicodeBlock = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useUnicodeType4gram"))
        //        {
        //            useUnicodeType4gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useUnicodeType5gram"))
        //        {
        //            useUnicodeType5gram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useShapeStrings1"))
        //        {
        //            useShapeStrings1 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useShapeStrings3"))
        //        {
        //            useShapeStrings3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useShapeStrings4"))
        //        {
        //            useShapeStrings4 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useShapeStrings5"))
        //        {
        //            useShapeStrings5 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordUTypeConjunctions2"))
        //        {
        //            useWordUTypeConjunctions2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordUTypeConjunctions3"))
        //        {
        //            useWordUTypeConjunctions3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordShapeConjunctions2"))
        //        {
        //            useWordShapeConjunctions2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordShapeConjunctions3"))
        //        {
        //            useWordShapeConjunctions3 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMidDotShape"))
        //        {
        //            useMidDotShape = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"augmentedDateChars"))
        //        {
        //            augmentedDateChars = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"suppressMidDotPostprocessing"))
        //        {
        //            suppressMidDotPostprocessing = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"printNR"))
        //        {
        //            printNR = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"use4Clique"))
        //        {
        //            use4Clique = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFilter"))
        //        {
        //            useFilter = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"largeChSegFile"))
        //        {
        //            largeChSegFile = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"keepEnglishWhitespaces"))
        //        {
        //            keepEnglishWhitespaces = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"keepAllWhitespaces"))
        //        {
        //            keepAllWhitespaces = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"sighanPostProcessing"))
        //        {
        //            sighanPostProcessing = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useChPos"))
        //        {
        //            useChPos = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"sighanCorporaDict"))
        //        {
        //            sighanCorporaDict = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useObservedSequencesOnly"))
        //        {
        //            useObservedSequencesOnly = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"maxDocSize"))
        //        {
        //            maxDocSize = int.Parse(val);
        //            splitDocuments = true;
        //        }
        //        else if (key.EqualsIgnoreCase(@"printProbs"))
        //        {
        //            printProbs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"printFirstOrderProbs"))
        //        {
        //            printFirstOrderProbs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"saveFeatureIndexToDisk"))
        //        {
        //            saveFeatureIndexToDisk = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"removeBackgroundSingletonFeatures"))
        //        {
        //            removeBackgroundSingletonFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"doGibbs"))
        //        {
        //            doGibbs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNERPrior"))
        //        {
        //            useNERPrior = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAcqPrior"))
        //        {
        //            useAcqPrior = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSemPrior"))
        //        {
        //            useSemPrior = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMUCFeatures"))
        //        {
        //            useMUCFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"initViterbi"))
        //        {
        //            initViterbi = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"checkNameList"))
        //        {
        //            checkNameList = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFirstWord"))
        //        {
        //            useFirstWord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useUnknown"))
        //        {
        //            useUnknown = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"cacheNGrams"))
        //        {
        //            cacheNGrams = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNumberFeature"))
        //        {
        //            useNumberFeature = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"annealingRate"))
        //        {
        //            annealingRate = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"annealingType"))
        //        {
        //            if (val.EqualsIgnoreCase(@"linear") || val.EqualsIgnoreCase(@"exp") || val.EqualsIgnoreCase(@"exponential"))
        //            {
        //                annealingType = val;
        //            }
        //            else
        //            {
        //                Console.Error.WriteLine(@"unknown annealingType: " + val + @".  Please use linear|exp|exponential");
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"numSamples"))
        //        {
        //            numSamples = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"inferenceType"))
        //        {
        //            inferenceType = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadProcessedData"))
        //        {
        //            loadProcessedData = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"normalizationTable"))
        //        {
        //            normalizationTable = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"dictionary"))
        //        {
        //            val = val.Trim();
        //            if (val.Length > 0 && !@"true".Equals(val) && !@"null".Equals(val) && !@"false".Equals(@"val"))
        //            {
        //                dictionary = val;
        //            }
        //            else
        //            {
        //                dictionary = null;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"serDictionary"))
        //        {
        //            val = val.Trim();
        //            if (val.Length > 0 && !@"true".Equals(val) && !@"null".Equals(val) && !@"false".Equals(@"val"))
        //            {
        //                serializedDictionary = val;
        //            }
        //            else
        //            {
        //                serializedDictionary = null;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"dictionary2"))
        //        {
        //            val = val.Trim();
        //            if (val.Length > 0 && !@"true".Equals(val) && !@"null".Equals(val) && !@"false".Equals(@"val"))
        //            {
        //                dictionary2 = val;
        //            }
        //            else
        //            {
        //                dictionary2 = null;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"normTableEncoding"))
        //        {
        //            normTableEncoding = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLemmaAsWord"))
        //        {
        //            useLemmaAsWord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"type"))
        //        {
        //            type = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"readerAndWriter"))
        //        {
        //            readerAndWriter = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"plainTextDocumentReaderAndWriter"))
        //        {
        //            plainTextDocumentReaderAndWriter = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"gazFilesFile"))
        //        {
        //            gazFilesFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"baseTrainDir"))
        //        {
        //            baseTrainDir = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"baseTestDir"))
        //        {
        //            baseTestDir = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"trainFiles"))
        //        {
        //            trainFiles = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"trainFileList"))
        //        {
        //            trainFileList = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"trainDirs"))
        //        {
        //            trainDirs = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"testDirs"))
        //        {
        //            testDirs = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"testFiles"))
        //        {
        //            testFiles = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"textFiles"))
        //        {
        //            textFiles = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePrediction2"))
        //        {
        //            usePrediction2 = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useObservedFeaturesOnly"))
        //        {
        //            useObservedFeaturesOnly = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"iobWrapper"))
        //        {
        //            iobWrapper = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useDistSim"))
        //        {
        //            useDistSim = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"casedDistSim"))
        //        {
        //            casedDistSim = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"distSimFileFormat"))
        //        {
        //            distSimFileFormat = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"distSimMaxBits"))
        //        {
        //            distSimMaxBits = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numberEquivalenceDistSim"))
        //        {
        //            numberEquivalenceDistSim = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"unknownWordDistSimClass"))
        //        {
        //            unknownWordDistSimClass = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useOnlySeenWeights"))
        //        {
        //            useOnlySeenWeights = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"predProp"))
        //        {
        //            predProp = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"distSimLexicon"))
        //        {
        //            distSimLexicon = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSegmentation"))
        //        {
        //            useSegmentation = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useInternal"))
        //        {
        //            useInternal = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useExternal"))
        //        {
        //            useExternal = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useEitherSideWord"))
        //        {
        //            useEitherSideWord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useEitherSideDisjunctive"))
        //        {
        //            useEitherSideDisjunctive = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"featureDiffThresh"))
        //        {
        //            featureDiffThresh = double.Parse(val);
        //            if (props.GetProperty(@"numTimesPruneFeatures") == null)
        //            {
        //                numTimesPruneFeatures = 1;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"numTimesPruneFeatures"))
        //        {
        //            numTimesPruneFeatures = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"newgeneThreshold"))
        //        {
        //            newgeneThreshold = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"adaptFile"))
        //        {
        //            adaptFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"doAdaptation"))
        //        {
        //            doAdaptation = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"selfTrainFile"))
        //        {
        //            selfTrainFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"selfTrainIterations"))
        //        {
        //            selfTrainIterations = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"selfTrainWindowSize"))
        //        {
        //            selfTrainWindowSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"selfTrainConfidenceThreshold"))
        //        {
        //            selfTrainConfidenceThreshold = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numFolds"))
        //        {
        //            numFolds = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"startFold"))
        //        {
        //            startFold = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"endFold"))
        //        {
        //            endFold = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"adaptSigma"))
        //        {
        //            adaptSigma = double.Parse(val);
        //        }
        //        else if (key.StartsWith(@"prop") && !key.Equals(@"prop"))
        //        {
        //            comboProps.Add(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"outputFormat"))
        //        {
        //            outputFormat = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSMD"))
        //        {
        //            useSMD = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useScaledSGD"))
        //        {
        //            useScaledSGD = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"scaledSGDMethod"))
        //        {
        //            scaledSGDMethod = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"tuneSGD"))
        //        {
        //            tuneSGD = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"StochasticCalculateMethod"))
        //        {
        //            if (val.EqualsIgnoreCase(@"AlgorithmicDifferentiation"))
        //            {
        //                stochasticMethod = StochasticCalculateMethods.AlgorithmicDifferentiation;
        //            }
        //            else if (val.EqualsIgnoreCase(@"IncorporatedFiniteDifference"))
        //            {
        //                stochasticMethod = StochasticCalculateMethods.IncorporatedFiniteDifference;
        //            }
        //            else if (val.EqualsIgnoreCase(@"ExternalFinitedifference"))
        //            {
        //                stochasticMethod = StochasticCalculateMethods.ExternalFiniteDifference;
        //            }
        //        }
        //        else if (key.EqualsIgnoreCase(@"initialGain"))
        //        {
        //            initialGain = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"stochasticBatchSize"))
        //        {
        //            stochasticBatchSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"SGD2QNhessSamples"))
        //        {
        //            SGD2QNhessSamples = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSGD"))
        //        {
        //            useSGD = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useInPlaceSGD"))
        //        {
        //            useInPlaceSGD = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSGDtoQN"))
        //        {
        //            useSGDtoQN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"SGDPasses"))
        //        {
        //            SGDPasses = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"QNPasses"))
        //        {
        //            QNPasses = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"gainSGD"))
        //        {
        //            gainSGD = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHybrid"))
        //        {
        //            useHybrid = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"hybridCutoffIteration"))
        //        {
        //            hybridCutoffIteration = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useStochasticQN"))
        //        {
        //            useStochasticQN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"outputIterationsToFile"))
        //        {
        //            outputIterationsToFile = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"testObjFunction"))
        //        {
        //            testObjFunction = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"testVariance"))
        //        {
        //            testVariance = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"CRForder"))
        //        {
        //            CRForder = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"CRFwindow"))
        //        {
        //            CRFwindow = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"testHessSamples"))
        //        {
        //            testHessSamples = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"estimateInitial"))
        //        {
        //            estimateInitial = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"printLabelValue"))
        //        {
        //            printLabelValue = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"searchGraphPrefix"))
        //        {
        //            searchGraphPrefix = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"searchGraphPrune"))
        //        {
        //            searchGraphPrune = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"kBest"))
        //        {
        //            useKBest = true;
        //            kBest = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useRobustQN"))
        //        {
        //            useRobustQN = true;
        //        }
        //        else if (key.EqualsIgnoreCase(@"combo"))
        //        {
        //            combo = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"verboseForTrueCasing"))
        //        {
        //            verboseForTrueCasing = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"trainHierarchical"))
        //        {
        //            trainHierarchical = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"domain"))
        //        {
        //            domain = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"baseline"))
        //        {
        //            baseline = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"doFE"))
        //        {
        //            doFE = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"restrictLabels"))
        //        {
        //            restrictLabels = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"transferSigmas"))
        //        {
        //            transferSigmas = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"announceObjectBankEntries"))
        //        {
        //            announceObjectBankEntries = true;
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePos"))
        //        {
        //            usePos = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAgreement"))
        //        {
        //            useAgreement = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAccCase"))
        //        {
        //            useAccCase = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useInna"))
        //        {
        //            useInna = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useConcord"))
        //        {
        //            useConcord = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useFirstNgram"))
        //        {
        //            useFirstNgram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLastNgram"))
        //        {
        //            useLastNgram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"collapseNN"))
        //        {
        //            collapseNN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTagsCpC"))
        //        {
        //            useTagsCpC = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTagsCpCp2C"))
        //        {
        //            useTagsCpCp2C = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTagsCpCp2Cp3C"))
        //        {
        //            useTagsCpCp2Cp3C = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTagsCpCp2Cp3Cp4C"))
        //        {
        //            useTagsCpCp2Cp3Cp4C = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numTags"))
        //        {
        //            numTags = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useConjBreak"))
        //        {
        //            useConjBreak = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAuxPairs"))
        //        {
        //            useAuxPairs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePPVBPairs"))
        //        {
        //            usePPVBPairs = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAnnexing"))
        //        {
        //            useAnnexing = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTemporalNN"))
        //        {
        //            useTemporalNN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"markProperNN"))
        //        {
        //            markProperNN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePath"))
        //        {
        //            usePath = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"markMasdar"))
        //        {
        //            markMasdar = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"innaPPAttach"))
        //        {
        //            innaPPAttach = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSVO"))
        //        {
        //            useSVO = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"mixedCaseMapFile"))
        //        {
        //            mixedCaseMapFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"auxTrueCaseModels"))
        //        {
        //            auxTrueCaseModels = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"use2W"))
        //        {
        //            use2W = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLC"))
        //        {
        //            useLC = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useYetMoreCpCShapes"))
        //        {
        //            useYetMoreCpCShapes = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useIfInteger"))
        //        {
        //            useIfInteger = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"twoStage"))
        //        {
        //            twoStage = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"evaluateIters"))
        //        {
        //            evaluateIters = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"evalCmd"))
        //        {
        //            evalCmd = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"evaluateTrain"))
        //        {
        //            evaluateTrain = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"evaluateBackground"))
        //        {
        //            evaluateBackground = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"tuneSampleSize"))
        //        {
        //            tuneSampleSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTopics"))
        //        {
        //            useTopics = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePhraseFeatures"))
        //        {
        //            usePhraseFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePhraseWords"))
        //        {
        //            usePhraseWords = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePhraseWordTags"))
        //        {
        //            usePhraseWordTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"usePhraseWordSpecialTags"))
        //        {
        //            usePhraseWordSpecialTags = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useProtoFeatures"))
        //        {
        //            useProtoFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useWordnetFeatures"))
        //        {
        //            useWordnetFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"wikiFeatureDbFile"))
        //        {
        //            wikiFeatureDbFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"tokenizerOptions"))
        //        {
        //            tokenizerOptions = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"tokenizerFactory"))
        //        {
        //            tokenizerFactory = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCommonWordsFeature"))
        //        {
        //            useCommonWordsFeature = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useYear"))
        //        {
        //            useYear = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSentenceNumber"))
        //        {
        //            useSentenceNumber = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useLabelSource"))
        //        {
        //            useLabelSource = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"tokenFactory"))
        //        {
        //            tokenFactory = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"tokensAnnotationClassName"))
        //        {
        //            tokensAnnotationClassName = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"numLopExpert"))
        //        {
        //            numLopExpert = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"initialLopScales"))
        //        {
        //            initialLopScales = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"initialLopWeights"))
        //        {
        //            initialLopWeights = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"includeFullCRFInLOP"))
        //        {
        //            includeFullCRFInLOP = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"backpropLopTraining"))
        //        {
        //            backpropLopTraining = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"randomLopWeights"))
        //        {
        //            randomLopWeights = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"randomLopFeatureSplit"))
        //        {
        //            randomLopFeatureSplit = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"nonLinearCRF"))
        //        {
        //            nonLinearCRF = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"secondOrderNonLinear"))
        //        {
        //            secondOrderNonLinear = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numHiddenUnits"))
        //        {
        //            numHiddenUnits = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useOutputLayer"))
        //        {
        //            useOutputLayer = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useHiddenLayer"))
        //        {
        //            useHiddenLayer = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"gradientDebug"))
        //        {
        //            gradientDebug = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"checkGradient"))
        //        {
        //            checkGradient = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSigmoid"))
        //        {
        //            useSigmoid = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"skipOutputRegularization"))
        //        {
        //            skipOutputRegularization = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"sparseOutputLayer"))
        //        {
        //            sparseOutputLayer = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"tieOutputLayer"))
        //        {
        //            tieOutputLayer = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"blockInitialize"))
        //        {
        //            blockInitialize = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"softmaxOutputLayer"))
        //        {
        //            softmaxOutputLayer = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadBisequenceClassifierEn"))
        //        {
        //            loadBisequenceClassifierEn = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceClassifierPropEn"))
        //        {
        //            bisequenceClassifierPropEn = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadBisequenceClassifierCh"))
        //        {
        //            loadBisequenceClassifierCh = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceClassifierPropCh"))
        //        {
        //            bisequenceClassifierPropCh = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceTestFileEn"))
        //        {
        //            bisequenceTestFileEn = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceTestFileCh"))
        //        {
        //            bisequenceTestFileCh = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceTestOutputEn"))
        //        {
        //            bisequenceTestOutputEn = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceTestOutputCh"))
        //        {
        //            bisequenceTestOutputCh = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceTestAlignmentFile"))
        //        {
        //            bisequenceTestAlignmentFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequencePriorType"))
        //        {
        //            bisequencePriorType = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceAlignmentPriorPenaltyCh"))
        //        {
        //            bisequenceAlignmentPriorPenaltyCh = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bisequenceAlignmentPriorPenaltyEn"))
        //        {
        //            bisequenceAlignmentPriorPenaltyEn = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"alignmentPruneThreshold"))
        //        {
        //            alignmentPruneThreshold = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"factorInAlignmentProb"))
        //        {
        //            factorInAlignmentProb = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useChromaticSampling"))
        //        {
        //            useChromaticSampling = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useSequentialScanSampling"))
        //        {
        //            useSequentialScanSampling = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"maxAllowedChromaticSize"))
        //        {
        //            maxAllowedChromaticSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"keepEmptySentences"))
        //        {
        //            keepEmptySentences = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useBilingualNERPrior"))
        //        {
        //            useBilingualNERPrior = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"samplingSpeedUpThreshold"))
        //        {
        //            samplingSpeedUpThreshold = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"entityMatrixCh"))
        //        {
        //            entityMatrixCh = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"entityMatrixEn"))
        //        {
        //            entityMatrixEn = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"multiThreadGibbs"))
        //        {
        //            multiThreadGibbs = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"matchNERIncentive"))
        //        {
        //            matchNERIncentive = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useEmbedding"))
        //        {
        //            useEmbedding = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"prependEmbedding"))
        //        {
        //            prependEmbedding = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"embeddingWords"))
        //        {
        //            embeddingWords = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"embeddingVectors"))
        //        {
        //            embeddingVectors = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"transitionEdgeOnly"))
        //        {
        //            transitionEdgeOnly = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"priorLambda"))
        //        {
        //            priorLambda = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"addCapitalFeatures"))
        //        {
        //            addCapitalFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"arbitraryInputLayerSize"))
        //        {
        //            arbitraryInputLayerSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"noEdgeFeature"))
        //        {
        //            noEdgeFeature = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"terminateOnEvalImprovement"))
        //        {
        //            terminateOnEvalImprovement = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"terminateOnEvalImprovementNumOfEpoch"))
        //        {
        //            terminateOnEvalImprovementNumOfEpoch = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useMemoryEvaluator"))
        //        {
        //            useMemoryEvaluator = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"suppressTestDebug"))
        //        {
        //            suppressTestDebug = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useOWLQN"))
        //        {
        //            useOWLQN = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"printWeights"))
        //        {
        //            printWeights = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"totalDataSlice"))
        //        {
        //            totalDataSlice = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numOfSlices"))
        //        {
        //            numOfSlices = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"regularizeSoftmaxTieParam"))
        //        {
        //            regularizeSoftmaxTieParam = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"softmaxTieLambda"))
        //        {
        //            softmaxTieLambda = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"totalFeatureSlice"))
        //        {
        //            totalFeatureSlice = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"numOfFeatureSlices"))
        //        {
        //            numOfFeatureSlices = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"addBiasToEmbedding"))
        //        {
        //            addBiasToEmbedding = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"hardcodeSoftmaxOutputWeights"))
        //        {
        //            hardcodeSoftmaxOutputWeights = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useNERPriorBIO"))
        //        {
        //            useNERPriorBIO = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"entityMatrix"))
        //        {
        //            entityMatrix = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"multiThreadClassifier"))
        //        {
        //            multiThreadClassifier = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useGenericFeatures"))
        //        {
        //            useGenericFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"printFactorTable"))
        //        {
        //            printFactorTable = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useAdaGradFOBOS"))
        //        {
        //            useAdaGradFOBOS = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"initRate"))
        //        {
        //            initRate = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"groupByFeatureTemplate"))
        //        {
        //            groupByFeatureTemplate = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"groupByOutputClass"))
        //        {
        //            groupByOutputClass = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"priorAlpha"))
        //        {
        //            priorAlpha = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"splitWordRegex"))
        //        {
        //            splitWordRegex = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"groupByInput"))
        //        {
        //            groupByInput = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"groupByHiddenUnit"))
        //        {
        //            groupByHiddenUnit = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"unigramLM"))
        //        {
        //            unigramLM = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"bigramLM"))
        //        {
        //            bigramLM = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"wordSegBeamSize"))
        //        {
        //            wordSegBeamSize = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"vocabFile"))
        //        {
        //            vocabFile = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"averagePerceptron"))
        //        {
        //            averagePerceptron = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadCRFSegmenterPath"))
        //        {
        //            loadCRFSegmenterPath = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"loadPCTSegmenterPath"))
        //        {
        //            loadPCTSegmenterPath = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"crfSegmenterProp"))
        //        {
        //            crfSegmenterProp = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"pctSegmenterProp"))
        //        {
        //            pctSegmenterProp = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"dualDecompMaxItr"))
        //        {
        //            dualDecompMaxItr = int.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"dualDecompInitialStepSize"))
        //        {
        //            dualDecompInitialStepSize = double.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"dualDecompDebug"))
        //        {
        //            dualDecompDebug = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"intermediateSegmenterOut"))
        //        {
        //            intermediateSegmenterOut = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"intermediateSegmenterModel"))
        //        {
        //            intermediateSegmenterModel = val;
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCWSWordFeatures"))
        //        {
        //            useCWSWordFeatures = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCWSWordFeaturesAll"))
        //        {
        //            useCWSWordFeaturesAll = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useCWSWordFeaturesBigram"))
        //        {
        //            useCWSWordFeaturesBigram = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"pctSegmenterLenAdjust"))
        //        {
        //            pctSegmenterLenAdjust = Boolean.Parse(val);
        //        }
        //        else if (key.EqualsIgnoreCase(@"useTrainLexicon"))
        //        {
        //            useTrainLexicon = Boolean.Parse(val);
        //        }
        //        else if (key.Length > 0 && !key.Equals(@"prop"))
        //        {
        //            Console.Error.WriteLine(@"Unknown property: |" + key + '|');
        //        }
        //    }

        //    if (startFold > numFolds)
        //    {
        //        Console.Error.WriteLine(@"startFold > numFolds -> setting startFold to 1");
        //        startFold = 1;
        //    }

        //    if (endFold > numFolds)
        //    {
        //        Console.Error.WriteLine(@"endFold > numFolds -> setting to numFolds");
        //        endFold = numFolds;
        //    }

        //    if (combo)
        //    {
        //        splitDocuments = false;
        //    }

        //    stringRep = sb.ToString();
        //}

        public override string ToString()
        {
            return stringRep;
        }

        public virtual string GetNotNullTrueStringRep()
        {
            try
            {
                string rep = @"";
                string joiner = @"\n";
                FieldInfo[] f = this.GetType().GetFields();
                foreach (FieldInfo ff in f)
                {
                    string name = ff.Name;
                    Type type = ff.FieldType;
                    if (type.Equals(typeof(bool)))
                    {
                        bool val = (bool)ff.GetValue(this);
                        if (val)
                        {
                            rep += joiner + name + @"=" + val;
                        }
                    }
                    else if (type.Equals(typeof(string)))
                    {
                        string val = (string)ff.GetValue(this);
                        if (val != null)
                            rep += joiner + name + @"=" + val;
                    }
                    else if (type.Equals(typeof(double)))
                    {
                        double val = (double)ff.GetValue(this);
                        rep += joiner + name + @"=" + val;
                    }
                    else if (type.Equals(typeof(int)))
                    {
                        int val = (int)ff.GetValue(this);
                        rep += joiner + name + @"=" + val;
                    }
                    else if (type.Equals(typeof(float)))
                    {
                        float val = (float)ff.GetValue(this);
                        rep += joiner + name + @"=" + val;
                    }
                    else if (type.Equals(typeof(byte)))
                    {
                        byte val = (byte)ff.GetValue(this);
                        rep += joiner + name + @"=" + val;
                    }
                    else if (type.Equals(typeof(char)))
                    {
                        char val = (char)ff.GetValue(this);
                        rep += joiner + name + @"=" + val;
                    }
                    else if (type.Equals(typeof(long)))
                    {
                        long val = (long)ff.GetValue(this);
                        rep += joiner + name + @"=" + val;
                    }
                }

                return rep;
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }

            return null;
        }
    }
}
