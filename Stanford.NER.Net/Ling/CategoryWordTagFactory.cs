using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class CategoryWordTagFactory : ILabelFactory
    {
        public virtual ILabel NewLabel(string labelStr)
        {
            return new CategoryWordTag(labelStr);
        }

        public virtual ILabel NewLabel(string labelStr, int options)
        {
            return new CategoryWordTag(labelStr);
        }

        public virtual ILabel NewLabelFromString(string labelStr)
        {
            CategoryWordTag cwt = new CategoryWordTag();
            cwt.SetFromString(labelStr);
            return cwt;
        }

        public virtual ILabel NewLabel(string word, string tag, string category)
        {
            return new CategoryWordTag(category, word, tag);
        }

        public virtual ILabel NewLabel(ILabel oldLabel)
        {
            return new CategoryWordTag(oldLabel);
        }
    }
}
