using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition.Hosting;
using PA.Configuration;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Reflection;
using System.Diagnostics;
using PA.Converters.Extensions;

namespace PA.Plugin.Configuration
{
    public class UriHandlerExportProvider : CatalogExportProvider, IConfigurationProvider
    {
        public IConfigurationSource Source { get; private set; }

        public UriHandlerExportProvider(ComposablePartCatalog catalog, bool isThreadSafe, IConfigurationSource configurationSource)
            : base(catalog, isThreadSafe)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
        }

        public UriHandlerExportProvider(ComposablePartCatalog catalog, IConfigurationSource configurationSource)
            : base(catalog)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            foreach (ConfigurationProviderExtensions.Item Configuration in this.GetConfigurationItems(definition))
            {
                Type targetType = Configuration.Contract.Type;

                if (typeof(IUriHandler).IsAssignableFrom(targetType))
                {

                    foreach (Uri u in ("|" + Configuration.Value).ParseTo(typeof(Uri[])) as Array)
                    {
                        if (Uri.IsWellFormedUriString(u.AbsoluteUri, UriKind.Absolute))
                        {
                            ImportDefinition newDefinition = new ImportDefinition(
                                (d) => ValidateExport(d, targetType, u),
                                targetType.FullName,
                                ImportCardinality.ExactlyOne,
                                definition.IsRecomposable,
                                false);

                            Export e = base.GetExportsCore(newDefinition, atomicComposition).FirstOrDefault();
                            Lazy<Type> type = this.Catalog.Parts.ExportTypes(newDefinition, typeof(Uri)).FirstOrDefault();

                            if (e is Export && type is Lazy<Type>)
                            {
                                yield return new Export(e.Definition, () => Activator.CreateInstance(type.Value, u));
                            }
                        }
                    }
                }
            }

            yield break;
        }

        private bool ValidateExport(ExportDefinition definition, Type targetType, Uri Uri)
        {
            return (definition.ContractName == targetType.FullName)
                && definition.Metadata.ContainsKey("ExportTypeIdentity")
                && targetType.FullName.Equals(definition.Metadata["ExportTypeIdentity"])
                && definition.Metadata.ContainsKey("Scheme")
                && Uri.Scheme.Equals(definition.Metadata["Scheme"]);
        }
    }
}
