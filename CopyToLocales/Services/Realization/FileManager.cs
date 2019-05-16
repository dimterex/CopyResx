using CopyToLocales.Services.Interfaces;

using System.IO;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace CopyToLocales.Services.Realization
{
    public class FileManager : IFileManager
    {
        #region Methods

        /// <summary>
        /// Указать папку.
        /// </summary>
        public FileInfo OpenFolderPath()
        {
            OpenFileDialog folderBrowserDialog = new OpenFileDialog();

            if (folderBrowserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                return new FileInfo(folderBrowserDialog.FileName);

            return null;
        }

        public Settings.Settings GetSettings(string filepath)
        {
            if (File.Exists(filepath))
            {
                string tmp = string.Empty;

                using (StreamReader r = new StreamReader(filepath))
                    tmp = r.ReadToEnd();

                return JsonConvert.DeserializeObject<Settings.Settings>(tmp);
            }

            return new Settings.Settings();
        }

        public void SaveSettings(string filepath, Settings.Settings settings)
        {
            var tmp = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(filepath, tmp);
        }

        public void SaveFile(string filePath, params string[] strings)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            FileStream fs = File.Create(filePath);
            StreamWriter sw = new StreamWriter(fs);

            foreach (string s in strings)
                sw.Write(s);
           
            sw.Close();
            fs.Close();
            sw.Dispose();
            fs.Dispose();
        }

        #endregion Methods
    }
}
