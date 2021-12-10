using System;
using System.Collections.Generic;
using System.Text;
using PA.Plugin.File;

namespace PA.Plugin.File.Interfaces
{
    public interface IFileImporterPlugin : IFilePlugin
    {
        object Load(string Filename);
    }
}
