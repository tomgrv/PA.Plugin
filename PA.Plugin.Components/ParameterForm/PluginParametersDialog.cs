using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using PA.Plugin;

namespace PA.Plugin.Components.ParameterForm
{
    public class PluginParametersDialog : CommonDialog
    {
        public class WindowWrapper : System.Windows.Forms.IWin32Window
        {
            public WindowWrapper(IntPtr handle)
            {
                _hwnd = handle;
            }

            public IntPtr Handle
            {
                get { return _hwnd; }
            }

            private IntPtr _hwnd;
        }

        private PluginParametersForm form;

        public bool HasParameters
        {
            get
            {
                return this.form.HasParameters;
            }
        }

        public PluginParametersDialog()
            : base()
        {
            this.form = new PluginParametersForm();
        }


        public override void Reset()
        {
            if (this.form is PluginParametersForm)
            {
                this.form.Dispose();
            }

            this.form = new PluginParametersForm();
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            return this.form.ShowDialog(new WindowWrapper(hwndOwner)) == DialogResult.OK;
        }

        public void Refresh(IPlugin pi)
        {
            this.form.Refresh(pi);
        }
    }
}
