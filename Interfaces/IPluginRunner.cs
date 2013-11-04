using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace PA.Plugin
{
    public interface IPluginRunner :  IComponent
    {
        bool IsBusy { get;}
        void RunAsync<T>(T p, params object[] args) where T : class, IPluginOperation;
        event EventHandler Started;
        event RunWorkerCompletedEventHandler Done;
        void Cancel();
    }
}
