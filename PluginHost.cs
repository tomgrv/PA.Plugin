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

namespace PA
{
    public partial class PluginHost : Component, ISupportInitialize, ISupportInitializeNotification
    {
        [Browsable(false)]
        public AggregateCatalog Catalogs { get; private set; }

        [Browsable(false)]
        public CompositionContainer CompositionContainer { get; private set; }

        [Description("Plugin Location")]
        [AmbientValue("")]
        public string Location { get; set; }

        [Description("Configuration access")]
        public IConfigurationSource Configuration { get; set; }

        #region Types

        public IEnumerable<Type> GetExportedTypes<T>()
        {
            return Catalogs.Parts
                .Select(p => ComposablePartExportType<T>(p))
                .Where(t => t is Type);
        }

        private static Type ComposablePartExportType<T>(ComposablePartDefinition part)
        {

            if (part.ExportDefinitions
                .Any(def => def.Metadata.ContainsKey("ExportTypeIdentity") && def.Metadata["ExportTypeIdentity"].Equals(typeof(T).FullName)))
            {
                  return ReflectionModelServices.GetPartType(part).Value;
            }

            return null;
        }

        #endregion

        #region Load/Unload

        public void Load(DirectoryInfo path)
        {
            this.Load(path.FullName);
        }

        public void Load(string path)
        {
            if (!Path.IsPathRooted(path))
            {
                if (Directory.Exists(path))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), path);
                }
                else
                {
                    path = Directory.GetCurrentDirectory();
                }
            }


            if (Directory.Exists(path))
            {
                DirectoryCatalog dc = new DirectoryCatalog(path);
                this.Catalogs.Catalogs.Add(dc);

                if (this.Configuration is IConfigurationSource && dc is ComposablePartCatalog)
                {
                    ConfigurationCatalog cc = new ConfigurationCatalog(this.Configuration, dc);
                    this.Catalogs.Catalogs.Add(cc);
                }
            }

            try
            {
                this.CompositionContainer.ComposeParts(this.Parent);

                while (this.toCompose.Count > 0)
                {
                    this.CompositionContainer.ComposeParts(this.toCompose.Dequeue());
                }
            }
            catch (System.Reflection.ReflectionTypeLoadException e)
            {
                foreach (Exception ex in e.LoaderExceptions)
                {
                    Trace.TraceError(ex.Message);
                }
            }
            catch (CompositionException e)
            {
                foreach (CompositionError er in e.Errors)
                {
                    Trace.TraceError(er.Description);
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                this.toCompose.Clear();
            }
        }

        public void UnLoad(DirectoryInfo path)
        {
            this.UnLoad(path.FullName);
        }

        public void UnLoad(string path)
        {
            Catalogs.Catalogs.Remove(new DirectoryCatalog(path));
        }

        public void UnLoad()
        {
            Catalogs.Catalogs.Clear();
        }

        #endregion

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

        #region ISupportInitialize Members

        public void BeginInit()
        {
            if (!this.DesignMode)
            {
                this.Catalogs = new AggregateCatalog();
            }
        }

        public void EndInit()
        {
            if (!this.DesignMode)
            {
                if (CompositionContainer == null)
                {
                    List<ExportProvider> eplist = new List<ExportProvider>();

                    if (this.Configuration is IConfigurationSource)
                    {
                        eplist.Add(new UriHandlerExportProvider(Catalogs, true, this.Configuration));
                        eplist.Add(new ConfigurationItemExportProvider(this.Configuration));
                    }

                    CompositionContainer = new CompositionContainer(Catalogs, eplist.ToArray());

                    foreach (CatalogExportProvider ep in eplist.OfType<CatalogExportProvider>())
                    {
                        ep.SourceProvider = CompositionContainer;
                    }
                }

                if (this.Configuration is IConfigurationSource && this.Configuration.ContainsSetting(Process.GetCurrentProcess().ProcessName + "/Plugins"))
                {
                    this.Location = this.Configuration.GetSetting(Process.GetCurrentProcess().ProcessName + "/Plugins");
                }

                this.Load(this.Location);
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
