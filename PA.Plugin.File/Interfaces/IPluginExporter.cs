using System;
using System.Collections.Generic;
using System.Text;

namespace Toolbox.Plugin.Interfaces
{

    public interface IPluginExporter: IPluginFile
    {
        void Save(object Data);
        void Save(object Data, string Filename);
    }

    public interface IPluginExporter<T> : IPluginFile
    {
        void Save(T Data);
        void Save(T Data, string Filename);
    }
}
