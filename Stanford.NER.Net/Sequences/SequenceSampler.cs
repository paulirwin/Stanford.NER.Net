using Stanford.NER.Net.Math;
using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public class SequenceSampler : IBestSequenceFinder
    {
        private class TestSequenceModel : ISequenceModel
        {
            private int[] correctTags = new[]
        {
        0, 0, 1, 2, 3, 4, 5, 6, 7, 6, 5, 4, 3, 2, 1, 0, 0
        }

            ;
            private int[] allTags = new[]
        {
        1, 2, 3, 4, 5, 6, 7, 8, 9
        }

            ;
            private int[] midTags = new[]
        {
        0, 1, 2, 3
        }

            ;
            private int[] nullTags = new[]
        {
        0
        }

            ;
            public virtual int Length()
            {
                return correctTags.Length - LeftWindow() - RightWindow();
            }

            public virtual int LeftWindow()
            {
                return 2;
            }

            public virtual int RightWindow()
            {
                return 0;
            }

            public virtual int[] GetPossibleValues(int pos)
            {
                if (pos < LeftWindow() || pos >= LeftWindow() + Length())
                {
                    return nullTags;
                }

                if (correctTags[pos] < 4)
                {
                    return midTags;
                }

                return allTags;
            }

            public virtual double ScoreOf(int[] tags, int pos)
            {
                return 1.0;
            }

            public virtual double ScoreOf(int[] sequence)
            {
                throw new NotSupportedException();
            }

            public virtual double[] ScoresOf(int[] tags, int pos)
            {
                int[] tagsAtPos = GetPossibleValues(pos);
                double[] scores = new double[tagsAtPos.Length];
                Arrays.Fill(scores, 1.0);
                return scores;
            }
        }

        private static string ArrayToString(int[] x)
        {
            StringBuilder sb = new StringBuilder(@"(");
            for (int j = 0; j < x.Length; j++)
            {
                sb.Append(x[j]);
                if (j != x.Length - 1)
                {
                    sb.Append(@", ");
                }
            }

            sb.Append(@")");
            return sb.ToString();
        }

        public static void Main(String[] args)
        {
            SequenceSampler ti = new SequenceSampler();
            ISequenceModel ts = new TestSequenceModel();
            int[] bestTags = ti.BestSequence(ts);
            Console.Out.WriteLine(@"The best sequence is ... " + ArrayToString(bestTags));
        }

        public virtual int[] BestSequence(ISequenceModel ts)
        {
            int[] sample = new int[ts.Length() + ts.LeftWindow()];
            for (int pos = ts.LeftWindow(); pos < sample.Length; pos++)
            {
                double[] scores = ts.ScoresOf(sample, pos);
                double total = 0.0;
                for (int i = 0; i < scores.Length; i++)
                {
                    scores[i] = Math.Exp(scores[i]);
                }

                ArrayMath.Normalize(scores);
                int l = ArrayMath.SampleFromDistribution(scores);
                sample[pos] = ts.GetPossibleValues(pos)[l];
            }

            return sample;
        }
    }
}
