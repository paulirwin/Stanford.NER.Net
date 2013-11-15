using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stanford.NER.Net.Support
{
    public class Properties : HashMap<object, object>
    {
        protected Properties defaults;

        public Properties()
        {
        }

        public Properties(Properties defaults)
        {
            this.defaults = defaults;
        }

        public string GetProperty(string key)
        {
            return this[key] as string;
        }

        public void SetProperty(string key, string value)
        {
            this[key] = value;
        }

    }
}
