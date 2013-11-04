using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace PA.Plugin.Configuration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class UriHandlerAttribute : ExportAttribute
    {
        public string Scheme { get; private set; }

        public UriHandlerAttribute(Type type)
            : base(type)
        {
            this.Scheme = System.Uri.UriSchemeFile;
        }

        public UriHandlerAttribute(string scheme, Type type)
            : base(type)
        {
            this.Scheme = scheme;
        }
    }

}
