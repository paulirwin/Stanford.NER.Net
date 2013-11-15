using Stanford.NER.Net.Ling;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Classify
{
    public abstract class AbstractLinearClassifierFactory<L, F> : IClassifierFactory<L, F, IClassifier<L, F>>
    {
        IIndex<L> labelIndex = new HashIndex<L>();
        IIndex<F> featureIndex = new HashIndex<F>();

        public AbstractLinearClassifierFactory()
        {
        }

        virtual int NumFeatures()
        {
            return featureIndex.Size();
        }

        virtual int NumClasses()
        {
            return labelIndex.Size();
        }

        public virtual IClassifier<L, F> TrainClassifier(IList<RVFDatum<L, F>> examples)
        {
            Dataset<L, F> dataset = new Dataset<L, F>();
            dataset.AddAll(examples);
            return TrainClassifier(dataset);
        }

        protected abstract double[][] TrainWeights(GeneralDataset<L, F> dataset);
        public virtual LinearClassifier<L, F> TrainClassifier(ICollection<IDatum<L, F>> examples)
        {
            Dataset<L, F> dataset = new Dataset<L, F>();
            dataset.AddAll(examples);
            return TrainClassifier(dataset);
        }

        public virtual LinearClassifier<L, F> TrainClassifier(Reference<ICollection<IDatum<L, F>>> ref_renamed)
        {
            ICollection<IDatum<L, F>> examples = ref_renamed.Get();
            return TrainClassifier(examples);
        }

        public virtual LinearClassifier<L, F> TrainClassifier(GeneralDataset<L, F> data)
        {
            labelIndex = data.LabelIndex();
            featureIndex = data.FeatureIndex();
            double[][] weights = TrainWeights(data);
            return new LinearClassifier<L, F>(weights, featureIndex, labelIndex);
        }
    }
}
