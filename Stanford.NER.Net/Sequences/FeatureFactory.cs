using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public abstract class FeatureFactory<IN>
    {
        //private static readonly long serialVersionUID = @"7249250071983091694L";
        protected SeqClassifierFlags flags;
        public FeatureFactory()
        {
        }

        public virtual void Init(SeqClassifierFlags flags)
        {
            this.flags = flags;
        }

        public static readonly Clique cliqueC = Clique.ValueOf(new int[] { 0 } );
        public static readonly Clique cliqueCpC = Clique.ValueOf(new int[] { -1, 0 });
        public static readonly Clique cliqueCp2C = Clique.ValueOf(new int[] { -2, 0 } );
        public static readonly Clique cliqueCp3C = Clique.ValueOf(new int[] { -3, 0 } );
        public static readonly Clique cliqueCp4C = Clique.ValueOf(new int[] { -4, 0 } );
        public static readonly Clique cliqueCp5C = Clique.ValueOf(new int[] { -5, 0 } );
        public static readonly Clique cliqueCpCp2C = Clique.ValueOf(new int[] { -2, -1, 0 });
        public static readonly Clique cliqueCpCp2Cp3C = Clique.ValueOf(new int[] { -3, -2, -1, 0 } );
        public static readonly Clique cliqueCpCp2Cp3Cp4C = Clique.ValueOf(new int[] { -4, -3, -2, -1, 0 });
        public static readonly Clique cliqueCpCp2Cp3Cp4Cp5C = Clique.ValueOf(new int[] { -5, -4, -3, -2, -1, 0 });
        public static readonly Clique cliqueCnC = Clique.ValueOf(new int[] { 0, 1 });
        public static readonly Clique cliqueCpCnC = Clique.ValueOf(new int[] { -1, 0, 1 });
        public static readonly List<Clique> knownCliques = new List<Clique>() { cliqueC, cliqueCpC, cliqueCp2C, cliqueCp3C, cliqueCp4C, cliqueCp5C, cliqueCpCp2C, cliqueCpCp2Cp3C, cliqueCpCp2Cp3Cp4C, cliqueCpCp2Cp3Cp4Cp5C, cliqueCnC, cliqueCpCnC };
        
        public virtual List<Clique> GetCliques()
        {
            return GetCliques(flags.maxLeft, flags.maxRight);
        }

        public static List<Clique> GetCliques(int maxLeft, int maxRight)
        {
            List<Clique> cliques = new List<Clique>();
            foreach (Clique c in knownCliques)
            {
                if (-c.MaxLeft() <= maxLeft && c.MaxRight() <= maxRight)
                {
                    cliques.Add(c);
                }
            }

            return cliques;
        }

        public abstract ICollection<String> GetCliqueFeatures(PaddedList<IN> info, int position, Clique clique);

        protected virtual void AddAllInterningAndSuffixing(ICollection<String> accumulator, ICollection<String> addend, string suffix)
        {
            bool nonNullSuffix = suffix != null && !@"".Equals(suffix);
            if (nonNullSuffix)
            {
                suffix = '|' + suffix;
            }

            foreach (string feat in addend)
            {
                string featwritable = feat;

                if (nonNullSuffix)
                {
                    featwritable = featwritable + suffix;
                }

                accumulator.Add(featwritable);
            }
        }

        protected virtual string GetWord(CoreLabel label)
        {
            string word = label.GetString(typeof(CoreAnnotations.TextAnnotation));
            if (flags.wordFunction != null)
            {
                word = flags.wordFunction.Apply(word);
            }

            return word;
        }
    }
}
