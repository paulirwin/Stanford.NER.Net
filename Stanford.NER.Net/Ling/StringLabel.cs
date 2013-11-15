using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class StringLabel : ValueLabel, IHasOffset
    {
        private string str;
        private int beginPosition = -1;
        private int endPosition = -1;

        public StringLabel()
        {
        }

        public StringLabel(string str)
        {
            this.str = str;
        }

        public StringLabel(string str, int beginPosition, int endPosition)
        {
            this.str = str;
            SetBeginPosition(beginPosition);
            SetEndPosition(endPosition);
        }

        public StringLabel(ILabel label)
        {
            this.str = label.Value();
            if (label is IHasOffset)
            {
                IHasOffset ofs = (IHasOffset)label;
                SetBeginPosition(ofs.BeginPosition());
                SetEndPosition(ofs.EndPosition());
            }
        }

        public override string Value()
        {
            return str;
        }

        public override void SetValue(string value)
        {
            str = value;
        }

        public override void SetFromString(string str)
        {
            this.str = str;
        }

        public override string ToString()
        {
            return str;
        }

        private class StringLabelFactoryHolder
        {
            private StringLabelFactoryHolder()
            {
            }

            internal static readonly ILabelFactory lf = new StringLabelFactory();
        }

        public override ILabelFactory LabelFactory()
        {
            return StringLabelFactoryHolder.lf;
        }

        public static ILabelFactory Factory()
        {
            return StringLabelFactoryHolder.lf;
        }

        public virtual int BeginPosition()
        {
            return beginPosition;
        }

        public virtual int EndPosition()
        {
            return endPosition;
        }

        public virtual void SetBeginPosition(int beginPosition)
        {
            this.beginPosition = beginPosition;
        }

        public virtual void SetEndPosition(int endPosition)
        {
            this.endPosition = endPosition;
        }
    }
}
