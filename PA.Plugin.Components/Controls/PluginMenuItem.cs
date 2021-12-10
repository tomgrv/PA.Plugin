using PA.Plugin.Components;
using PA.Plugin.Components.Interfaces;
using PA.Plugin.Extensions;
using PA.Plugin.Operations.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;

namespace PA.Plugin.Components.Controls
{
    [ToolboxItem(typeof(IPluginHandler))]
    public partial class PluginMenuItem : ToolStripMenuItem, IPluginHandler, ISupportInitialize
    {
        public enum GroupType
        {
            None,
            Category,
            CategoryDropDown
        }

        [Browsable(false)]
        [ReadOnly(true)]
        [ImportMany("Toolbox.Plugin.Controls/PluginMenuItem/ForceDefaultParameters")]
        public IEnumerable<string> ForceDefaultParameters { get; private set; }

        #region Plugin Management

        [Category("Plugin Management")]
        [Description("Specify how to group plugins")]
        [DefaultValue(GroupType.None)]
        public PluginMenuItem.GroupType GroupBy { get; set; }


        public void BuildWithType<T>() where T : IPlugin
        {
            this.DropDownItems.Clear();

            foreach (IPlugin o in this.Imports.OfType<T>().OrderBy(o => PluginManager.GetAttribute<PluginDescriptionAttribute>(o.GetType()).Description))
            {
                PluginToolStripItem PluginMenu = new PluginToolStripItem(o, new EventHandler<PluginEventArgs>(this.OnPluginItemClicked));

                string category = PluginMenu.Plugin.GetCategory();

                switch (this.GroupBy)
                {
                    case PluginMenuItem.GroupType.CategoryDropDown:

                        if (category.Length > 0)
                        {
                            ToolStripMenuItem item = this.DropDownItems.Cast<ToolStripMenuItem>().LastOrDefault(t => t.Name == category);

                            if (item is ToolStripItem)
                            {
                                item.DropDownItems.Add(PluginMenu);
                            }
                            else
                            {
                                item = new ToolStripMenuItem(category) { Name = category };
                                this.DropDownItems.Add(item);
                                item.DropDownItems.Add(PluginMenu);
                            }
                        }
                        else
                        {
                            this.DropDownItems.Add(PluginMenu);
                        }

                        break;

                    case PluginMenuItem.GroupType.Category:

                        if (category.Length > 0)
                        {
                            ToolStripItem item = this.DropDownItems.Cast<ToolStripItem>().LastOrDefault(t => t.Name == category);

                            if (item is ToolStripItem)
                            {
                                this.DropDownItems.Insert(this.DropDownItems.IndexOf(item) + 1, PluginMenu);
                            }
                            else
                            {
                                this.DropDownItems.Add(new ToolStripSeparator() { Name = category });
                                this.DropDownItems.Add(PluginMenu);
                            }
                        }
                        else
                        {
                            this.DropDownItems.Insert(0, PluginMenu);
                        }
                        break;

                    default:
                        this.DropDownItems.Add(PluginMenu);
                        break;
                }
            }

            this.Enabled = (this.DropDownItems.Count > 0);
        }

        #endregion

        public PluginMenuItem()
            : base()
        {
            InitializeComponent();
        }

        #region IPluginSource Members

        [Category("Plugin Management")]
        public PluginLoader Loader { get; set; }

        [Category("Plugin Management")]
        public event EventHandler<PluginEventArgs> PluginChanged;

        [Browsable(false)]
        public IPlugin Plugin { get; protected set; }

        #endregion

        #region IPartImportsSatisfiedNotification Members

        [ImportMany(AllowRecomposition = true)]
        protected virtual IEnumerable<IActionPlugin> Imports { get; set; }

        public virtual void OnImportsSatisfied()
        {
            this.BuildWithType<IActionPlugin>();
        }

        #endregion

        #region ISupportInitialize Members

        public virtual void BeginInit()
        {
            if (!this.DesignMode)
            {

            }
        }

        public virtual void EndInit()
        {
            if (!this.DesignMode)
            {
                if (this.Loader is PluginLoader)
                {
                    this.Loader.DelayedComposition(this);
                }
            }
        }

        #endregion

        protected virtual void OnPluginItemClicked(object sender, PluginEventArgs e)
        {
            if (e is PluginEventArgs && e.Plugin is IPlugin)
            {
                this.ParameterDialog.Refresh(e.Plugin);
                this.OnPluginChanged(e);
            }
        }

        protected virtual void OnPluginChanged(PluginEventArgs e)
        {
            if (this.ForceDefaultParameters.Contains(e.Plugin.GetType().FullName)
                || !this.ParameterDialog.HasParameters
                || this.ParameterDialog.ShowDialog() == DialogResult.OK)
            {
                this.Plugin = e.Plugin;

                if (this.PluginChanged != null)
                {
                    this.PluginChanged(this, e);
                }
            }
        }
    }


}
