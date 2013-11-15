using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface ILabeled<E>
    {
        E Label();
        ICollection<E> Labels();
    }
}
