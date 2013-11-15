using Stanford.NER.Net.Ling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public class CRFDatum<FEAT, LAB>
    {
        private readonly List<FEAT> features;
        private readonly LAB label;
        private readonly List<double[]> featureVals;

        public CRFDatum(List<FEAT> features, LAB label, List<double[]> featureVals)
        {
            this.features = features;
            this.label = label;
            this.featureVals = featureVals;
        }

        public virtual List<FEAT> AsFeatures()
        {
            return features;
        }

        public virtual List<double[]> AsFeatureVals()
        {
            return featureVals;
        }

        public virtual LAB Label()
        {
            return label;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder(@"CRFDatum[\n");
            sb.Append(@"    label=").Append(label).Append('\n');
            for (int i = 0, sz = features.Count; i < sz; i++)
            {
                sb.Append(@"    features(").Append(i).Append(@"):").Append(features[i]);
                sb.Append(@", val=").Append(featureVals[i]);
                sb.Append('\n');
            }

            sb.Append(']');
            return sb.ToString();
        }

        public override bool Equals(Object o)
        {
            if (!(o is IDatum))
            {
                return (false);
            }

            IDatum<LAB,FEAT> d = (IDatum<LAB,FEAT>)o;
            return features.Equals(d.AsFeatures());
        }

        public override int GetHashCode()
        {
            return features.GetHashCode();
        }

        //private static readonly long serialVersionUID = -@"8345554365027671190L";
    }
}
