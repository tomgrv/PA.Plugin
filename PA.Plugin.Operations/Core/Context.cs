using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin.Operations.Core
{
    public class Context
    {
        public DataMap Data { get; set; }
        public Type Type { get; set; }

        public Context(IDictionary<string, object> data)
        {
            this.Data = new DataMap(data);
        }

        public Context()
        {
            this.Data = new DataMap();
        }
    }
}
