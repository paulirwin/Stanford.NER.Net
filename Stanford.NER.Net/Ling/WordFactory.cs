using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Stanford.NER.Net.Ling
{
    public class WordFactory : ILabelFactory
    {
        public WordFactory()
        {
        }

        public virtual ILabel NewLabel(string word)
        {
            return new Word(word);
        }

        public virtual ILabel NewLabel(string word, int options)
        {
            return new Word(word);
        }

        public virtual ILabel NewLabelFromString(string word)
        {
            return new Word(word);
        }

        public virtual ILabel NewLabel(ILabel oldLabel)
        {
            return new Word(oldLabel);
        }
    }
}
