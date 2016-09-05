using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace PA.Plugin.Components.Interfaces
{
    public interface IPluginProvider: IComponent
    {
        IPlugin Plugin { get; }
    }
}
