using System.ComponentModel.Composition;
using System.ComponentModel;
using System;

namespace PA.Plugin
{
    [InheritedExport]
    public interface IPluginOperation : IPlugin, ICloneable
    {
        bool CanExecute<T>(object obj) where T : IPluginOperation;
        object Execute<T>(object obj) where T : IPluginOperation;
    }
}
