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
using System.Runtime.Serialization;
using System.Runtime.InteropServices;
using PA.Plugin.Extensions;
using System.Security.AccessControl;

namespace PA.Plugin.Configuration
{
    public class WithLabelExportProvider<T> : CatalogExportProvider, IConfigurationProvider
        where T : class
         
    {
        public IConfigurationSource Source { get; private set; }

        private Func<string, bool> IsValid;
        private Func<string, T> GetInstance;
        private Func<T, string> GetLabel;

        public WithLabelExportProvider(ComposablePartCatalog catalog, bool isThreadSafe, IConfigurationSource configurationSource)
            : base(catalog, isThreadSafe)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
        }

        public WithLabelExportProvider(ComposablePartCatalog catalog, bool isThreadSafe, IConfigurationSource configurationSource, [Optional]Func<T, string> label, [Optional] Func<string, bool> validation, [Optional] Func<string, T> creation)
            : base(catalog, isThreadSafe)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
            this.IsValid = validation;
            this.GetInstance = creation;
            this.GetLabel = label;
        }

        public WithLabelExportProvider(ComposablePartCatalog catalog, IConfigurationSource configurationSource, [Optional] Func<T, string> label, [Optional] Func<string, bool> validation, [Optional] Func<string, T> creation)
            : base(catalog)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
            this.IsValid = validation;
            this.GetInstance = creation;
            this.GetLabel = label;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            lock (this.Source)
            {
                foreach (Export e in this.GetConfigurationItems(definition)
                                          .GetExports<IPlugin<T>>((t, s) => this.GetPartInstance(definition, t, s)))
                {
                    yield return e;
                }
            }
        }

        private IPlugin<T> GetPartInstance(ImportDefinition definition, Type type, string configvalue)
        {
            if (typeof(IPlugin<T>).IsAssignableFrom(type))
            {
                if (this.IsValid != null ? this.IsValid(configvalue) : true)
                {
                    T value = this.GetInstance != null ? this.GetInstance(configvalue) : configvalue.ParseTo<T, string>();

                    string contract = ContractExtensions.GetTypeIdentity(type);

                    ImportDefinition newDefinition = new ImportDefinition(
                        (d) => ValidateExport(d, contract, this.GetLabel != null ? this.GetLabel(value) : value.ToString()),
                        type.FullName,
                        ImportCardinality.ExactlyOne,
                        definition.IsRecomposable,
                        false);

                    Lazy<Type> partType = this.Catalog.Parts.ExportTypes<ComposablePartDefinition>(newDefinition).FirstOrDefault();
                    if (partType != null)
                    {
                        return value.ParseTo<IPlugin<T>, T>(partType.Value);
                    }
                }
            }

            return null;
        }

        private bool ValidateExport(ExportDefinition definition, string contract, string label)
        {
            return definition.ContractName == contract
                && definition.Metadata.ContainsKey("ExportTypeIdentity")
                && contract.Equals(definition.Metadata["ExportTypeIdentity"])
                && definition.Metadata.ContainsKey("Label")
                && label.Equals(definition.Metadata["Label"]);
        }
    }
}
