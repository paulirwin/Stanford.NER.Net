using Stanford.NER.Net.Support;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public class CoreLabel : ArrayCoreMap, ILabel, IHasWord, IHasTag, IHasCategory, IHasLemma, IHasContext, IHasIndex, IHasOffset
    {
        public CoreLabel()
            : base()
        {
        }

        public CoreLabel(int capacity)
            : base(capacity)
        {
        }

        public CoreLabel(CoreLabel label)
            : this((ICoreMap)label)
        {
        }

        public CoreLabel(ICoreMap label)
            : base(label.Size())
        {
            foreach (Type key in label.KeySet())
            {
                Set(key, label.Get<object>(key));
            }
        }

        public CoreLabel(ILabel label)
            : base(0)
        {
            if (label is ICoreMap)
            {
                ICoreMap cl = (ICoreMap)label;
                SetCapacity(cl.Size());
                foreach (Type key in cl.KeySet())
                {
                    Set(key, cl.Get<object>(key));
                }
            }
            else
            {
                if (label is IHasWord)
                {
                    SetWord(((IHasWord)label).Word());
                }

                SetValue(label.Value());
            }
        }

        public CoreLabel(String[] keys, String[] values)
            : base(keys.Length)
        {
            InitFromStrings(keys, values);
        }

        public interface IGenericAnnotation<T>
        {
        }

        public static readonly IDictionary<String, Type> genericKeys = new HashMap<string, Type>();
        public static readonly IDictionary<Type, String> genericValues = new HashMap<Type, string>();

        private void InitFromStrings(String[] keys, String[] values)
        {
            for (int i = 0; i < Math.Min(keys.Length, values.Length); i++)
            {
                string key = keys[i];
                string value = values[i];
                KeyLookup lookup = AnnotationLookup.GetCoreKey(key);
                if (lookup == null)
                {
                    if (key != null)
                    {
                        throw new NotSupportedException(@"Unknown key " + key);
                    }
                }
                else
                {
                    try
                    {
                        Type valueClass = AnnotationLookup.GetValueType(lookup.coreKey);
                        if (valueClass.Equals(typeof(string)))
                        {
                            this.Set(lookup.coreKey, values[i]);
                        }
                        else if (valueClass == typeof(int))
                        {
                            this.Set(lookup.coreKey, int.Parse(values[i]));
                        }
                        else if (valueClass == typeof(Double))
                        {
                            this.Set(lookup.coreKey, double.Parse(values[i]));
                        }
                        else if (valueClass == typeof(long))
                        {
                            this.Set(lookup.coreKey, long.Parse(values[i]));
                        }
                    }
                    catch (Exception e)
                    {
                        //e.PrintStackTrace();
                        Console.Error.WriteLine(@"CORE: CoreLabel.initFromStrings: " + @"Bad type for " + key + @". Value was: " + value + @"; expected " + AnnotationLookup.GetValueType(lookup.coreKey));
                    }
                }
            }
        }

        private class CoreLabelFactory : ILabelFactory
        {
            public override ILabel NewLabel(string labelStr)
            {
                CoreLabel label = new CoreLabel();
                label.SetValue(labelStr);
                return label;
            }

            public override ILabel NewLabel(string labelStr, int options)
            {
                return NewLabel(labelStr);
            }

            public override ILabel NewLabel(ILabel oldLabel)
            {
                if (oldLabel is CoreLabel)
                {
                    return new CoreLabel((CoreLabel)oldLabel);
                }
                else
                {
                    CoreLabel label = new CoreLabel();
                    if (oldLabel is IHasWord)
                        label.SetWord(((IHasWord)oldLabel).Word());
                    if (oldLabel is IHasTag)
                        label.SetTag(((IHasTag)oldLabel).Tag());
                    if (oldLabel is IHasOffset)
                    {
                        label.SetBeginPosition(((IHasOffset)oldLabel).BeginPosition());
                        label.SetEndPosition(((IHasOffset)oldLabel).EndPosition());
                    }

                    if (oldLabel is IHasCategory)
                        label.SetCategory(((IHasCategory)oldLabel).Category());
                    if (oldLabel is IHasIndex)
                        label.SetIndex(((IHasIndex)oldLabel).Index());
                    label.SetValue(oldLabel.Value());
                    return label;
                }
            }

            public override ILabel NewLabelFromString(string encodedLabelStr)
            {
                throw new NotSupportedException(@"This code branch left blank" + @" because we do not understand what this method should do.");
            }
        }

        public static ILabelFactory Factory()
        {
            return new CoreLabelFactory();
        }

        public override ILabelFactory LabelFactory()
        {
            return CoreLabel.Factory();
        }

        public virtual string GetString(Type key)
        {
            string value = Get<string>(key);
            if (value == null)
            {
                return @"";
            }

            return value;
        }

        public override void SetFromString(string labelStr)
        {
            throw new NotSupportedException(@"Cannot set from string");
        }

        public override void SetValue(string value)
        {
            Set(typeof(CoreAnnotations.ValueAnnotation), value);
        }

        public override string Value()
        {
            return Get(typeof(CoreAnnotations.ValueAnnotation));
        }

        public override void SetWord(string word)
        {
            Set(typeof(CoreAnnotations.TextAnnotation), word);
            Remove(typeof(CoreAnnotations.LemmaAnnotation));
        }

        public override string Word()
        {
            return Get(typeof(CoreAnnotations.TextAnnotation));
        }

        public override void SetTag(string tag)
        {
            Set(typeof(CoreAnnotations.PartOfSpeechAnnotation), tag);
        }

        public override string Tag()
        {
            return Get(typeof(CoreAnnotations.PartOfSpeechAnnotation));
        }

        public override void SetCategory(string category)
        {
            Set(typeof(CoreAnnotations.CategoryAnnotation), category);
        }

        public override string Category()
        {
            return Get(typeof(CoreAnnotations.CategoryAnnotation));
        }

        public override void SetAfter(string after)
        {
            Set(typeof(CoreAnnotations.AfterAnnotation), after);
        }

        public override string After()
        {
            return GetString(typeof(CoreAnnotations.AfterAnnotation));
        }

        public override void SetBefore(string before)
        {
            Set(typeof(CoreAnnotations.BeforeAnnotation), before);
        }

        public override string Before()
        {
            return GetString(typeof(CoreAnnotations.BeforeAnnotation));
        }

        public override void SetOriginalText(string originalText)
        {
            Set(typeof(CoreAnnotations.OriginalTextAnnotation), originalText);
        }

        public override string OriginalText()
        {
            return GetString(typeof(CoreAnnotations.OriginalTextAnnotation));
        }

        public override string DocID()
        {
            return Get(typeof(CoreAnnotations.DocIDAnnotation));
        }

        public override void SetDocID(string docID)
        {
            Set(typeof(CoreAnnotations.DocIDAnnotation), docID);
        }

        public virtual string Ner()
        {
            return Get(typeof(CoreAnnotations.NamedEntityTagAnnotation));
        }

        public virtual void SetNER(string ner)
        {
            Set(typeof(CoreAnnotations.NamedEntityTagAnnotation), ner);
        }

        public override string Lemma()
        {
            return Get(typeof(CoreAnnotations.LemmaAnnotation));
        }

        public override void SetLemma(string lemma)
        {
            Set(typeof(CoreAnnotations.LemmaAnnotation), lemma);
        }

        public override int Index()
        {
            int n = Get(typeof(CoreAnnotations.IndexAnnotation));
            if (n == null)
                return -1;
            return n;
        }

        public override void SetIndex(int index)
        {
            Set(typeof(CoreAnnotations.IndexAnnotation), index);
        }

        public override int SentIndex()
        {
            int n = Get(typeof(CoreAnnotations.SentenceIndexAnnotation));
            if (n == null)
                return -1;
            return n;
        }

        public override void SetSentIndex(int sentIndex)
        {
            Set(typeof(CoreAnnotations.SentenceIndexAnnotation), sentIndex);
        }

        public override int BeginPosition()
        {
            int i = Get(typeof(CoreAnnotations.CharacterOffsetBeginAnnotation));
            if (i != null)
                return i;
            return -1;
        }

        public override int EndPosition()
        {
            int i = Get(typeof(CoreAnnotations.CharacterOffsetEndAnnotation));
            if (i != null)
                return i;
            return -1;
        }

        public override void SetBeginPosition(int beginPos)
        {
            Set(typeof(CoreAnnotations.CharacterOffsetBeginAnnotation), beginPos);
        }

        public override void SetEndPosition(int endPos)
        {
            Set(typeof(CoreAnnotations.CharacterOffsetEndAnnotation), endPos);
        }

        public static readonly string TAG_SEPARATOR = @"/";
        public static readonly string DEFAULT_FORMAT = @"value-index";
        public override string ToString()
        {
            return ToString(DEFAULT_FORMAT);
        }

        public virtual string ToString(string format)
        {
            StringBuilder buf = new StringBuilder();
            if (format.Equals(@"value"))
            {
                buf.Append(Value());
            }
            else if (format.Equals(@"{map}"))
            {
                Map map2 = new TreeMap();
                foreach (Class key in this.KeySet())
                {
                    map2.Put(key.GetName(), Get(key));
                }

                buf.Append(map2);
            }
            else if (format.Equals(@"value{map}"))
            {
                buf.Append(Value());
                Map map2 = new TreeMap(asClassComparator);
                foreach (Class key in this.KeySet())
                {
                    map2.Put(key, Get(key));
                }

                map2.Remove(typeof(CoreAnnotations.ValueAnnotation));
                buf.Append(map2);
            }
            else if (format.Equals(@"value-index"))
            {
                buf.Append(Value());
                int index = this.Get(typeof(CoreAnnotations.IndexAnnotation));
                if (index != null)
                {
                    buf.Append('-').Append((index).IntValue());
                }

                buf.Append(ToPrimes());
            }
            else if (format.Equals(@"value-index{map}"))
            {
                buf.Append(Value());
                int index = this.Get(typeof(CoreAnnotations.IndexAnnotation));
                if (index != null)
                {
                    buf.Append('-').Append((index).IntValue());
                }

                Map<String, Object> map2 = new TreeMap<String, Object>();
                foreach (Class key in this.KeySet())
                {
                    string cls = key.GetName();
                    int idx = cls.IndexOf('$');
                    if (idx >= 0)
                    {
                        cls = cls.Substring(idx + 1);
                    }

                    map2.Put(cls, this.Get(key));
                }

                map2.Remove(@"IndexAnnotation");
                map2.Remove(@"ValueAnnotation");
                if (!map2.IsEmpty())
                {
                    buf.Append(map2);
                }
            }
            else if (format.Equals(@"word"))
            {
                buf.Append(Word());
            }
            else if (format.Equals(@"text-index"))
            {
                buf.Append(this.Get(typeof(CoreAnnotations.TextAnnotation)));
                int index = this.Get(typeof(CoreAnnotations.IndexAnnotation));
                if (index != null)
                {
                    buf.Append('-').Append((index).IntValue());
                }

                buf.Append(ToPrimes());
            }

            return buf.ToString();
        }

        public virtual string ToPrimes()
        {
            int copy = Get(typeof(CoreAnnotations.CopyAnnotation));
            if (copy == null || copy == 0)
                return @"";
            return StringUtils.Repeat('\n', copy);
        }

        private static readonly IComparer<Type> asClassComparator = new AnonymousComparator(this);
        private sealed class AnonymousComparator : IComparer<Type>
        {
            public AnonymousComparator(CoreLabelFactory parent)
            {
                this.parent = parent;
            }

            private readonly CoreLabelFactory parent;
            public override int Compare(Type o1, Type o2)
            {
                return o1.Name.CompareTo(o2.Name);
            }
        }
    }
}
