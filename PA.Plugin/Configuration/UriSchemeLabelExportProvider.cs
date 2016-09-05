using PA.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;

namespace PA.Plugin.Configuration
{
    public class UriSchemeLabelExportProvider : WithLabelExportProvider<Uri>, IConfigurationProvider
    {
        public UriSchemeLabelExportProvider(ComposablePartCatalog catalog, bool isThreadSafe, IConfigurationSource configurationSource)
            : base(catalog, isThreadSafe, configurationSource,
                  label: u => u.Scheme,
                  validation: s => Uri.IsWellFormedUriString(s, UriKind.Absolute),
                  creation: s => new Uri(s)) {}
       
        public UriSchemeLabelExportProvider(ComposablePartCatalog catalog, IConfigurationSource configurationSource)
            : base(catalog, configurationSource,
                   label: u => u.Scheme,
                   validation: s => Uri.IsWellFormedUriString(s, UriKind.Absolute),
                creation: s => new Uri(s)) {}
        
    }
}
