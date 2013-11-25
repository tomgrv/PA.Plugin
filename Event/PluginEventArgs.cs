using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin
{
    public class PluginItemEventArgs : EventArgs
    {
        public IPlugin Plugin { get; private set; }

        public PluginEventArgs(IPlugin pi)
            :base()
        {
            this.Plugin = pi;
        }

        public static readonly PluginItemEventArgs Empty;
        
    }
}
