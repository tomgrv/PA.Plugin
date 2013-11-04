using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PA.Plugin
{
    [Obsolete]
    public interface IPluginHelper :  IComponent
    {
        IPluginHelper NextHelper { get; set; }
        bool Help<T>(T p) where T : IPlugin;
    }
}
