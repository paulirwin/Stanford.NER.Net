using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public class BeamBestSequenceFinder : IBestSequenceFinder
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
                return 2;
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

            public virtual double ScoreOf(int[] sequence)
            {
                throw new NotSupportedException();
            }

            public virtual double ScoreOf(int[] tags, int pos)
            {
                bool match = true;
                bool ones = true;
                for (int loc = pos - LeftWindow(); loc <= pos + RightWindow(); loc++)
                {
                    if (tags[loc] != correctTags[loc])
                    {
                        match = false;
                    }

                    if (tags[loc] != 1 && loc >= LeftWindow() && loc < Length() + LeftWindow())
                    {
                        ones = false;
                    }
                }

                if (match)
                {
                    return pos;
                }

                if (ones)
                {
                    return 0;
                }

                return 0;
            }

            public virtual double[] ScoresOf(int[] tags, int pos)
            {
                int[] tagsAtPos = GetPossibleValues(pos);
                double[] scores = new double[tagsAtPos.Length];
                for (int t = 0; t < tagsAtPos.Length; t++)
                {
                    tags[pos] = tagsAtPos[t];
                    scores[t] = ScoreOf(tags, pos);
                }

                return scores;
            }
        }

        public static void Main(String[] args)
        {
            IBestSequenceFinder ti = new BeamBestSequenceFinder(4, true);
            ISequenceModel ts = new TestSequenceModel();
            int[] bestTags = ti.BestSequence(ts);
            Console.Out.WriteLine(@"The best sequence is .... " + Arrays.ToString(bestTags));
        }

        private static int[] tmp = null;
        private class TagSeq : IScored
        {
            private class TagList
            {
                internal int tag = -1;
                internal TagList last = null;
            }

            internal double score = 0.0;
            public virtual double Score()
            {
                return score;
            }

            private int size = 0;
            public virtual int Size()
            {
                return size;
            }

            private TagList info = null;
            public virtual int[] TmpTags(int count, int s)
            {
                if (tmp == null || tmp.Length < s)
                {
                    tmp = new int[s];
                }

                TagList tl = info;
                int i = Size() - 1;
                while (tl != null && count >= 0)
                {
                    tmp[i] = tl.tag;
                    i--;
                    count--;
                    tl = tl.last;
                }

                return tmp;
            }

            public virtual int[] Tags()
            {
                int[] t = new int[Size()];
                int i = Size() - 1;
                for (TagList tl = info; tl != null; tl = tl.last)
                {
                    t[i] = tl.tag;
                    i--;
                }

                return t;
            }

            public virtual void ExtendWith(int tag)
            {
                TagList last = info;
                info = new TagList();
                info.tag = tag;
                info.last = last;
                size++;
            }

            public virtual void ExtendWith(int tag, ISequenceModel ts, int s)
            {
                ExtendWith(tag);
                int[] tags = TmpTags(ts.LeftWindow() + 1 + ts.RightWindow(), s);
                score += ts.ScoreOf(tags, Size() - ts.RightWindow() - 1);
            }

            public virtual TagSeq Tclone()
            {
                TagSeq o = new TagSeq();
                o.info = info;
                o.size = size;
                o.score = score;
                return o;
            }
        }

        private int beamSize;
        private bool exhaustiveStart;
        private bool recenter = true;
        public virtual int[] BestSequence(ISequenceModel ts)
        {
            return BestSequence(ts, (1024 * 128));
        }

        public virtual int[] BestSequence(ISequenceModel ts, int size)
        {
            int length = ts.Length();
            int leftWindow = ts.LeftWindow();
            int rightWindow = ts.RightWindow();
            int padLength = length + leftWindow + rightWindow;
            int[][] tags = new int[padLength][];
            int[] tagNum = new int[padLength];
            for (int pos = 0; pos < padLength; pos++)
            {
                tags[pos] = ts.GetPossibleValues(pos);
                tagNum[pos] = tags[pos].Length;
            }

            var newBeam = new Beam<TagSeq>(beamSize, ScoredComparator.ASCENDING_COMPARATOR);
            TagSeq initSeq = new TagSeq();
            newBeam.Add(initSeq);
            for (int pos = 0; pos < padLength; pos++)
            {
                var oldBeam = newBeam;
                if (pos < leftWindow + rightWindow && exhaustiveStart)
                {
                    newBeam = new Beam<TagSeq>(100000, ScoredComparator.ASCENDING_COMPARATOR);
                }
                else
                {
                    newBeam = new Beam<TagSeq>(beamSize, ScoredComparator.ASCENDING_COMPARATOR);
                }

                for (IEnumerator beamI = oldBeam.GetEnumerator(); beamI.MoveNext(); )
                {
                    Console.Out.Write(@"#");
                    Console.Out.Flush();
                    TagSeq tagSeq = (TagSeq)beamI.Current;
                    for (int nextTagNum = 0; nextTagNum < tagNum[pos]; nextTagNum++)
                    {
                        TagSeq nextSeq = tagSeq.Tclone();
                        if (pos >= leftWindow + rightWindow)
                        {
                            nextSeq.ExtendWith(tags[pos][nextTagNum], ts, size);
                        }
                        else
                        {
                            nextSeq.ExtendWith(tags[pos][nextTagNum]);
                        }

                        newBeam.Add(nextSeq);
                    }
                }

                Console.Out.WriteLine(@" done");
                if (recenter)
                {
                    double max = Double.NegativeInfinity;
                    for (IEnumerator beamI = newBeam.GetEnumerator(); beamI.MoveNext(); )
                    {
                        TagSeq tagSeq = (TagSeq)beamI.Current;
                        if (tagSeq.score > max)
                        {
                            max = tagSeq.score;
                        }
                    }

                    for (IEnumerator beamI = newBeam.GetEnumerator(); beamI.MoveNext(); )
                    {
                        TagSeq tagSeq = (TagSeq)beamI.Current;
                        tagSeq.score -= max;
                    }
                }
            }

            try
            {
                var enumerator = newBeam.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    TagSeq bestSeq = enumerator.Current;
                    int[] seq = bestSeq.Tags();
                    return seq;
                }
                else
                    return null;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"Beam empty -- no best sequence.");
                return null;
            }
        }

        public BeamBestSequenceFinder(int beamSize)
            : this(beamSize, false, false)
        {
        }

        public BeamBestSequenceFinder(int beamSize, bool exhaustiveStart)
            : this(beamSize, exhaustiveStart, false)
        {
        }

        public BeamBestSequenceFinder(int beamSize, bool exhaustiveStart, bool recenter)
        {
            this.exhaustiveStart = exhaustiveStart;
            this.beamSize = beamSize;
            this.recenter = recenter;
        }
    }
}
