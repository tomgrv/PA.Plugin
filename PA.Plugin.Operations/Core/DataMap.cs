using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin.Operations.Core
{
    public class DataMap : Dictionary<string, object>
    {
        public DataMap(IDictionary<string, object> map)
            : base(map)
        { 
        }

        public DataMap()
            : base()
        {
        }
    }
}
