using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin
{
    public interface IPluginHandler
    {
        PluginItem PluginItem { get; }
        event EventHandler<PluginItemEventArgs> PluginChanged;
    }
}
