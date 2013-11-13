using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface IHasContext
    {
        string Before();
        void SetBefore(string before);
        string OriginalText();
        void SetOriginalText(string originalText);
        string After();
        void SetAfter(string after);
    }
}
