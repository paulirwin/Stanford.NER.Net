using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public abstract class ValueLabel : ILabel, IComparable<ValueLabel>
    {
        protected ValueLabel()
        {
        }

        public virtual string Value()
        {
            return null;
        }

        public virtual void SetValue(string value)
        {
        }

        public override string ToString()
        {
            string val = Value();
            return (val == null) ? @"" : val;
        }

        public virtual void SetFromString(string labelStr)
        {
            throw new NotSupportedException();
        }

        public override bool Equals(Object obj)
        {
            string val = Value();
            return (obj is ValueLabel) && (val == null ? ((ILabel)obj).Value() == null : val.Equals(((ILabel)obj).Value()));
        }

        public override int GetHashCode()
        {
            string val = Value();
            return val == null ? 3 : val.GetHashCode();
        }

        public virtual int CompareTo(ValueLabel valueLabel)
        {
            return Value().CompareTo(valueLabel.Value());
        }

        public abstract ILabelFactory LabelFactory();
    }
}
