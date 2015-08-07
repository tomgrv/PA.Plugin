using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Composition;
using PA.Plugin.Components.Controls;
using PA.Plugin.Components.Interfaces;

namespace PA.Plugin
{
    [Obsolete]
    public interface IPluginSource : IPartImportsSatisfiedNotification, IPluginHandler
    {
        PluginLoader PluginHost { get; set; }
        //IPluginHelper Helper { get; set; }
        //IPluginRunner Runner { get; set; }
    }
}
