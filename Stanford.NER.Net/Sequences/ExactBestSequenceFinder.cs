using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public class ExactBestSequenceFinder : IBestSequenceFinder
    {
        private static readonly bool useOld = false;
        private static readonly bool DEBUG = false;
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

            public virtual double ScoreOf(int[] tags, int pos)
            {
                bool match = true;
                for (int loc = pos - LeftWindow(); loc <= pos + RightWindow(); loc++)
                {
                    if (tags[loc] != correctTags[loc])
                    {
                        match = false;
                    }
                }

                if (match)
                {
                    return pos;
                }

                return 0;
            }

            public virtual double ScoreOf(int[] sequence)
            {
                throw new NotSupportedException();
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
            IBestSequenceFinder ti = new ExactBestSequenceFinder();
            ISequenceModel ts = new TestSequenceModel();
            int[] bestTags = ti.BestSequence(ts);
            Console.Out.WriteLine(@"The best sequence is ... " + Arrays.ToString(bestTags));
        }

        public virtual int[] BestSequence(ISequenceModel ts)
        {
            if (useOld)
            {
                return BestSequenceOld(ts);
            }
            else
            {
                return BestSequenceNew(ts);
            }
        }

        public static Pair<int[], Double> BestSequenceWithLinearConstraints(ISequenceModel ts, double[][] linearConstraints)
        {
            return BestSequenceNew(ts, linearConstraints);
        }

        private static int[] BestSequenceNew(ISequenceModel ts)
        {
            return BestSequenceNew(ts, null).First();
        }

        private static Pair<int[], Double> BestSequenceNew(ISequenceModel ts, double[][] linearConstraints)
        {
            int length = ts.Length();
            int leftWindow = ts.LeftWindow();
            int rightWindow = ts.RightWindow();
            int padLength = length + leftWindow + rightWindow;
            if (linearConstraints != null && linearConstraints.Length != padLength)
                throw new Exception(@"linearConstraints.length (" + linearConstraints.Length + @") does not match padLength (" + padLength + @") of SequenceModel" + @", length==" + length + @", leftW=" + leftWindow + @", rightW=" + rightWindow);
            int[][] tags = new int[padLength][];
            int[] tagNum = new int[padLength];
            if (DEBUG)
            {
                Console.Error.WriteLine(@"Doing bestSequence length " + length + @"; leftWin " + leftWindow + @"; rightWin " + rightWindow + @"; padLength " + padLength);
            }

            for (int pos = 0; pos < padLength; pos++)
            {
                tags[pos] = ts.GetPossibleValues(pos);
                tagNum[pos] = tags[pos].Length;
                if (DEBUG)
                {
                    Console.Error.WriteLine(@"There are " + tagNum[pos] + @" values at position " + pos + @": " + Arrays.ToString(tags[pos]));
                }
            }

            int[] tempTags = new int[padLength];
            int[] productSizes = new int[padLength];
            int curProduct = 1;
            for (int i = 0; i < leftWindow + rightWindow; i++)
            {
                curProduct *= tagNum[i];
            }

            for (int pos = leftWindow + rightWindow; pos < padLength; pos++)
            {
                if (pos > leftWindow + rightWindow)
                {
                    curProduct /= tagNum[pos - leftWindow - rightWindow - 1];
                }

                curProduct *= tagNum[pos];
                productSizes[pos - rightWindow] = curProduct;
            }

            double[][] windowScore = new double[padLength][];
            for (int pos = leftWindow; pos < leftWindow + length; pos++)
            {
                if (DEBUG)
                {
                    Console.Error.WriteLine(@"scoring word " + pos + @" / " + (leftWindow + length) + @", productSizes =  " + productSizes[pos] + @", tagNum = " + tagNum[pos] + @"...");
                }

                windowScore[pos] = new double[productSizes[pos]];
                Arrays.Fill(tempTags, tags[0][0]);
                if (DEBUG)
                {
                    Console.Error.WriteLine(@"windowScore[" + pos + @"] has size (productSizes[pos]) " + windowScore[pos].Length);
                }

                for (int product = 0; product < productSizes[pos]; product++)
                {
                    int p = product;
                    int shift = 1;
                    for (int curPos = pos + rightWindow; curPos >= pos - leftWindow; curPos--)
                    {
                        tempTags[curPos] = tags[curPos][p % tagNum[curPos]];
                        p /= tagNum[curPos];
                        if (curPos > pos)
                        {
                            shift *= tagNum[curPos];
                        }
                    }

                    if (tempTags[pos] == tags[pos][0])
                    {
                        double[] scores = ts.ScoresOf(tempTags, pos);
                        if (DEBUG)
                        {
                            Console.Error.WriteLine(@"Matched at array index [product] " + product + @"; tempTags[pos] == tags[pos][0] == " + tempTags[pos]);
                        }

                        if (DEBUG)
                        {
                            Console.Error.WriteLine(@"For pos " + pos + @" scores.length is " + scores.Length + @"; tagNum[pos] = " + tagNum[pos] + @"; windowScore[pos].length = " + windowScore[pos].Length);
                        }

                        if (DEBUG)
                        {
                            Console.Error.WriteLine(@"scores: " + Arrays.ToString(scores));
                        }

                        for (int t = 0; t < tagNum[pos]; t++)
                        {
                            if (DEBUG)
                            {
                                Console.Error.WriteLine(@"Setting value of windowScore[" + pos + @"][" + product + @"+" + t + @"*" + shift + @"] = " + scores[t]);
                            }

                            windowScore[pos][product + t * shift] = scores[t];
                        }
                    }
                }
            }

            double[][] score = new double[padLength][];
            int[][] trace = new int[padLength][];
            for (int pos = 0; pos < padLength; pos++)
            {
                score[pos] = new double[productSizes[pos]];
                trace[pos] = new int[productSizes[pos]];
            }

            for (int pos = leftWindow; pos < length + leftWindow; pos++)
            {
                for (int product = 0; product < productSizes[pos]; product++)
                {
                    if (pos == leftWindow)
                    {
                        score[pos][product] = windowScore[pos][product];
                        if (linearConstraints != null)
                        {
                            if (DEBUG && linearConstraints[pos][product % tagNum[pos]] != 0)
                                Console.Error.WriteLine(@"Applying linear constraints=" + linearConstraints[pos][product % tagNum[pos]] + @" to preScore=" + windowScore[pos][product] + @" at pos=" + pos + @" for tag=" + (product % tagNum[pos]));
                            score[pos][product] += linearConstraints[pos][product % tagNum[pos]];
                        }

                        trace[pos][product] = -1;
                    }
                    else
                    {
                        score[pos][product] = Double.NegativeInfinity;
                        trace[pos][product] = -1;
                        int sharedProduct = product / tagNum[pos + rightWindow];
                        int factor = productSizes[pos] / tagNum[pos + rightWindow];
                        for (int newTagNum = 0; newTagNum < tagNum[pos - leftWindow - 1]; newTagNum++)
                        {
                            int predProduct = newTagNum * factor + sharedProduct;
                            double predScore = score[pos - 1][predProduct] + windowScore[pos][product];
                            if (linearConstraints != null)
                            {
                                if (DEBUG && pos == 2 && linearConstraints[pos][product % tagNum[pos]] != 0)
                                {
                                    Console.Error.WriteLine(@"Applying linear constraints=" + linearConstraints[pos][product % tagNum[pos]] + @" to preScore=" + predScore + @" at pos=" + pos + @" for tag=" + (product % tagNum[pos]));
                                    Console.Error.WriteLine(@"predScore:" + predScore + @" = score[" + (pos - 1) + @"][" + predProduct + @"]:" + score[pos - 1][predProduct] + @" + windowScore[" + pos + @"][" + product + @"]:" + windowScore[pos][product]);
                                }

                                predScore += linearConstraints[pos][product % tagNum[pos]];
                            }

                            if (predScore > score[pos][product])
                            {
                                score[pos][product] = predScore;
                                trace[pos][product] = predProduct;
                            }
                        }
                    }
                }
            }

            double bestFinalScore = Double.NegativeInfinity;
            int bestCurrentProduct = -1;
            for (int product = 0; product < productSizes[leftWindow + length - 1]; product++)
            {
                if (score[leftWindow + length - 1][product] > bestFinalScore)
                {
                    bestCurrentProduct = product;
                    bestFinalScore = score[leftWindow + length - 1][product];
                }
            }

            int lastProduct = bestCurrentProduct;
            for (int last = padLength - 1; last >= length - 1 && last >= 0; last--)
            {
                tempTags[last] = tags[last][lastProduct % tagNum[last]];
                lastProduct /= tagNum[last];
            }

            for (int pos = leftWindow + length - 2; pos >= leftWindow; pos--)
            {
                int bestNextProduct = bestCurrentProduct;
                bestCurrentProduct = trace[pos + 1][bestNextProduct];
                tempTags[pos - leftWindow] = tags[pos - leftWindow][bestCurrentProduct / (productSizes[pos] / tagNum[pos - leftWindow])];
            }

            return new Pair<int[], Double>(tempTags, bestFinalScore);
        }

        private static int[] BestSequenceOld(ISequenceModel ts)
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

            int[] tempTags = new int[padLength];
            int[] productSizes = new int[padLength];
            int curProduct = 1;
            for (int i = 0; i < leftWindow + rightWindow; i++)
                curProduct *= tagNum[i];
            for (int pos = leftWindow + rightWindow; pos < padLength; pos++)
            {
                if (pos > leftWindow + rightWindow)
                    curProduct /= tagNum[pos - leftWindow - rightWindow - 1];
                curProduct *= tagNum[pos];
                productSizes[pos - rightWindow] = curProduct;
            }

            double[][] windowScore = new double[padLength][];
            for (int pos = leftWindow; pos < leftWindow + length; pos++)
            {
                windowScore[pos] = new double[productSizes[pos]];
                Arrays.Fill(tempTags, tags[0][0]);
                for (int product = 0; product < productSizes[pos]; product++)
                {
                    int p = product;
                    for (int curPos = pos + rightWindow; curPos >= pos - leftWindow; curPos--)
                    {
                        tempTags[curPos] = tags[curPos][p % tagNum[curPos]];
                        p /= tagNum[curPos];
                    }

                    windowScore[pos][product] = ts.ScoreOf(tempTags, pos);
                }
            }

            double[][] score = new double[padLength][];
            int[][] trace = new int[padLength][];
            for (int pos = 0; pos < padLength; pos++)
            {
                score[pos] = new double[productSizes[pos]];
                trace[pos] = new int[productSizes[pos]];
            }

            for (int pos = leftWindow; pos < length + leftWindow; pos++)
            {
                for (int product = 0; product < productSizes[pos]; product++)
                {
                    if (pos == leftWindow)
                    {
                        score[pos][product] = windowScore[pos][product];
                        trace[pos][product] = -1;
                    }
                    else
                    {
                        score[pos][product] = Double.NegativeInfinity;
                        trace[pos][product] = -1;
                        int sharedProduct = product / tagNum[pos + rightWindow];
                        int factor = productSizes[pos] / tagNum[pos + rightWindow];
                        for (int newTagNum = 0; newTagNum < tagNum[pos - leftWindow - 1]; newTagNum++)
                        {
                            int predProduct = newTagNum * factor + sharedProduct;
                            double predScore = score[pos - 1][predProduct] + windowScore[pos][product];
                            if (predScore > score[pos][product])
                            {
                                score[pos][product] = predScore;
                                trace[pos][product] = predProduct;
                            }
                        }
                    }
                }
            }

            double bestFinalScore = Double.NegativeInfinity;
            int bestCurrentProduct = -1;
            for (int product = 0; product < productSizes[leftWindow + length - 1]; product++)
            {
                if (score[leftWindow + length - 1][product] > bestFinalScore)
                {
                    bestCurrentProduct = product;
                    bestFinalScore = score[leftWindow + length - 1][product];
                }
            }

            int lastProduct = bestCurrentProduct;
            for (int last = padLength - 1; last >= length - 1; last--)
            {
                tempTags[last] = tags[last][lastProduct % tagNum[last]];
                lastProduct /= tagNum[last];
            }

            for (int pos = leftWindow + length - 2; pos >= leftWindow; pos--)
            {
                int bestNextProduct = bestCurrentProduct;
                bestCurrentProduct = trace[pos + 1][bestNextProduct];
                tempTags[pos - leftWindow] = tags[pos - leftWindow][bestCurrentProduct / (productSizes[pos] / tagNum[pos - leftWindow])];
            }

            return tempTags;
        }
    }
}
