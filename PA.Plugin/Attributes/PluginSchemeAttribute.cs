using System;
using System.ComponentModel.Composition;

namespace Toolbox.Plugin
{
    public interface IPluginAdressable
    {
        string Scheme { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface), MetadataAttribute]
    public class PluginSchemeAttribute : Attribute, IPluginAdressable
    {
        public string Scheme { get; private set; }

        public PluginSchemeAttribute(string Scheme)
            : base()
        {
            this.Scheme = Scheme;
        }

        public PluginSchemeAttribute()
            : base()
        {
            this.Scheme = "";
        }
    }
}
