using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;

namespace PA.Plugin.Configuration
{
    public class CompositionConfigurator
    {
        public ComposablePartCatalog Catalog { get { return this._catalog; } }

        private AggregateCatalog _catalog;
        private List<ExportProvider> _exportProviders;

        public CompositionConfigurator()
        {
            this._catalog = new AggregateCatalog();
            this._exportProviders = new List<ExportProvider>();
        }

        public void WithDirectory(string path)
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

            this.With(new DirectoryCatalog(path));
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
    }
}
