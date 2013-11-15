using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class BasicDatum<LabelType, FeatureType> : IDatum<LabelType, FeatureType>
    {
        private readonly ICollection<FeatureType> features;
        private readonly List<LabelType> labels = new List<LabelType>();
        public BasicDatum(ICollection<FeatureType> features, ICollection<LabelType> labels)
            : this(features)
        {
            SetLabels(labels);
        }

        public BasicDatum(ICollection<FeatureType> features, LabelType label)
            : this(features)
        {
            SetLabel(label);
        }

        public BasicDatum(ICollection<FeatureType> features)
        {
            this.features = features;
        }

        public BasicDatum()
            : this(null)
        {
        }

        public virtual ICollection<FeatureType> AsFeatures()
        {
            return (features);
        }

        public virtual LabelType Label()
        {
            return ((labels.Size() > 0) ? labels[0] : (LabelType)null);
        }

        public virtual ICollection<LabelType> Labels()
        {
            return labels;
        }

        public virtual void SetLabel(LabelType label)
        {
            labels.Clear();
            AddLabel(label);
        }

        public virtual void SetLabels(ICollection<LabelType> labels)
        {
            this.labels.Clear();
            if (labels != null)
            {
                this.labels.AddAll(labels);
            }
        }

        public virtual void AddLabel(LabelType label)
        {
            if (label != null)
            {
                labels.Add(label);
            }
        }

        public override string ToString()
        {
            return (@"BasicDatum[features=" + AsFeatures() + @",labels=" + Labels() + @"]");
        }

        public override bool Equals(Object o)
        {
            if (!(o is IDatum))
            {
                return (false);
            }

            IDatum<LabelType, FeatureType> d = (IDatum<LabelType, FeatureType>)o;
            return features.Equals(d.AsFeatures());
        }

        public virtual int GetHashCode()
        {
            return features.GetHashCode();
        }
    }
}
