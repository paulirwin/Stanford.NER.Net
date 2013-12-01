using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class WordLemmaTag : ILabel, IComparable<WordLemmaTag>, IHasWord, IHasTag
    {
        private string word;
        private string lemma;
        private string tag;
        private static readonly string DIVIDER = @"/";
        public WordLemmaTag(string word)
        {
            this.word = word;
            this.lemma = null;
            SetTag(null);
        }

        public WordLemmaTag(ILabel word)
            : this(word.Value())
        {
        }

        public WordLemmaTag()
        {
        }

        public WordLemmaTag(string word, string tag)
        {
            WordTag wT = new WordTag(word, tag);
            this.word = word;
            this.lemma = Morphology.StemStatic(wT).Word();
            SetTag(tag);
        }

        public WordLemmaTag(string word, string lemma, string tag)
            : this(word)
        {
            this.lemma = lemma;
            SetTag(tag);
        }

        public WordLemmaTag(ILabel word, ILabel tag)
            : this(word)
        {
            WordTag wT = new WordTag(word, tag);
            this.lemma = Morphology.StemStatic(wT).Word();
            SetTag(tag.Value());
        }

        public virtual string Value()
        {
            return word;
        }

        public virtual string Word()
        {
            return Value();
        }

        public virtual void SetValue(string value)
        {
            word = value;
        }

        public virtual void SetWord(string word)
        {
            SetValue(word);
        }

        public virtual void SetLemma(string lemma)
        {
            this.lemma = lemma;
        }

        public void SetTag(string tag)
        {
            this.tag = tag;
        }

        public virtual string Tag()
        {
            return tag;
        }

        public virtual string Lemma()
        {
            return lemma;
        }

        public override string ToString()
        {
            return ToString(DIVIDER);
        }

        public virtual string ToString(string divider)
        {
            return Word() + divider + lemma + divider + tag;
        }

        public virtual void SetFromString(string labelStr)
        {
            SetFromString(labelStr, DIVIDER);
        }

        public virtual void SetFromString(string labelStr, string divider)
        {
            int first = labelStr.IndexOf(divider);
            int second = labelStr.LastIndexOf(divider);
            if (first == second)
            {
                SetWord(labelStr.Substring(0, first));
                SetTag(labelStr.Substring(first + 1));
                SetLemma(Morphology.StemStatic(labelStr.Substring(0, first), labelStr.Substring(first + 1)).Word());
            }
            else if (first >= 0)
            {
                SetWord(labelStr.Substring(0, first));
                SetLemma(labelStr.Substring(first + 1, second));
                SetTag(labelStr.Substring(second + 1));
            }
            else
            {
                SetWord(labelStr);
                SetLemma(null);
                SetTag(null);
            }
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (!(o is WordLemmaTag))
                return false;
            WordLemmaTag other = (WordLemmaTag)o;
            return Word().Equals(other.Word()) && Lemma().Equals(other.Lemma()) && Tag().Equals(other.Tag());
        }

        public override int GetHashCode()
        {
            int result;
            result = (word != null ? word.GetHashCode() : 3);
            result = 29 * result + (tag != null ? tag.GetHashCode() : 0);
            result = 29 * result + (lemma != null ? lemma.GetHashCode() : 0);
            return result;
        }

        public virtual int CompareTo(WordLemmaTag wordLemmaTag)
        {
            int first = Word().CompareTo(wordLemmaTag.Word());
            if (first != 0)
                return first;
            int second = Lemma().CompareTo(wordLemmaTag.Lemma());
            if (second != 0)
                return second;
            else
                return Tag().CompareTo(wordLemmaTag.Tag());
        }

        public virtual ILabelFactory LabelFactory()
        {
            return new WordLemmaTagFactory();
        }

        public static void Main(String[] args)
        {
            WordLemmaTag wLT = new WordLemmaTag();
            wLT.SetFromString(@"hunter/NN");
            Console.Out.WriteLine(wLT.Word());
            Console.Out.WriteLine(wLT.Lemma());
            Console.Out.WriteLine(wLT.Tag());
            WordLemmaTag wLT2 = new WordLemmaTag();
            wLT2.SetFromString(@"bought/buy/V");
            Console.Out.WriteLine(wLT2.Word());
            Console.Out.WriteLine(wLT2.Lemma());
            Console.Out.WriteLine(wLT2.Tag());
            WordLemmaTag wLT3 = new WordLemmaTag();
            wLT2.SetFromString(@"life");
            Console.Out.WriteLine(wLT3.Word());
            Console.Out.WriteLine(wLT3.Lemma());
            Console.Out.WriteLine(wLT3.Tag());
        }

    }
}
