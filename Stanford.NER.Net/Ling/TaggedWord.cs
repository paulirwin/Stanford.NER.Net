using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class TaggedWord : Word, IHasTag
    {
        private string tag;
        private static readonly string DIVIDER = @"/";
        public TaggedWord()
            : base()
        {
        }

        public TaggedWord(string word)
            : base(word)
        {
        }

        public TaggedWord(string word, string tag)
            : base(word)
        {
            this.tag = tag;
        }

        public TaggedWord(ILabel oldLabel)
            : base(oldLabel.Value())
        {
            if (oldLabel is IHasTag)
            {
                this.tag = ((IHasTag)oldLabel).Tag();
            }
        }

        public TaggedWord(ILabel word, ILabel tag)
            : base(word)
        {
            this.tag = tag.Value();
        }

        public virtual string Tag()
        {
            return tag;
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
            return Word() + divider + tag;
        }

        public virtual void SetFromString(string taggedWord)
        {
            SetFromString(taggedWord, DIVIDER);
        }

        public virtual void SetFromString(string taggedWord, string divider)
        {
            int where = taggedWord.LastIndexOf(divider);
            if (where >= 0)
            {
                SetWord(taggedWord.Substring(0, where));
                SetTag(taggedWord.Substring(where + 1));
            }
            else
            {
                SetWord(taggedWord);
                SetTag(null);
            }
        }

        private class LabelFactoryHolder
        {
            private LabelFactoryHolder()
            {
            }

            internal static readonly ILabelFactory lf = new TaggedWordFactory();
        }

        public override ILabelFactory LabelFactory()
        {
            return LabelFactoryHolder.lf;
        }

        public static ILabelFactory Factory()
        {
            return LabelFactoryHolder.lf;
        }

    }
}
