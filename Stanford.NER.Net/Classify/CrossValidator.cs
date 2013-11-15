using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public class CrossValidator<L, F>
    {
        private readonly GeneralDataset<L, F> originalTrainData;
        private readonly int kFold;
        private readonly SavedState[] savedStates;
        public CrossValidator(GeneralDataset<L, F> trainData)
            : this(trainData, 10)
        {
        }

        public CrossValidator(GeneralDataset<L, F> trainData, int kFold)
        {
            originalTrainData = trainData;
            this.kFold = kFold;
            savedStates = new SavedState[kFold];
            for (int i = 0; i < savedStates.Length; i++)
            {
                savedStates[i] = new SavedState();
            }
        }

        private IEnumerator<Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState>> Iterator()
        {
            return new CrossValidationIterator(this);
        }

        public virtual double ComputeAverage(IFunction<Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState>, Double> function)
        {
            double sum = 0;
            IEnumerator<Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState>> foldIt = Iterator();
            while (foldIt.MoveNext())
            {
                sum += function.Apply(foldIt.Current);
            }

            return sum / kFold;
        }

        class CrossValidationIterator : IEnumerator<Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState>>
        {
            public CrossValidationIterator(CrossValidator<L, F> parent)
            {
                this.parent = parent;
            }

            private readonly CrossValidator<L, F> parent;
            int iter = 0;
            Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState> current;

            public virtual bool MoveNext()
            {
                if (iter < parent.kFold)
                {
                    int start = parent.originalTrainData.Size() * iter / parent.kFold;
                    int end = parent.originalTrainData.Size() * (iter + 1) / parent.kFold;
                    Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>> split = parent.originalTrainData.Split(start, end);
                    current = new Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState>(split.Item1, split.Item2, parent.savedStates[iter++]);
                    return true;
                }

                return false;
            }
            
            public virtual Tuple<GeneralDataset<L, F>, GeneralDataset<L, F>, SavedState> Current
            {
                get
                {
                    return current;
                }
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public void Reset()
            {
            }
        }

        public class SavedState
        {
            public Object state;
        }

        public static void Main(String[] args)
        {
            Dataset<String, String> d = Dataset.ReadSVMLightFormat(args[0]);
            IEnumerator<Tuple<GeneralDataset<String, String>, GeneralDataset<String, String>, SavedState>> it = (new CrossValidator<String, String>(d)).Iterator();
            while (it.MoveNext())
            {
                var throwaway = it.Current;
                break;
            }
        }
    }
}
