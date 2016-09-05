using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PA.Plugin
{
    [Obsolete]
    public class PluginItemEventArgs : EventArgs
    {
        public PluginItem PluginItem { get; private set; }

        public PluginItemEventArgs(PluginItem pi)
            : base()
        {
            this.PluginItem = pi;
        }

        public new static readonly PluginItemEventArgs Empty;

    }
}
