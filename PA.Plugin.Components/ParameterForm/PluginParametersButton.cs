using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using PA.Plugin.Components.Interfaces;

namespace PA.Plugin.Components.ParameterForm
{
    public partial class PluginParametersButton : Button
    {
        private PluginParametersForm form;

        public PluginParametersButton()
        {
            InitializeComponent();
            this.form = new PluginParametersForm();
        }

        public bool Refresh<T>(T p) where T : IPlugin
        {
            this.form.Refresh<T>(p);
            this.Enabled = this.form.HasParameters;
            return true;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            this.form.ShowDialog();
        }
    }
}
