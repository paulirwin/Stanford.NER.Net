using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class LabeledWord : Word
    {
        private ILabel tag;
        private static readonly string DIVIDER = @"/";
        public LabeledWord()
            : base()
        {
        }

        public LabeledWord(string word)
            : base(word)
        {
        }

        public LabeledWord(string word, ILabel tag)
            : base(word)
        {
            this.tag = tag;
        }

        public LabeledWord(ILabel word, ILabel tag)
            : base(word)
        {
            this.tag = tag;
        }

        public virtual ILabel Tag()
        {
            return tag;
        }

        public virtual void SetTag(ILabel tag)
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
