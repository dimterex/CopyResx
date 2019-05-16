using System.IO;

namespace CopyToLocales.Services.Interfaces
{
    public interface IFileManager
    {
        FileInfo OpenFolderPath();

        Settings.Settings GetSettings(string filepath);

        void SaveSettings(string filepath, Settings.Settings settings);
        void SaveFile(string combine, params string[] strings);
    }
}