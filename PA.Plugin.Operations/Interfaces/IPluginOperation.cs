
using System.ComponentModel;
using System;
using System.ComponentModel.Composition;

namespace PA.Plugin.Operations.Interfaces
{
    [InheritedExport]
    [Obsolete]
    public interface IPluginOperation : IPlugin
    {
        [Obsolete]
        bool CanExecute<T>(object obj)
            where T : IPluginOperation;
        [Obsolete]
        object Execute<T>(object obj)
            where T : IPluginOperation;

        [Obsolete]
        bool CanExecute(params object[] arguments);

        [Obsolete]
        object Execute(params object[] arguments);
    }
}
