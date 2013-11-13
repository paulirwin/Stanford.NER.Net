using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.ObjectBank
{
    public interface IIteratorFromReaderFactory<T>
    {
        IEnumerator<T> GetIterator(TextReader r);
    }
}
