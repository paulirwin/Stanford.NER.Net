using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class Word : StringLabel, IHasWord
    {
        public static readonly string EMPTY_STRING = @"*t*";
        public static readonly Word EMPTY = new Word(EMPTY_STRING);
        public Word()
            : base()
        {
        }

        public Word(string word)
            : base(word)
        {
        }

        public Word(string word, int beginPosition, int endPosition)
            : base(word, beginPosition, endPosition)
        {
        }

        public Word(ILabel lab)
            : base(lab)
        {
        }

        public override string Word()
        {
            return Value();
        }

        public override void SetWord(string word)
        {
            SetValue(word);
        }

        private class WordFactoryHolder
        {
            internal static readonly ILabelFactory lf = new WordFactory();
            private WordFactoryHolder()
            {
            }
        }

        public override ILabelFactory LabelFactory()
        {
            return WordFactoryHolder.lf;
        }

        public static ILabelFactory Factory()
        {
            return WordFactoryHolder.lf;
        }
    }
}
