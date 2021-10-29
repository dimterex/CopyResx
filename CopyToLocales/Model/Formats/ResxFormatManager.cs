namespace CopyToLocales.Model.Outputs
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Resources;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel;
    using CopyToLocales.ViewModel.Common;
    using CopyToLocales.ViewModel.Enums;

    public class ResxFormatManager : IFormatManager
    {
        #region Constants

        private const string RESX_EXTENSION = ".resx";

        #endregion

        #region Fields

        private readonly List<string> _blackList;

        private readonly ILogService _logService;

        #endregion

        #region Properties

        public FormatTypes FormatType => FormatTypes.Resx;

        #endregion

        #region Constructors

        public ResxFormatManager(ILogService logService, ISettingsManager settingsManager)
        {
            _logService = logService;
            _blackList = settingsManager.GetBlackList();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public Dictionary<string, ResourceModel> InitReaders(IList<SelectFileViewModel> selectFileViewModels)
        {
            var tt = new Dictionary<string, ResourceModel>();

            foreach (var selectFileViewModel in selectFileViewModels)
            foreach (DictionaryEntry dictionaryEntry in new ResXResourceReader(selectFileViewModel.FullPath))
            {
                if (_blackList.Any(x => dictionaryEntry.Key.ToString().EndsWith(x)))
                    continue;

                var strKey = dictionaryEntry.Key.ToString();

                if (tt.ContainsKey(strKey))
                {
                    tt[strKey].KeyValueList.Add(selectFileViewModel.SourceKey, dictionaryEntry.Value.ToString());
                }
                else
                {
                    var newvalue = new ResourceModel(strKey);
                    newvalue.KeyValueList.Add(selectFileViewModel.SourceKey, dictionaryEntry.Value.ToString());
                    tt.Add(strKey, newvalue);
                }
            }

            return tt;
        }

        public void Update(List<ChangedResourceViewModel> forUpdate, IList<SelectFileViewModel> targetFileList)
        {
            var readList = new Dictionary<string, List<DictionaryEntry>>();
            var writers = new Dictionary<string, ResXResourceWriter>();

            foreach (var fileViewModel in targetFileList)
            {
                var selectFileViewModel =
                    targetFileList.FirstOrDefault(x => x.SourceKey.Equals(fileViewModel.SourceKey));
                var writer = new ResXResourceWriter(selectFileViewModel.FullPath);

                writers.Add(fileViewModel.SourceKey, writer);

                using (var reader = new ResXResourceReader(selectFileViewModel.FullPath))
                {
                    var resx = reader.Cast<DictionaryEntry>().ToList();
                    readList.Add(fileViewModel.SourceKey, resx);

                    reader.Close();
                }
            }

            foreach (var keyValuePair in forUpdate)
                foreach (var valuePair in keyValuePair.KeyValueList)
                    SaveValue(keyValuePair.NewKey, valuePair.Value, readList[valuePair.Key]);

            foreach (var writer in writers)
            {
                var list = readList[writer.Key];
                foreach (var dictionaryEntry in list)
                    writer.Value.AddResource(dictionaryEntry.Key.ToString(), dictionaryEntry.Value);
                writer.Value.Generate();
                writer.Value.Close();
                // TODO Добавление в  *.Designer.cs для быстрого доступа к данным ресурсам.
            }

            writers.Clear();
            _logService.AddMessage("Все значения записаны.");
        }

        public List<string> GetAnotherLanguages(string path)
        {
            var fileInfo = new FileInfo(path);
            var file = fileInfo.Name.Split('.').FirstOrDefault();
            return Directory.GetFiles(fileInfo.DirectoryName).Where(x => Filter(x, file)).ToList();
        }

        private bool Filter(string filePath, string name)
        {
            if (string.IsNullOrEmpty(name))
                return true;

            var fi = new FileInfo(filePath);

            var fileName = fi.Name.Split('.').FirstOrDefault();

            if (name.Equals(fileName) && fi.Extension.Equals(RESX_EXTENSION))
                return true;

            return false;
        }

        private void SaveValue(string key, string value, List<DictionaryEntry> resourceReader)
        {
            var existingResource = resourceReader.FirstOrDefault(r => r.Key.ToString() == key);
            if (existingResource.Key == null && existingResource.Value == null)
            {
                resourceReader.Add(new DictionaryEntry {Key = key, Value = value});
                _logService.AddMessage($"Добавлен ключ: {key} со значением: {value}");
            }
            else
            {
                var modifiedResx = new DictionaryEntry {Key = existingResource.Key, Value = value};
                var index = resourceReader.IndexOf(existingResource);
                resourceReader.Remove(existingResource);
                resourceReader.Insert(index, modifiedResx);
            }
        }

        #endregion
    }
}