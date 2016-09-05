using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using PA.Plugin.Extensions;
using PA.Plugin.Components.Interfaces;

namespace PA.Plugin.Components.Controls
{
    public partial class PluginToolStripItem : ToolStripMenuItem, IPluginProvider
    {
        public IPlugin Plugin { get; private set; }

        public PluginToolStripItem()
        {
            InitializeComponent();
        }

        public PluginToolStripItem(IPlugin pi)
            : base(pi.GetDescription(), pi.GetImage(), null, pi.ToString())
        {
            this.Plugin = pi;
            InitializeComponent();
        }

        public PluginToolStripItem(IPlugin pi, EventHandler<PluginEventArgs> OnClick)
            : base(pi.GetDescription(), pi.GetImage(), null, pi.ToString())
        {
            this.Plugin = pi;
            InitializeComponent();

            this.Click += new EventHandler((o, e) => OnClick(o, new PluginEventArgs(pi)));
        }
    }
}
