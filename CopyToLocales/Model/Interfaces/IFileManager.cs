namespace CopyToLocales.Model.Interfaces
{
    using System.IO;
    using CopyToLocales.DAL;

    public interface IFileManager
    {
        FileInfo OpenFolderPath();

        SettingsDto GetSettings(string filepath);

        void SaveSettings(string filepath, SettingsDto settingsDto);
        void SaveFile(string combine, params string[] strings);
    }
}