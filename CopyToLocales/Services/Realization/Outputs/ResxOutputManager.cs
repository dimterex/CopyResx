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
    using System.Resources;

    using ViewModel;

    public class ResxOutputManager : IOutputManager
    {
        #region Constants

        private const string EN = ".en.resx";
        private const string RU = ".ru.resx";
        
        #endregion Constants

        #region Fields

        private readonly ILogService _logService;
        private readonly Dictionary<string, ResXResourceWriter> _targetResourceWriters;

        #endregion Fields

        #region Properties

        public OutputTypes OutputType => OutputTypes.Resx;

        #endregion Properties

        #region Constuctors

        public ResxOutputManager(ILogService logService)
        {
            _logService = logService;
            _targetResourceWriters = new Dictionary<string, ResXResourceWriter>();
        }

        #endregion Constuctors

        #region Methods

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public string InitReaders(FileType fileType, SelectFileViewModel selectFileViewModel)
        {
            var fileInfo = new FileInfo(selectFileViewModel.FullPath);
            var rsx = fileInfo.Name.Split('.');
            string name = string.Empty;
            if (rsx.Length == 2)
                name = $".ru.{rsx[1]}"; //Используется для русского языка.
            else if (rsx.Length == 3)
                name = $".{rsx[1]}.{rsx[2]}";

            foreach (DictionaryEntry dictionaryEntry in new ResXResourceReader(selectFileViewModel.FullPath))
                selectFileViewModel.DictionaryEntryElements.Add(new DictionaryEntryElement(dictionaryEntry));

            switch (fileType)
            {
                case FileType.Target:
                    _targetResourceWriters.Add(name, new ResXResourceWriter(selectFileViewModel.FullPath));
                    break;
            }

            return name;
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
                SaveOnceFile(item.Key, 
                    item.Value.DictionaryEntryElements, 
                    targetDictionaryEntryElements[item.Key].DictionaryEntryElements, 
                    _targetResourceWriters[item.Key],
                    defaultSource);
            }
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
        /// Получить ключ по значению.
        /// </summary>
        private object GetKeyFromValue(object sourceValue, List<DictionaryEntryElement> targetResourceReader)
        {
            foreach (DictionaryEntryElement value in targetResourceReader)
            {
                if (value.Value.Equals(sourceValue))
                    return value.Key;
            }
            return null;
        }

        /// <summary>
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey, List<DictionaryEntryElement> sourceResourceReader,
                                  List<DictionaryEntryElement> targetResourceReader,
                                  ResXResourceWriter targetResourceWriter,
                                  List<DictionaryEntryElement> defaultSourceElements)
        {
            var newValue = new Dictionary<string, string>();
            foreach (DictionaryEntryElement sourceName in sourceResourceReader.Where(x => x.IsCopy).ToList())
            {
                object enValue = GetEnDefaultValue(sourceName.Key, defaultSourceElements);

                object valueInResx = GetKeyFromValue(sourceName.Value, targetResourceReader);

                if (valueInResx != null)
                {
                    _logService.AddMessage($"Значение {sourceName.Value} для файла {itemKey} доступно по ключу {valueInResx}.");
                    continue;
                }

                //// TODO Добавление в  *.Designer.cs для быстрого доступа к данным ресурсам.
                _logService.AddMessage($"Получено значение по ключу {sourceName.Key} со значением: {sourceName.Value} для файла {itemKey}");

                //// Переносить для английского языка в любом случае.
                //// Если значение в текущем языке совпадет со значеним для ангийского языка,
                //// значит данная фраза не локализована и нет необходимости ее переносить.
                if (enValue == null || !sourceName.Value.Equals(enValue) || itemKey == EN)
                    newValue.Add(sourceName.NewKey, sourceName.Value);
            }

            if (newValue.Count > 0)
                SaveNewValue(targetResourceReader, targetResourceWriter, newValue);
        }

        /// <summary>
        /// Записать зничения в файл.
        /// </summary>
        private void SaveNewValue(List<DictionaryEntryElement> targetResourceReader, ResXResourceWriter targetResourceWriter, Dictionary<string, string> keyValuePairs)
        {
            try
            {
                foreach (DictionaryEntryElement entryElement in targetResourceReader)
                    targetResourceWriter.AddResource(entryElement.Key, entryElement.Value);

                foreach (var keyValuePair in keyValuePairs)
                {
                    targetResourceWriter.AddResource(new ResXDataNode(keyValuePair.Key, keyValuePair.Value));
                    _logService.AddMessage($"Добавлен ключ: {keyValuePair.Key} со значением: {keyValuePair.Value}");
                }

                targetResourceWriter.Generate();
                targetResourceWriter.Close();

                _logService.AddMessage($"Все значения записаны.");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private bool CheckNames(Dictionary<string, SelectFileViewModel> sourceDictionaryEntryElements)
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

        #endregion Methods
    }
}
