using Stanford.NER.Net.Classify;
using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Stats
{
    public class AccuracyStats<L> : IScorer<L>
    {
        double confWeightedAccuracy;
        double accuracy;
        double optAccuracy;
        double optConfWeightedAccuracy;
        double logLikelihood;
        int[] accrecall;
        int[] optaccrecall;
        L posLabel;
        string saveFile;
        static int saveIndex = 1;

        public AccuracyStats(ProbabilisticClassifier<L, F> classifier, GeneralDataset<L, F> data, L posLabel)
        {
            this.posLabel = posLabel;
            Score(classifier, data);
        }

        public AccuracyStats(L posLabel, string saveFile)
        {
            this.posLabel = posLabel;
            this.saveFile = saveFile;
        }

        public virtual double Score(ProbabilisticClassifier<L, F> classifier, GeneralDataset<L, F> data)
        {
            List<Tuple<double, int>> dataScores = new List<Tuple<double, int>>();
            for (int i = 0; i < data.Size(); i++)
            {
                IDatum<L, F> d = data.GetRVFDatum(i);
                ICounter<L> scores = classifier.LogProbabilityOf(d);
                int labelD = d.Label().Equals(posLabel) ? 1 : 0;
                dataScores.Add(new Tuple<double, int>(System.Math.Exp(scores.GetCount(posLabel)), labelD));
            }

            PRCurve prc = new PRCurve(dataScores);
            confWeightedAccuracy = prc.Cwa();
            accuracy = prc.Accuracy();
            optAccuracy = prc.OptimalAccuracy();
            optConfWeightedAccuracy = prc.OptimalCwa();
            logLikelihood = prc.LogLikelihood();
            accrecall = prc.CwaArray();
            optaccrecall = prc.OptimalCwaArray();
            return accuracy;
        }

        public virtual string GetDescription(int numDigits)
        {
            NumberFormatInfo nf = (NumberFormatInfo)CultureInfo.CurrentCulture.NumberFormat.Clone();
            nf.NumberDecimalDigits = numDigits;
            StringBuilder sb = new StringBuilder();
            sb.Append(@"--- Accuracy Stats ---").Append('\n');
            sb.Append(@"accuracy: ").Append(accuracy.ToString(nf)).Append('\n');
            sb.Append(@"optimal fn accuracy: ").Append(optAccuracy.ToString(nf)).Append('\n');
            sb.Append(@"confidence weighted accuracy :").Append(confWeightedAccuracy.ToString(nf)).Append('\n');
            sb.Append(@"optimal confidence weighted accuracy: ").Append(optConfWeightedAccuracy.ToString(nf)).Append('\n');
            sb.Append(@"log-likelihood: ").Append(logLikelihood).Append('\n');
            if (saveFile != null)
            {
                string f = saveFile + '-' + saveIndex;
                sb.Append(@"saving accuracy info to ").Append(f).Append(@".accuracy\n");
                StringUtils.PrintToFile(f + @".accuracy", ToStringArr(accrecall));
                sb.Append(@"saving optimal accuracy info to ").Append(f).Append(@".optimal_accuracy\n");
                StringUtils.PrintToFile(f + @".optimal_accuracy", ToStringArr(optaccrecall));
                saveIndex++;
            }

            return sb.ToString();
        }

        public static string ToStringArr(int[] acc)
        {
            StringBuilder sb = new StringBuilder();
            int total = acc.Length;
            for (int i = 0; i < acc.Length; i++)
            {
                double coverage = (i + 1) / (double)total;
                double accuracy = acc[i] / (double)(i + 1);
                coverage *= 1000000;
                accuracy *= 1000000;
                sb.Append(((int)coverage) / 10000);
                sb.Append('\t');
                sb.Append(((int)accuracy) / 10000);
                sb.Append('\n');
            }

            return sb.ToString();
        }
    }
}
