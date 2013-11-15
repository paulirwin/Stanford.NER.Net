using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stanford.NER.Net.Ling
{
    public class TaggedWordFactory : ILabelFactory
    {
        public static readonly int TAG_LABEL = 2;
        private readonly char divider;
        public TaggedWordFactory()
            : this('/')
        {
        }

        public TaggedWordFactory(char divider)
        {
            this.divider = divider;
        }

        public virtual ILabel NewLabel(string labelStr)
        {
            return new TaggedWord(labelStr);
        }

        public virtual ILabel NewLabel(string labelStr, int options)
        {
            if (options == TAG_LABEL)
            {
                return new TaggedWord(null, labelStr);
            }

            return new TaggedWord(labelStr);
        }

        public virtual ILabel NewLabelFromString(string word)
        {
            int where = word.LastIndexOf(divider);
            if (where >= 0)
            {
                return new TaggedWord(word.Substring(0, where), word.Substring(where + 1));
            }
            else
            {
                return new TaggedWord(word);
            }
        }

        public virtual ILabel NewLabel(ILabel oldLabel)
        {
            return new TaggedWord(oldLabel);
        }
    }
}
