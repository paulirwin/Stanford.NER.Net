﻿using Stanford.NER.Net.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Ling
{
    public interface ICoreAnnotation<V>
        where V : TypesafeMap.Key<V>
    {
        Type GetTypeValue();
    }
}
