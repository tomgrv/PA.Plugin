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

namespace PA.Components
{
    public partial class PluginLoader : Component, ISupportInitialize, ISupportInitializeNotification
    {
        [Browsable(false)]
        public CompositionContainer CompositionContainer { get; private set; }

        [Description("Plugin Location")]
        [AmbientValue("")]
        public string Location { get; set; }

        [Description("Configuration access")]
        public IConfigurationSource Configuration { get; set; }

        #region Delayed composition

        private Queue<IComponent> toCompose = new Queue<IComponent>();

        public void DelayedComposition(IComponent component)
        {
            if (this.CompositionContainer is CompositionContainer && this.IsInitialized)
            {
                try
                {
                    this.CompositionContainer.ComposeParts(component);
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
            
        }

        public void EndInit()
        {
            if (!this.DesignMode)
            {
                if (this.Configuration is IConfigurationSource && this.Configuration.ContainsSetting(Process.GetCurrentProcess().ProcessName + "/Plugins"))
                {
                    this.Location = this.Configuration.GetSetting(Process.GetCurrentProcess().ProcessName + "/Plugins");
                }

                this.CompositionContainer = PluginManager.GetContainer(x =>
                {
                    x.WithDirectory(this.Location);
                    x.With(new UriHandlerExportProvider(x.Catalog, true, this.Configuration));
                    x.With(new ConfigurationItemExportProvider(this.Configuration));
                });

                this.CompositionContainer.ComposeParts(this.Parent);
                this.CompositionContainer.ComposeParts(this.toCompose);
            }

            this.OnInitialized();
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
    }
}
