using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface ILabel
    {
        string Value();
        void SetValue(string value);
        string ToString();
        void SetFromString(string labelStr);
        ILabelFactory LabelFactory();
    }
}
