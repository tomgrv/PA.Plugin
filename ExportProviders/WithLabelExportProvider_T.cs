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

namespace PA.Plugin.Configuration
{
    public class WithLabelExportProvider<T> : CatalogExportProvider, IConfigurationProvider
        where T : class
    {
        public IConfigurationSource Source { get; private set; }

        Func<string, bool> _isValid;
        Func<string, T> _create;
        Func<T, string> _getLabel;

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
            this._isValid = validation;
            this._create = creation;
            this._getLabel = label;
        }

        public WithLabelExportProvider(ComposablePartCatalog catalog, IConfigurationSource configurationSource, [Optional] Func<T, string> label, [Optional] Func<string, bool> validation, [Optional] Func<string, T> creation)
            : base(catalog)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.Source = configurationSource;
            this._isValid = validation;
            this._create = creation;
            this._getLabel = label;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            foreach (ConfigurationProviderExtensions.Item Configuration in this.GetConfigurationItems(definition))
            {
                if (!Configuration.Contract.Name.StartsWith("#/"))
                {
                    Type targetType = Configuration.Contract.Type;

                    if (targetType.IsArray)
                    {
                        foreach (string configvalue in Configuration.Value.AsArray())
                        {
                            Export e = GetUnitaryExport(definition, targetType.GetElementType(), configvalue);
                            if (e is Export)
                            {
                                yield return e;
                            }
                        }
                    }
                    else
                    {
                        Export e = GetUnitaryExport(definition, targetType, Configuration.Value);
                        if (e is Export)
                        {
                            yield return e;
                        }
                    }
                }
                else
                {
                }
            }
        }

        private Export GetUnitaryExport(ImportDefinition definition, Type type, string configvalue)
        {
            if (type.IsAssignableFrom(typeof(IPlugin<T>)))
            {
                if (this._isValid is Func<string, bool> ? this._isValid(configvalue) : true)
                {
                    T value = this._create is Func<string, T> ? this._create(configvalue) : configvalue.ParseTo<T, string>();

                    string contract = ContractExtensions.GetTypeIdentity(type);

                    ImportDefinition newDefinition = new ImportDefinition(
                        (d) => ValidateExport(d, contract, this._getLabel is Func<T, string> ? this._getLabel(value) : value.ToString()),
                        type.FullName,
                        ImportCardinality.ExactlyOne,
                        definition.IsRecomposable,
                        false);

                    Lazy<Type> partType = this.Catalog.Parts.ExportTypes(newDefinition, typeof(T)).FirstOrDefault();
                    if (partType is Lazy<Type>)
                    {
                        return new Export(definition.ContractName, () => value.ParseTo<object,T>(partType.Value));
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
