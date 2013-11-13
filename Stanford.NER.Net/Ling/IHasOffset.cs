using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface IHasOffset
    {
        int BeginPosition();
        void SetBeginPosition(int beginPos);
        int EndPosition();
        void SetEndPosition(int endPos);
    }
}
