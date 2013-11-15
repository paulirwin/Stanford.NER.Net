using Stanford.NER.Net.Stats;
using Stanford.NER.Net.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class RVFDatum<L, F> : IDatum<L, F>
    {
        private readonly ICounter<F> features;
        private L label;

        public RVFDatum(ICounter<F> features, L label)
        {
            this.features = features;
            SetLabel(label);
        }

        public RVFDatum(IDatum<L, F> m)
        {
            this.features = new ClassicCounter<F>();
            foreach (F key in m.AsFeatures())
            {
                features.IncrementCount(key, 1.0);
            }

            SetLabel(m.Label());
        }

        public RVFDatum(ICounter<F> features)
        {
            this.features = features;
        }

        public RVFDatum()
            : this((ClassicCounter<F>)null)
        {
        }

        public virtual ICounter<F> AsFeaturesCounter()
        {
            return features;
        }

        public virtual ICollection<F> AsFeatures()
        {
            return features.KeySet();
        }

        public virtual void SetLabel(L label)
        {
            this.label = label;
        }

        public override string ToString()
        {
            return @"RVFDatum[features=" + AsFeaturesCounter() + @",label=" + Label() + @"]";
        }

        public virtual L Label()
        {
            return label;
        }

        public virtual ICollection<L> Labels()
        {
            return Collections.SingletonList(label);
        }

        public virtual double GetFeatureCount(F feature)
        {
            return features.GetCount(feature);
        }

        public override bool Equals(Object o)
        {
            if (this == o)
            {
                return true;
            }

            if (!(o is RVFDatum<L, F>))
            {
                return (false);
            }

            RVFDatum<L, F> d = (RVFDatum<L, F>)o;
            return features.Equals(d.AsFeaturesCounter());
        }

        public override int GetHashCode()
        {
            return features.GetHashCode();
        }
    }
}
