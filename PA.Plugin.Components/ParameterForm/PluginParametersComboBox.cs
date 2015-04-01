using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace PA.Plugin.Components.ParameterForm
{
    public partial class PluginParametersComboBox : UserControl
    {        
        public PluginParametersComboBox()
        {
            InitializeComponent();
        }

        private void label_Resize(object sender, EventArgs e)
        {
            this.comboBox.Left = this.label.Right + 6;
        }

        private void label_Click(object sender, EventArgs e)
        {

        }

    }
}
