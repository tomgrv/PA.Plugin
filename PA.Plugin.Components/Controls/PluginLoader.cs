using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.ComponentModel.Design;
using System.IO;
using System.Reflection;
using PA.Plugin;
using System.Diagnostics;
using PA.Configuration;
using PA.Plugin.Configuration;
using PA.Plugin.Extensions;

namespace PA.Plugin.Components.Controls
{
    public partial class PluginLoader : Component, ISupportInitialize, ISupportInitializeNotification, IEditableObject
    {
        [Description("Plugin Location")]
        [AmbientValue("")]
        public string Location { get; set; }

        [Description("Configuration access")]
        public IConfigurationSource Configuration { get; set; }

        private CompositionContainer _container;
        private CompositionConfigurator _configurator;


        #region Delayed composition

        private Queue<IComponent> toCompose = new Queue<IComponent>();

        public void DelayedComposition(IComponent component)
        {
            if (this._container is CompositionContainer && this.IsInitialized)
            {
                try
                {
                    this._container.ComposeParts(component);
                }
                catch (System.Reflection.ReflectionTypeLoadException e)
                {
                    foreach (Exception ex in e.LoaderExceptions)
                    {
                        Trace.TraceError(ex.Message);
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
            }
            else
            {
                this.toCompose.Enqueue(component);
            }
        }

        #endregion

        #region ISupportInitialize Members

        public void BeginInit()
        {
            this._configurator = new CompositionConfigurator();
        }

        public void EndInit()
        {
            if (!this.DesignMode)
            {
                this.EndEdit();
            }

            if (!this.IsInitialized)
            {
                this.OnInitialized();
            }
        }

        #endregion

        #region ISupportInitializeNotification Members

        virtual protected void OnInitialized()
        {
            this.IsInitialized = true;

            if (this.Initialized != null)
            {
                this.Initialized(this, EventArgs.Empty);
            }
        }

        public event EventHandler Initialized;

        [Browsable(false)]
        public bool IsInitialized { get; private set; }

        #endregion

        #region Parent Support

        [DefaultValue(null)]
        [AmbientValue(null)]
        public IComponent Parent { get; set; }

        public override ISite Site
        {
            get { return base.Site; }
            set
            {
                base.Site = value;

                if (value is ISite && Parent == null)
                {
                    IDesignerHost host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    this.Parent = host != null ? host.RootComponent : null;
                }
            }
        }

        #endregion

        public void BeginEdit()
        {
            this._configurator.Unload();
            this.toCompose.Clear();
        }

        public void CancelEdit()
        {
            this._configurator.Reload();
            this.toCompose.Clear();
        }

        public void EndEdit()
        {
            if (this.Configuration is IConfigurationSource && this.Configuration.ContainsSetting(Process.GetCurrentProcess().ProcessName + "/Plugins"))
            {
                this.Location = this.Configuration.GetSetting(Process.GetCurrentProcess().ProcessName + "/Plugins");
            }

            this._configurator.WithDirectory(this.Location);
            this._configurator.With(new ConfigurationItemExportProvider(this.Configuration));
            this._configurator.With(new ConfigurationCatalog(this.Configuration, this._configurator.Catalog));
            this._configurator.With(new WithLabelExportProvider<Uri>(this._configurator.Catalog, true, this.Configuration,
                  label: u => u.Scheme,
                  validation: s => Uri.IsWellFormedUriString(s, UriKind.Absolute),
                  creation: s => new Uri(s)
            ));
            
            this._container = this._configurator.GetContainer();
            this._container.ComposeParts(this.Parent);
            this._container.ComposeParts(this.toCompose);
        }
    }
}
