using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;

namespace PA.Plugin
{
    public interface IPluginSource : IPartImportsSatisfiedNotification, IPluginHandler
    {
        PluginHost PluginHost { get; set; }
        //IPluginHelper Helper { get; set; }
        //IPluginRunner Runner { get; set; }
    }
}
