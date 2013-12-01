using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class CoreAnnotations
    {
        private CoreAnnotations()
        {
        }

        public class TextAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LemmaAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PartOfSpeechAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NamedEntityTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class StackedNamedEntityTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class TrueCaseAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class TrueCaseTextAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class TokensAnnotation : ICoreAnnotation<List<CoreLabel>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<CoreLabel>);
            }
        }

        public class GenericTokensAnnotation : ICoreAnnotation<List<ICoreMap>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<ICoreMap>);
            }
        }

        public class SentencesAnnotation : ICoreAnnotation<List<ICoreMap>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<ICoreMap>);
            }
        }

        public class ParagraphsAnnotation : ICoreAnnotation<List<ICoreMap>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<ICoreMap>);
            }
        }

        public class TokenBeginAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class TokenEndAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class CalendarAnnotation : ICoreAnnotation<Calendar>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Calendar);
            }
        }

        public class DocIDAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class IndexAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class BeginIndexAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class EndIndexAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class ForcedSentenceEndAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class SentenceIndexAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class LineNumberAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class ValueAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CategoryAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class OriginalTextAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class BeforeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class AfterAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CoarseTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CoNLLDepAnnotation : ICoreAnnotation<ICoreMap>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(ICoreMap);
            }
        }

        public class CoNLLPredicateAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class CoNLLSRLAnnotation : ICoreAnnotation<IDictionary<int, String>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(IDictionary<int, string>);
            }
        }

        public class CoNLLDepTypeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CoNLLDepParentIndexAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class IDFAnnotation : ICoreAnnotation<Double>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Double);
            }
        }

        public class ProjectedCategoryAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ArgumentAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MarkingAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SemanticHeadWordAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SemanticHeadTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class VerbSenseAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CategoryFunctionalTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NERIDAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NormalizedNamedEntityTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public enum SRL_ID
        {
            ARG,
            NO,
            ALL_NO,
            REL
        }

        public class SRLIDAnnotation : ICoreAnnotation<SRL_ID>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(SRL_ID);
            }
        }

        public class ShapeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LeftTermAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class ParentAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class INAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SpanAnnotation : ICoreAnnotation<IntPair>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(IntPair);
            }
        }

        public class AnswerAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class GoldAnswerAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class FeaturesAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class InterpretationAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class RoleAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class GazetteerAnnotation : ICoreAnnotation<List<String>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<string>);
            }
        }

        public class StemAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PolarityAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MorphoNumAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MorphoPersAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MorphoGenAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MorphoCaseAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ChineseCharAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ChineseOrigSegAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ChineseSegAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ChineseIsSegmentedAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class CharacterOffsetBeginAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class CharacterOffsetEndAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class CostMagnificationAnnotation : ICoreAnnotation<Double>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Double);
            }
        }

        public class WordSenseAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SRLInstancesAnnotation : ICoreAnnotation<List<List<Pair<String, Pair>>>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<List<Pair<String, Pair>>>);
            }
        }

        public class NumTxtSentencesAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class TagLabelAnnotation : ICoreAnnotation<ILabel>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(ILabel);
            }
        }

        public class DomainAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PositionAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CharAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class UnknownAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class IDAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class GazAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PossibleAnswersAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class DistSimAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class AbbrAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ChunkAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class GovernorAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class AbgeneAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class GeniaAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class AbstrAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class FreqAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class DictAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class WebAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class FemaleGazAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MaleGazAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LastGazAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class IsURLAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class EntityTypeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class IsDateRangeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PredictedAnswerAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class OriginalAnswerAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class OriginalCharAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class UTypeAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class EntityRuleAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SectionAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class WordPositionAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ParaPositionAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SentencePositionAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SentenceIDAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class EntityClassAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class AnswerObjectAnnotation : ICoreAnnotation<Object>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Object);
            }
        }

        public class BestCliquesAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class BestFullAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LastTaggedAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LabelAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NeighborsAnnotation : ICoreAnnotation<List<Pair<WordLemmaTag, String>>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<Pair<WordLemmaTag, String>>);
            }
        }

        public class ContextsAnnotation : ICoreAnnotation<List<Pair<String, String>>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<Pair<String, String>>);
            }
        }

        public class DependentsAnnotation : ICoreAnnotation<List<Pair<Triple<String, String, String>, String>>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<Pair<Triple<String, String, String>, String>>);
            }
        }

        public class WordFormAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class TrueTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SubcategorizationAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class BagOfWordsAnnotation : ICoreAnnotation<List<Tuple<String, String>>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<Tuple<string, string>>);
            }
        }

        public class HeightAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LengthAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LBeginAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LMiddleAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LEndAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class D2_LBeginAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class D2_LMiddleAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class D2_LEndAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class UBlockAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SpaceBeforeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class StateAnnotation : ICoreAnnotation<CoreLabel>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(CoreLabel);
            }
        }

        public class PrevChildAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class FirstChildAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class UnaryAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class DoAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class HaveAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class BeAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class NotAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class PercentAnnotation : ICoreAnnotation<Boolean>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(bool);
            }
        }

        public class GrandparentAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class HeadWordStringAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class MonthAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class DayAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class YearAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PriorAnnotation : ICoreAnnotation<IDictionary<String, Double>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(IDictionary<String, Double>);
            }
        }

        public class SemanticWordAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class SemanticTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CovertIDAnnotation : ICoreAnnotation<List<IntPair>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<IntPair>);
            }
        }

        public class ArgDescendentAnnotation : ICoreAnnotation<Tuple<String, Double>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Tuple<string, double>);
            }
        }

        public class CopyAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class XmlElementAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class XmlContextAnnotation : ICoreAnnotation<List<String>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<string>);
            }
        }

        public class TopicAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class WordnetSynAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PhraseWordsTagAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class PhraseWordsAnnotation : ICoreAnnotation<List<String>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<string>);
            }
        }

        public class ProtoAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class CommonWordsAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class DocDateAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NumericTypeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NumericValueAnnotation : ICoreAnnotation<long>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(long);
            }
        }

        public class NumericObjectAnnotation : ICoreAnnotation<Object>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Object);
            }
        }

        public class NumericCompositeValueAnnotation : ICoreAnnotation<long>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(long);
            }
        }

        public class NumericCompositeTypeAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class NumericCompositeObjectAnnotation : ICoreAnnotation<Object>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Object);
            }
        }

        public class NumerizedTokensAnnotation : ICoreAnnotation<List<ICoreMap>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(List<ICoreMap>);
            }
        }

        public class UtteranceAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class SpeakerAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class ParagraphAnnotation : ICoreAnnotation<int>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(int);
            }
        }

        public class LeftChildrenNodeAnnotation : ICoreAnnotation<SortedSet<Pair<CoreLabel, String>>>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(SortedSet<Pair<CoreLabel, String>>);
            }
        }

        public class AntecedentAnnotation : ICoreAnnotation<String>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(string);
            }
        }

        public class LabelWeightAnnotation : ICoreAnnotation<Double>
        {
            public virtual Type GetTypeValue()
            {
                return typeof(Double);
            }
        }
    }
}
