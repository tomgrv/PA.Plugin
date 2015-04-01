using System;
using System.ComponentModel.Composition;

namespace PA.Plugin
{
    public interface IPluginCategory
    {
        string Category { get; }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface), MetadataAttribute]
    public class PluginCategoryAttribute : Attribute, IPluginCategory
    {
        public string Category { get; private set; }

        public PluginCategoryAttribute(string Category)
            :base()
        {
            this.Category = Category;
        }

        public PluginCategoryAttribute()
            : base()
        {
            this.Category = "";
        }

    }
}
