using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace PA.Plugin
{
    [InheritedExport]
    public interface IPlugin : IDisposable
    {       
    }
}
