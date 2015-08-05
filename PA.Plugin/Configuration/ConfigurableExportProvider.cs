using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.ComponentModel.Composition.ReflectionModel;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using Toolbox.Converters;

namespace Toolbox.Configuration
{
    public class ConfigurableExportProvider : ExportProvider
    {
        private readonly IConfigurationSource configurationSource;

        public ConfigurableExportProvider(IConfigurationSource configurationSource)
        {
            // the configuration source determines where configuration values come from (eg. App.Config file)
            this.configurationSource = configurationSource;
        }

        protected override IEnumerable<Export> GetExportsCore(ImportDefinition definition, AtomicComposition atomicComposition)
        {
            var contractName = definition.ContractName;

            if (string.IsNullOrEmpty(contractName))
            {
                yield break;
            }

            if (definition.Cardinality != ImportCardinality.ZeroOrOne && definition.Cardinality != ImportCardinality.ExactlyOne)
            {
                yield break;
            }

            if (this.configurationSource.ContainsSetting(contractName))
            {
                Type targetType = null;
                string stringValue = null;

                if (ReflectionModelServices.IsImportingParameter(definition))
                {
                    var importingParameter = ReflectionModelServices.GetImportingParameter(definition);
                    targetType = importingParameter.Value.ParameterType;
                    stringValue = this.configurationSource.GetSetting(contractName);
                }
                else
                {
                    var getAccessor = ReflectionModelServices
                        .GetImportingMember(definition)
                        .GetAccessors()
                        .Where(x => x is MethodInfo)
                        .Select(x => x as MethodInfo)
                        .FirstOrDefault(x => (x.Attributes & MethodAttributes.SpecialName) == MethodAttributes.SpecialName && x.Name.StartsWith("get_", StringComparison.Ordinal));

                    if (getAccessor == null)
                    {
                        yield break;
                    }

                    targetType = getAccessor.ReturnType;

                    if (targetType == null)
                    {
                        yield break;
                    }

                    if (getAccessor.ReturnType.FullName.Equals(contractName))
                    {
                        stringValue = this.configurationSource.GetSetting(getAccessor.DeclaringType.FullName + "/" + getAccessor.Name.Remove(0, 4));
                    }
                    else
                    {
                        stringValue = this.configurationSource.GetSetting(contractName);
                    }

                    object value = null;

                    try
                    {
                        if (targetType.IsGenericType && typeof(Configurable).IsAssignableFrom(targetType))
                        {
                            value = Activator.CreateInstance(targetType, contractName, configurationSource);
                        }
                        else if (targetType.IsGenericType && typeof(IConfigurable).IsAssignableFrom(targetType))
                        {
                            targetType = typeof(Configurable<>).MakeGenericType(targetType.GetGenericArguments());
                            value = Activator.CreateInstance(targetType, contractName, configurationSource);
                        }
                        else if (targetType.IsValueType || targetType.IsArray || targetType.IsSerializable)
                        {
                            value = stringValue.ParseTo(targetType);
                        }
                        else
                        {
                        }
                    }
                    catch
                    {
                    }

                    if (value is object)
                    {
                        yield return new Export(contractName, () => value);
                    }
                }

            }
        }

      
    }
}
