using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.Plugin.Configuration
{
    public class CompositionConfigurator : IDisposable,IComponent
    {
        public ComposablePartCatalog Catalog { get { return this._catalog; } }

        private List<ComposablePartCatalog> _backup;
        private AggregateCatalog _catalog;
        private List<ExportProvider> _exportProviders;

        public CompositionConfigurator()
        {
            this._backup = new List<ComposablePartCatalog>();
            this._catalog = new AggregateCatalog();
            this._exportProviders = new List<ExportProvider>();
        }

        public void Unload()
        {
            foreach (ComposablePartCatalog c in this._catalog.Catalogs)
            {
                this._backup.Add(c);
            }

            this._catalog.Catalogs.Clear();
        }

        public void Reload()
        {
            foreach (ComposablePartCatalog c in this._backup)
            {
                this._catalog.Catalogs.Add(c);
            }

            this._catalog.Catalogs.Clear();
        }

        public void WithDirectory(string path)
        {
            DirectoryCatalog catalog = null;
            this.WithDirectory(path, out catalog);
        }

        public void WithDirectory(string path, out DirectoryCatalog catalog)
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

            catalog = new DirectoryCatalog(path);

            this.With(catalog);
        }

        public void With(ComposablePartCatalog catalog)
        {
            this._catalog.Catalogs.Add(catalog);
        }

        public void With(ExportProvider ep)
        {
            this._exportProviders.Add(ep);
        }

        public CompositionContainer GetContainer()
        {
            CompositionContainer cc = new CompositionContainer(this._catalog, this._exportProviders.ToArray());

            foreach (CatalogExportProvider cep in _exportProviders.OfType<CatalogExportProvider>())
            {
                cep.SourceProvider = cc;
            }

            return cc;
        }

        #region IComponent

        public void Dispose()
        {
            this._catalog.Dispose();

            if (this.Disposed != null)
            {
                this.Disposed(this, EventArgs.Empty);
            }
        }

        public event EventHandler Disposed;

        public ISite Site { get; set;}
      
        #endregion
    }
}
