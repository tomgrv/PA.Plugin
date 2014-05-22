using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin
{
    public interface IPlugin<T> 
        where T : class
    {
        T Value { get; }
    }
}
