using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class WordTag : ILabel, IHasWord, IHasTag, IComparable<WordTag>
    {
        private string word;
        private string tag;
        private static readonly string DIVIDER = @"/";
        public WordTag(string word, string tag)
        {
            SetWord(word);
            SetTag(tag);
        }

        public WordTag(string word)
            : this(word, null)
        {
        }

        public WordTag(ILabel word)
            : this(word.Value(), ((IHasTag)word).Tag())
        {
        }

        public WordTag()
        {
        }

        public WordTag(ILabel word, ILabel tag)
            : this(word.Value(), tag.Value())
        {
        }

        public static WordTag ValueOf(string s)
        {
            WordTag result = new WordTag();
            result.SetFromString(s);
            return result;
        }

        public static WordTag ValueOf(string s, string tagDivider)
        {
            WordTag result = new WordTag();
            result.SetFromString(s, tagDivider);
            return result;
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

        public virtual string Tag()
        {
            return tag;
        }

        public virtual void SetWord(string word)
        {
            SetValue(word);
        }

        public virtual void SetTag(string tag)
        {
            this.tag = tag;
        }

        public override string ToString()
        {
            return ToString(DIVIDER);
        }

        public virtual string ToString(string divider)
        {
            string tag = Tag();
            if (tag == null)
            {
                return Word();
            }
            else
            {
                return Word() + divider + tag;
            }
        }

        public virtual void SetFromString(string wordTagString)
        {
            SetFromString(wordTagString, DIVIDER);
        }

        public virtual void SetFromString(string wordTagString, string divider)
        {
            int where = wordTagString.LastIndexOf(divider);
            if (where >= 0)
            {
                SetWord(string.Intern(wordTagString.Substring(0, where));
                SetTag(string.Intern(wordTagString.Substring(where + 1));
            }
            else
            {
                SetWord(string.Intern(wordTagString));
                SetTag(null);
            }
        }

        public override bool Equals(Object o)
        {
            if (this == o)
                return true;
            if (!(o is WordTag))
                return false;
            WordTag wordTag = (WordTag)o;
            if (tag != null ? !tag.Equals(wordTag.tag) : wordTag.tag != null)
                return false;
            if (word != null ? !word.Equals(wordTag.word) : wordTag.word != null)
                return false;
            return true;
        }

        public override int GetHashCode()
        {
            int result;
            result = (word != null ? word.GetHashCode() : 0);
            result = 29 * result + (tag != null ? tag.GetHashCode() : 0);
            return result;
        }

        public virtual int CompareTo(WordTag wordTag)
        {
            int first = (word != null ? Word().CompareTo(wordTag.Word()) : 0);
            if (first != 0)
                return first;
            else
            {
                if (Tag() == null)
                {
                    if (wordTag.Tag() == null)
                        return 0;
                    else
                        return -1;
                }

                return Tag().CompareTo(wordTag.Tag());
            }
        }

        private class LabelFactoryHolder
        {
            internal static readonly ILabelFactory lf = new WordTagFactory();
        }

        public virtual ILabelFactory LabelFactory()
        {
            return LabelFactoryHolder.lf;
        }

        public static ILabelFactory Factory()
        {
            return LabelFactoryHolder.lf;
        }

        public virtual void Read(BinaryReader in_renamed)
        {
            try
            {
                word = in_renamed.ReadString();
                tag = in_renamed.ReadString();
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }
        }

        public virtual void Save(BinaryWriter out_renamed)
        {
            try
            {
                out_renamed.Write(word);
                out_renamed.Write(tag);
            }
            catch (Exception e)
            {
                //e.PrintStackTrace();
            }
        }
    }
}
