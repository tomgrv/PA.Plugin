using System.ComponentModel.Composition;
using System.ComponentModel;
using System;

namespace PA.Plugin
{
    [InheritedExport]
    public interface IPluginOperation : IPlugin, ICloneable
    {
        [Obsolete]
        bool CanExecute<T>(object obj)
            where T : IPluginOperation;
        [Obsolete]
        object Execute<T>(object obj)
            where T : IPluginOperation;

        bool CanExecute(params object[] arguments);
        bool CanExecute<T>(params object[] arguments)
           where T : IPluginOperation;
        R Execute<R>(params object[] arguments);
        R Execute<R, T>(params object[] arguments)
            where T : IPluginOperation;
    }
}
