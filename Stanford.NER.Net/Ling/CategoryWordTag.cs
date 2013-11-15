using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class CategoryWordTag : StringLabel, IHasCategory, IHasWord, IHasTag
    {
        protected string word;
        protected string tag;
        public static bool printWordTag = true;
        public static bool suppressTerminalDetails;
        public CategoryWordTag()
            : base()
        {
        }

        public CategoryWordTag(string label)
            : base(label)
        {
        }

        public CategoryWordTag(string category, string word, string tag)
            : base(category)
        {
            this.word = word;
            this.tag = tag;
        }

        public CategoryWordTag(ILabel oldLabel)
            : base(oldLabel)
        {
            if (oldLabel is IHasTag)
            {
                this.tag = ((IHasTag)oldLabel).Tag();
            }

            if (oldLabel is IHasWord)
            {
                this.word = ((IHasWord)oldLabel).Word();
            }
        }

        public virtual string Category()
        {
            return Value();
        }

        public virtual void SetCategory(string category)
        {
            SetValue(category);
        }

        public virtual string Word()
        {
            return word;
        }

        public virtual void SetWord(string word)
        {
            this.word = word;
        }

        public virtual string Tag()
        {
            return tag;
        }

        public virtual void SetTag(string tag)
        {
            this.tag = tag;
        }

        public virtual void SetCategoryWordTag(string category, string word, string tag)
        {
            SetCategory(category);
            SetWord(word);
            SetTag(tag);
        }

        public override string ToString()
        {
            if (Category() != null)
            {
                if ((Word() == null || Tag() == null) || !printWordTag || (suppressTerminalDetails && (Word().Equals(Category()) || Tag().Equals(Category()))))
                {
                    return Category();
                }
                else
                {
                    return Category() + @"[" + Word() + @"/" + Tag() + @"]";
                }
            }
            else
            {
                if (Tag() == null)
                {
                    return Word();
                }
                else
                {
                    return Word() + @"/" + Tag();
                }
            }
        }

        public virtual string ToString(string mode)
        {
            if (@"full".Equals(mode))
            {
                return Category() + @"[" + Word() + @"/" + Tag() + @"]";
            }

            return ToString();
        }

        public override void SetFromString(string labelStr)
        {
            throw new NotSupportedException();
        }

        private class LabelFactoryHolder
        {
            private LabelFactoryHolder()
            {
            }

            internal static readonly ILabelFactory lf = new CategoryWordTagFactory();
        }

        public override ILabelFactory LabelFactory()
        {
            return LabelFactoryHolder.lf;
        }

        public static ILabelFactory Factory()
        {
            return LabelFactoryHolder.lf;
        }
    }
}
