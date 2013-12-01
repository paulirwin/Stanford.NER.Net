using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stanford.NER.Net.Ling
{
    public class WordLemmaTagFactory : ILabelFactory
    {
        public static readonly int LEMMA_LABEL = 1;
        public static readonly int TAG_LABEL = 2;
        private readonly char divider;

        public WordLemmaTagFactory()
            : this('/')
        {
        }

        public WordLemmaTagFactory(char divider)
        {
            this.divider = divider;
        }

        public virtual ILabel NewLabel(string labelStr)
        {
            return new WordLemmaTag(labelStr);
        }

        public virtual ILabel NewLabel(string labelStr, int options)
        {
            if (options == TAG_LABEL)
            {
                return new WordLemmaTag(null, null, labelStr);
            }
            else if (options == LEMMA_LABEL)
            {
                return new WordLemmaTag(null, labelStr, null);
            }
            else
            {
                return new WordLemmaTag(labelStr);
            }
        }

        public virtual ILabel NewLabelFromString(string labelStr)
        {
            int first = labelStr.IndexOf(divider);
            int second = labelStr.LastIndexOf(divider);
            if (first == second)
            {
                return new WordLemmaTag(labelStr.Substring(0, first), Morphology.StemStatic(labelStr.Substring(0, first), labelStr.Substring(first + 1)).Word(), labelStr.Substring(first + 1));
            }
            else if (first >= 0)
            {
                return new WordLemmaTag(labelStr.Substring(0, first), labelStr.Substring(first + 1, second), labelStr.Substring(second + 1));
            }
            else
            {
                return new WordLemmaTag(labelStr);
            }
        }

        public virtual ILabel NewLabel(ILabel oldLabel)
        {
            return new WordLemmaTag(oldLabel);
        }
    }
}
