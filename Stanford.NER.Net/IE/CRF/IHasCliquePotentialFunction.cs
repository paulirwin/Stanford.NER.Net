using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.IE.CRF
{
    public interface IHasCliquePotentialFunction
    {
        ICliquePotentialFunction GetCliquePotentialFunction(double[] x);
    }
}
