using System.IO;
using System.ComponentModel.Composition;
using PA.Plugin.Operations.Interfaces;

namespace PA.Plugin.File.Interfaces
{
    [InheritedExport]
    public interface IFilePlugin : IPlugin, IJobPlugin
    {
        FileInfo File { get; }
    }
}
