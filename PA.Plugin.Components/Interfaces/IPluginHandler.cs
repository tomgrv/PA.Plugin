using PA.Plugin.Components;
using PA.Plugin.Components.Controls;
using PA.Plugin.Operations.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;

namespace PA.Plugin.Components.Interfaces
{
    public interface IPluginHandler : IPartImportsSatisfiedNotification, IPluginProvider
    {
        PluginLoader Loader { get; set; }
        void BuildWithType<T>() where T : IPlugin;
        event EventHandler<PluginEventArgs> PluginChanged;
    }
}
