using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.Composition;

namespace PA.Plugin.Threads.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class PluginThreadAttribute : ExportAttribute 
    {
        public int MaxInstances { get; private set;}
       
        public PluginThreadAttribute(int maxInstances)
            :base()
        {
            this.MaxInstances = maxInstances;
        }

        public PluginThreadAttribute()
            : base()
        {
            this.MaxInstances = 10;
        }
       
    }
}
