using System.ComponentModel.Composition;
using System.ComponentModel;
using System;

namespace PA.Plugin
{
    [InheritedExport]
    public interface IPluginAction : IPlugin, ICloneable //IPluginAction must be cloneable to save parameters during async operation
    {
        U Modify<T, U>(U obj)
            where T : class, IPluginOperation
            where U : class;
    }
}
