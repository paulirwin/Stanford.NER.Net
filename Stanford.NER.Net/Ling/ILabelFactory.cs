using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface ILabelFactory
    {
        ILabel NewLabel(string labelStr);
        ILabel NewLabel(string labelStr, int options);
        ILabel NewLabelFromString(string encodedLabelStr);
        ILabel NewLabel(ILabel oldLabel);
    }
}
