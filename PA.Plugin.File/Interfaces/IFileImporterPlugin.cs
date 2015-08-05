using System;
using System.Collections.Generic;
using System.Text;
using PA.Plugin.File;

namespace PA.Plugin.File.Interfaces
{
    public interface IPluginImporter : IFilePlugin
    {
        object Load(string Filename);
    }
}
