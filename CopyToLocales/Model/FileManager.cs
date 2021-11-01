using System.Xml;
using Microsoft.Win32;

namespace CopyToLocales.Model
{
    using System.IO;
    using CopyToLocales.DAL;
    using CopyToLocales.Model.Interfaces;
    using Newtonsoft.Json;

    public class FileManager : IFileManager
    {
        #region Methods

        /// <summary>
        /// Указать папку.
        /// </summary>
        public FileInfo OpenFolderPath()
        {
            var folderBrowserDialog = new OpenFileDialog();

            if (folderBrowserDialog.ShowDialog() == true)
                return new FileInfo(folderBrowserDialog.FileName);

            return null;
        }

        public SettingsDto GetSettings(string filepath)
        {
            if (File.Exists(filepath))
            {
                var tmp = string.Empty;

                using (var r = new StreamReader(filepath))
                {
                    tmp = r.ReadToEnd();
                }

                return JsonConvert.DeserializeObject<SettingsDto>(tmp);
            }

            return new SettingsDto();
        }

        public void SaveSettings(string filepath, SettingsDto settingsDto)
        {
            var tmp = JsonConvert.SerializeObject(settingsDto, Formatting.Indented);
            File.WriteAllText(filepath, tmp);
        }

        #endregion
    }
}