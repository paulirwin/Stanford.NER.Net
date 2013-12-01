using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stanford.NER.Net.Ling
{
    public class WordTagFactory : ILabelFactory
    {
        private readonly char divider;
        public WordTagFactory()
            : this('/')
        {
        }

        public WordTagFactory(char divider)
        {
            this.divider = divider;
        }

        public virtual ILabel NewLabel(string labelStr)
        {
            return new WordTag(labelStr);
        }

        public virtual ILabel NewLabel(string labelStr, int options)
        {
            if (options == TaggedWordFactory.TAG_LABEL)
            {
                return new WordTag(null, labelStr);
            }
            else
            {
                return new WordTag(labelStr);
            }
        }

        public virtual ILabel NewLabelFromString(string word)
        {
            int where = word.LastIndexOf(divider);
            if (where >= 0)
            {
                return new WordTag(word.Substring(0, where), word.Substring(where + 1));
            }
            else
            {
                return new WordTag(word);
            }
        }

        public virtual ILabel NewLabel(ILabel oldLabel)
        {
            if (oldLabel is IHasTag)
            {
                return new WordTag(oldLabel.Value(), ((IHasTag)oldLabel).Tag());
            }
            else
            {
                return new WordTag(oldLabel.Value());
            }
        }
    }
}
