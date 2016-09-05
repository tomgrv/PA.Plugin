using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;

namespace PA.Plugin
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class ExportWithLabelAttribute : ExportAttribute, IPluginLabel
    {
        public string Label { get; private set; }

        public ExportWithLabelAttribute(string label)
            : base()
        {
            this.Label = label;
        }

        public ExportWithLabelAttribute(Type type, string label)
            : base(type)
        {
            this.Label = label;
        }
    }
}
