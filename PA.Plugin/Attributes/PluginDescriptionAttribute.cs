using System;
using System.Linq;
using System.ComponentModel.Composition;
using System.ComponentModel;
using System.Collections.Generic;

namespace PA.Plugin
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property), MetadataAttribute]
    public class PluginDescriptionAttribute : DescriptionAttribute
    {
        public string Name { get; private set; }
        public IEnumerable<Type> TargetType { get; private set; }

        public PluginDescriptionAttribute()
            : base(null)
        {
            this.TargetType = Type.EmptyTypes;
            this.Name = null;
        }

        public PluginDescriptionAttribute(string Description, string Rename)
            : base(Description)
        {
            this.TargetType = Type.EmptyTypes;
            this.Name = Rename;
        }

        public PluginDescriptionAttribute(string Description, string Rename, params Type[] target)
            : base(Description)
        {
            this.TargetType = target;
            this.Name = Rename;
        }

        public PluginDescriptionAttribute(string Description)
            : base(Description)
        {
            this.TargetType = Type.EmptyTypes;
            this.Name = null;
        }

        public PluginDescriptionAttribute(string Description, params Type[] target)
            : base(Description)
        {
            this.TargetType = target;
            this.Name = null;
        }
    }


}
