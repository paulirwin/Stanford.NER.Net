using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public interface IBestSequenceFinder
    {
        int[] BestSequence(ISequenceModel sequenceModel);
    }
}
