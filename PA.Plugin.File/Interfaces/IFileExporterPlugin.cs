
using PA.Plugin.File;
using System.IO;
namespace PA.Plugin.File.Interfaces
{

    public interface IFileExporterPlugin: IFilePlugin
    {
        void Save(string Filename, object Data);
    }

   
}
