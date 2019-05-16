namespace CopyToLocales.Services.Realization
{
    using System;

    using CopyToLocales.Core;
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.ViewModel.Enums;

    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ViewModel;

    public class TextOutputManager : IOutputManager
    {
        #region Constants

        private const string FILE_PATH = "TextOutputs";
        private const string EN = ".en.resx";

        #endregion Constants

        #region Fields

        private readonly ILogService _logService;
        private readonly IFileManager _fileManager;

        #endregion Fields

        #region Properties

        public OutputTypes OutputType => OutputTypes.Text;

        #endregion Properties

        #region Constuctors

        public TextOutputManager(ILogService logService, IFileManager fileManager)
        {
            _logService = logService;
            _fileManager = fileManager;
        }

        #endregion Constuctors

        #region Methods

        #endregion Methods

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public string InitReaders(FileType fileType, SelectFileViewModel selectFileViewModel)
        {
            var txt = File.ReadAllLines(selectFileViewModel.FullPath);

            foreach (var keyValuePair in txt)
            {
                if (string.IsNullOrEmpty(keyValuePair))
                    continue;

                var keyValue = keyValuePair.Split('\t');

                string key = keyValue[0];
                string value = keyValue[1];

                selectFileViewModel.DictionaryEntryElements.Add(new DictionaryEntryElement(key, value));
            }

            return selectFileViewModel.FullPath;
        }
        

        public void Save(Dictionary<string, SelectFileViewModel> sourceDictionaryEntryElements,
                         Dictionary<string, SelectFileViewModel> targetDictionaryEntryElements)
        {
            if (!CheckNames(sourceDictionaryEntryElements))
                return;

            List<DictionaryEntryElement> defaultSource = null;

            foreach (var item in sourceDictionaryEntryElements)
            {
                if (!item.Key.Equals(EN))
                    continue;

                defaultSource = item.Value.DictionaryEntryElements;
                break;
            }

            if (defaultSource == null)
                defaultSource = sourceDictionaryEntryElements.FirstOrDefault().Value.DictionaryEntryElements;


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

        private string GetStringFormat(string key, string value)
        {
            return $"{key}\t{value}" + Environment.NewLine;
        }

        /// <summary>
        /// Получить значение для английского языка.
        /// </summary>
        private object GetEnDefaultValue(string sourceName, List<DictionaryEntryElement> defaultSourceDictionaryEntryElements)
        {
            foreach (DictionaryEntryElement value in defaultSourceDictionaryEntryElements)
            {
                if (value.Key.Equals(sourceName))
                    return value.Value;
            }
            return null;
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
          
            foreach (var sourceName in sourceDictionaryEntryElements.DictionaryEntryElements.Where(x => x.IsCopy).ToList())
            {
                object valueInResx = GetKeyFromValue(sourceName.Value, targetResourceReader.DictionaryEntryElements);

                var t = GetEnDefaultValue(sourceName.Key, defaultSourceElements);
                var keyInXaml = GetValueFromKey(t?.ToString() ?? sourceName.Key, targetResourceReader.DictionaryEntryElements);
   

                var tValue = string.Empty;
                if (t != null)
                    tValue = GetKeyFromValue(t.ToString(), defaultSourceElements)?.ToString();

                if (t != null && (valueInResx != null || keyInXaml != null))
                {
                    exist.Append(GetStringFormat(t.ToString(), sourceName.Value));
                    continue; 
                }

                if (valueInResx != null && keyInXaml == null)
                {
                    sb.Append(GetStringFormat(valueInResx.ToString(), sourceName.Value));
                    continue;
                }

                //if (valueInResx != null)
                //{
                //    exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
                //    _logService.AddMessage($"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
                //    continue;
                //}

                if (keyInXaml != null)
                {
                    exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
                    _logService.AddMessage($"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
                    continue;
                }

                _logService.AddMessage($"Получено значение по ключу {sourceName.Key} со значением: {sourceName.Value}");
                if (!string.IsNullOrEmpty(tValue) )
                {
                    if(!t.Equals(sourceName.Value))
                        sb.Append(GetStringFormat(t.ToString(), sourceName.Value));
                }
                else
                {
                    sb.Append(GetStringFormat(sourceName.Key, sourceName.Value));
                }
            }

            var sb1 = new StringBuilder();
            var exist1 = new StringBuilder();
            var dublicate1 = new StringBuilder();

            sb1.Append($"============================== Не содержаться" + Environment.NewLine);
            exist1.Append($"============================== Уже содержаться в данном файле" + Environment.NewLine);
            dublicate1.Append($"============================== Измененный ключ" + Environment.NewLine);

            sb1.Append(sb);
            exist1.Append(exist);
            dublicate.Append(dublicate);

            _fileManager.SaveFile(Path.Combine(FILE_PATH, "new" + itemKey + ".txt"), sb1.ToString(), exist1.ToString(), dublicate1.ToString());

            if (!targetResourceReader.FullPath.EndsWith(".txt"))
                return;

            var strings = new[]
            {
                sb.ToString(),
                //exist.ToString(),
                //dublicate.ToString()
            };

            using (StreamWriter stream = new StreamWriter(targetResourceReader.FullPath, true, Encoding.Unicode))
            {
                foreach (string s in strings)
                {
                    stream.WriteLine(s);
                }
            }
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

        public bool CheckNames(Dictionary<string, SelectFileViewModel> sourceDictionaryEntryElements)
        {
            foreach (KeyValuePair<string, SelectFileViewModel> sourceDictionaryEntryElement in sourceDictionaryEntryElements)
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
    }
}
