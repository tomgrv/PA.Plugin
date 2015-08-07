using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using PA.Plugin.Components.Interfaces;
using PA.Converters.Extensions;

namespace PA.Plugin.Components.ParameterForm
{
    public partial class PluginParametersPanel : FlowLayoutPanel, IPluginProvider
    {
        public PluginParametersPanel()
        {
            InitializeComponent();
        }

        [Browsable(false)]
        public IPlugin Plugin { get; private set; }

        [Browsable(false)]
        public int Count { get; private set; }

        public void Refresh<T>(T p)
            where T : IPlugin
        {
            this.Plugin = p;

            if (this.Plugin is T)
            {
                lock (this.Plugin)
                {
                    this.Controls.Clear();

                    foreach (PropertyInfo pi in this.Plugin
                        .GetType()
                        .GetProperties()
                        .Where(pda => pda.CanRead && pda.CanWrite))
                    {
                        PluginDescriptionAttribute pda = pi.GetCustomAttributes(typeof(PluginDescriptionAttribute), true)
                             .OfType<PluginDescriptionAttribute>()
                             .FirstOrDefault();

                        if (pda is PluginDescriptionAttribute && (pda.TargetType == Type.EmptyTypes || pda.TargetType.Contains(typeof(T))))
                        {
                            if (pi.PropertyType.IsEnum)
                            {
                                PluginParametersComboBox cb = new PluginParametersComboBox();
                                cb.AutoSize = true;
                                cb.Tag = pi;
                                cb.label.Text = pda.Description + (pda.Name is string ? " (" + pda.Name + ")" : "");
                                cb.comboBox.Items.AddRange(Enum.GetNames(pi.PropertyType));
                                cb.comboBox.SelectedIndex = 0;
                                this.Controls.Add(cb);
                            }
                            else if (pi.PropertyType.Equals(typeof(bool)))
                            {
                                CheckBox cb = new CheckBox();
                                cb.AutoSize = true;
                                cb.Text = pda.Description + (pda.Name is string ? " (" + pda.Name + ")" : "");
                                cb.Checked = (bool)pi.GetValue(this.Plugin, null);
                                cb.Tag = pi;
                                this.Controls.Add(cb);
                            }
                            else if (pi.PropertyType.IsSerializable)
                            {
                                PluginParametersTextBox cb = new PluginParametersTextBox();
                                cb.label.Text = pda.Description + (pda.Name is string ? " (" + pda.Name + ")" : "");
                                cb.textBox.Text = pi.GetValue(this.Plugin, null).ToString().Trim();
                                cb.Tag = pi;
                                this.Controls.Add(cb);
                            }

                        }
                    }

                    this.Count = this.Controls.Count;
                }
            }

            base.Refresh();
        }

        public void Save()
        {
            if (this.Plugin is IPlugin)
            {
                lock (this.Plugin)
                {
                    foreach (Control c in this.Controls)
                    {
                        PropertyInfo pi = c.Tag as PropertyInfo;

                        if (pi is PropertyInfo)
                        {
                            if (pi.PropertyType.IsEnum  && (c as PluginParametersComboBox).comboBox.SelectedItem != null && Enum.IsDefined(pi.PropertyType, (c as PluginParametersComboBox).comboBox.SelectedItem))
                            {
                                pi.SetValue(this.Plugin, (c as PluginParametersComboBox).comboBox.SelectedItem.ToString().ParseTo<object, string>(pi.PropertyType), null);
                            }
                            else if (pi.PropertyType.Equals(typeof(bool)))
                            {
                                pi.SetValue(this.Plugin, (c as CheckBox).Checked, null);
                            }
                            else if (pi.PropertyType.IsSerializable)
                            {
                                pi.SetValue(this.Plugin, (c as PluginParametersTextBox).textBox.Text.ParseTo<object,string>(pi.PropertyType), null);
                            }
                        }
                    }
                }
            }
        }
    }
}
