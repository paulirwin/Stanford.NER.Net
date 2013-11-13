using Stanford.NER.Net.ObjectBank;
using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Sequences
{
    public interface IDocumentReaderAndWriter<IN> : IIteratorFromReaderFactory<IList<IN>>
        where IN : ICoreMap
    {
        void Init(SeqClassifierFlags flags);
        void PrintAnswers(List<IN> doc, TextWriter out_renamed);
    }
}
