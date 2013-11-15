using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class StringLabelFactory : ILabelFactory
    {
        public virtual ILabel NewLabel(string labelStr)
        {
            return new StringLabel(labelStr);
        }

        public virtual ILabel NewLabel(string labelStr, int options)
        {
            return new StringLabel(labelStr);
        }

        public virtual ILabel NewLabelFromString(string labelStr)
        {
            return new StringLabel(labelStr);
        }

        public virtual ILabel NewLabel(ILabel oldLabel)
        {
            return new StringLabel(oldLabel);
        }
    }
}
