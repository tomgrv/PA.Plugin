using PA.Plugin.Operations.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;

namespace PA.Plugin.Operations.Interfaces
{
    [InheritedExport]
    public interface IActionPlugin : IPlugin
    {
        bool CanExecute(IDictionary<string, object> contextData);
        object Execute(IDictionary<string, object> contextData);
    }
}
