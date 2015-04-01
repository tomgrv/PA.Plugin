using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Windows.Forms.Design;
using PA.Plugin.Extensions;

namespace PA.Plugin.Components.ParameterForm
{
    public partial class PluginParametersForm : System.Windows.Forms.Form
    {
        public IPlugin Plugin
        {
            get
            {
                return this.flowLayoutPanel1.Plugin;
            }
        }

        public bool HasParameters
        {
            get
            {
                return this.flowLayoutPanel1.Controls.Count > 0;
            }
        }

        public PluginParametersForm()
        {
            InitializeComponent();
        }

        private void ok_button_Click(object sender, EventArgs e)
        {
            this.flowLayoutPanel1.Save();
        }

        public void Refresh<T>(T pi)
            where T : IPlugin
        {
            this.flowLayoutPanel1.Refresh<T>(pi);
            this.Text = "Parameters for " + pi.GetDescription();
        }
    }
}
