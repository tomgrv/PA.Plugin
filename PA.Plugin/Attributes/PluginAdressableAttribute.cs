using System;
using System.ComponentModel.Composition;

namespace Toolbox.Plugin
{
    public interface IPluginAdressable
    {
        string Scheme { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface), MetadataAttribute]
    public class PluginAdressableAttribute : ExportAttribute, IPluginAdressable
    {
        public string Scheme { get; private set; }

        public PluginAdressableAttribute(string Scheme):
            base(typeof(IPluginAdressable))
        {
            this.Scheme = Scheme;
        }

        public PluginAdressableAttribute():
            base(typeof(IPluginAdressable))
        {
            this.Scheme = "";
        }
    }
}
