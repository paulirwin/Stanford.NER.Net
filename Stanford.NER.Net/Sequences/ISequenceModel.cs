using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public interface ISequenceModel
    {
        int Length();
        int LeftWindow();
        int RightWindow();
        int[] GetPossibleValues(int position);
        double[] ScoresOf(int[] sequence, int position);
        double ScoreOf(int[] sequence, int position);
        double ScoreOf(int[] sequence);
    }
}
