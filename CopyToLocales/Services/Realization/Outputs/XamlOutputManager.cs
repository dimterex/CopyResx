namespace CopyToLocales.Services.Realization
{
    using CopyToLocales.Core;
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.ViewModel.Enums;

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Markup;

    using ViewModel;

    public class XamlOutputManager : IOutputManager
    {
        #region Constants

        private const string FILE_PATH = "XamlOutputs";

        #endregion

        #region Fields

        private readonly ILogService _logService;
        private readonly IFileManager _fileManager;

        private readonly string _header;
        private readonly string _bottom;

        #endregion Fields

        #region Properties

        public OutputTypes OutputType => OutputTypes.Xaml;

        #endregion Properties

        #region Constuctors

        public XamlOutputManager(ILogService logService, IFileManager fileManager)
        {
            _header = $"<ResourceDictionary" + Environment.NewLine +
                            "x: Uid = \"StringDictionary\" xmlns = \"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" + Environment.NewLine +
                            "xmlns: x = \"http://schemas.microsoft.com/winfx/2006/xaml\" " + Environment.NewLine +
                            "xmlns: system = \"clr-namespace:System;assembly=mscorlib\" >" + Environment.NewLine;

            _bottom = $"</ResourceDictionary>" + Environment.NewLine;
            _logService = logService;
            _fileManager = fileManager;
        }

        #endregion Constuctors

        #region Methods

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public string InitReaders(FileType fileType, SelectFileViewModel selectFileViewModel)
        {
            var all = File.ReadAllText(selectFileViewModel.FullPath);

            ResourceDictionary resourceDictionary = null;
            try
            {
                if (!(XamlReader.Parse(all) is ResourceDictionary resourceDictionary1))
                {
                    _logService.AddMessage($"Ошибка преобразования {all} в ResourceDictionary.");
                    return string.Empty;
                }

                resourceDictionary = resourceDictionary1;
            }
            catch (Exception e)
            {
                _logService.AddMessage($"Ошибка преобразования {all} в ResourceDictionary.");
                return string.Empty;
            }

            foreach (var item in resourceDictionary.OfType<DictionaryEntry>())
            {
                selectFileViewModel.DictionaryEntryElements.Add(new DictionaryEntryElement(item));
            }

            return selectFileViewModel.FullPath;
        }

        public void Save(Dictionary<string, SelectFileViewModel> sourceDictionaryEntryElements,
                         Dictionary<string, SelectFileViewModel> targetDictionaryEntryElements)
        {
            if (!CheckNames(sourceDictionaryEntryElements))
                return;

            List<DictionaryEntryElement> defaultSource  = sourceDictionaryEntryElements.FirstOrDefault().Value.DictionaryEntryElements;

            foreach (var item in sourceDictionaryEntryElements)
            {
                _logService.AddMessage($"Выбран файл локализации: {item.Key}");
                foreach (KeyValuePair<string, SelectFileViewModel> targetDictionaryEntryElement in targetDictionaryEntryElements)
                {
                    if (targetDictionaryEntryElement.Value.SourceKey == item.Key)
                        SaveOnceFile(item.Key, item.Value, targetDictionaryEntryElement.Value, defaultSource);
                }
            }
        }

        /// <summary>
        /// Получить ключ по значению.
        /// </summary>
        private object GetKeyFromValue(object sourceValue, List<DictionaryEntryElement> defaultDict)
        {
            foreach (DictionaryEntryElement item in defaultDict)
            {
                if (item.Value.Equals(sourceValue))
                    return item.Key;
            }
            return null;
        }

        /// <summary>
        /// Получить ключ по значению.
        /// </summary>
        private object GetValueFromKey(object sourceKey, List<DictionaryEntryElement> targetDic)
        {
            foreach (DictionaryEntryElement item in targetDic)
            {
                if (item.Key.Equals(sourceKey))
                    return item.Value;
            }

            return null;
        }

        private string GetStringFormat(string key, string value)
        {
            return $"<system:String x:Uid=\"{key}\" x:Key=\"{key}\">{value}</system:String>" + Environment.NewLine;
        }

        /// <summary>
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey,
                                  SelectFileViewModel sourceDictionaryEntryElements,
                                  SelectFileViewModel targetResourceReader,
                                  List<DictionaryEntryElement> defaultSourceElements)
        {
            var sb = new StringBuilder();
            var exist = new StringBuilder();
            var dublicate = new StringBuilder();
            var keys = new StringBuilder();
            sb.Append($"<!-- ============================== Не содержаться -->" + Environment.NewLine);
            exist.Append($"<!-- ============================== Уже содержаться в данном файле -->" + Environment.NewLine);
            dublicate.Append($"<!-- ============================== Измененный ключ -->" + Environment.NewLine);
            keys.Append($"<!-- ============================== Копии ключа -->" + Environment.NewLine);

            foreach (var sourceName in sourceDictionaryEntryElements.DictionaryEntryElements.Where(x => x.IsCopy).ToList())
            {
                object valueInResx = GetKeyFromValue(sourceName.Value, targetResourceReader.DictionaryEntryElements);
                var keyInXaml = GetValueFromKey(sourceName.Key, targetResourceReader.DictionaryEntryElements);

                if (valueInResx != null && keyInXaml == null)
                {
                    keys.Append($"{valueInResx} => {sourceName.Key}{Environment.NewLine}");
                    dublicate.Append(GetStringFormat(sourceName.Key, sourceName.Value));
                    continue;
                }

                if (valueInResx != null)
                {
                    //keys.Append($"{valueInResx} => {sourceName.Key}{Environment.NewLine}");
                    exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
                    _logService.AddMessage($"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
                    continue;
                }

                if (keyInXaml != null)
                {
                    //keys.Append($"{keyInXaml} => {sourceName.Key}{Environment.NewLine}");
                    exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
                    _logService.AddMessage($"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
                    continue;
                }

                _logService.AddMessage($"Получено значение по ключу {sourceName.Key} со значением: {sourceName.Value}");
                sb.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            }

            _fileManager.SaveFile(Path.Combine(FILE_PATH, "new" + itemKey + ".xaml"), 
                _header, sb.ToString(), exist.ToString(), dublicate.ToString(), _bottom, keys.ToString());
        }

        private bool CheckNames(Dictionary<string, SelectFileViewModel> dictionary)
        {
            foreach (KeyValuePair<string, SelectFileViewModel> sourceDictionaryEntryElement in dictionary)
            {
                foreach (DictionaryEntryElement dictionaryEntryElement in sourceDictionaryEntryElement.Value.DictionaryEntryElements)
                {
                    if (!dictionaryEntryElement.IsCopy)
                        continue;

                    _logService.AddMessage($"Старый ключ: {dictionaryEntryElement.Key}, " + $"новый ключ: {dictionaryEntryElement.NewKey}");

                    if (string.IsNullOrWhiteSpace(dictionaryEntryElement.NewKey))
                        return false;
                }
            }
            return true;
        }

        #endregion Methods
    }
}
