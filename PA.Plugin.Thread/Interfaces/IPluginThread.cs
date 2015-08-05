using System.Diagnostics;
using System.ComponentModel.Composition;

namespace PA.Plugin.Threads.Interfaces
{
    [InheritedExport]
    public interface IPluginThread : IPlugin
    {
        int ThreadId { get; }
        bool IsRunning { get; }

        bool Start();
        bool Stop();
        bool Pause();

    }
}
