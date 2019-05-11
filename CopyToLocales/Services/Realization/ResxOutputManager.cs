using CopyToLocales.Core;
using CopyToLocales.Services.Interfaces;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Resources;
using CopyToLocales.ViewModel.Enums;

namespace CopyToLocales.Services.Realization
{
    public class ResxOutputManager : IOutputManager
    {
        #region Constants

        private const string EN = ".en.resx";
        private const string RU = ".ru.resx";

        private const string RESX_EXTENSION = ".resx";
        
        #endregion Constants

        #region Fields

        private readonly ILogService _logService;
        private readonly Dictionary<string, ResXResourceReader> _targetResourceReaders;
        private readonly Dictionary<string, ResXResourceWriter> _targetResourceWriters;
        private readonly Dictionary<string, ResXResourceReader> _sourceResourceReaders;

        #endregion Fields

        #region Properties

        public OutputTypes OutputType => OutputTypes.Resx;
        public List<DictionaryEntryElement> SourceDictionaryEntryElements { get; }

        #endregion Properties

        #region Constuctors

        public ResxOutputManager(ILogService logService)
        {
            _logService = logService;
            _sourceResourceReaders = new Dictionary<string, ResXResourceReader>();
            _targetResourceReaders = new Dictionary<string, ResXResourceReader>();
            _targetResourceWriters = new Dictionary<string, ResXResourceWriter>();
            SourceDictionaryEntryElements = new List<DictionaryEntryElement>();
        }

        #endregion Constuctors

        #region Methods

        #endregion Methods

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

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public void InitReaders(FileType fileType, FileInfo fileInfo)
        {
            var file = fileInfo.Name.Split('.').FirstOrDefault();
            var allResx = Directory.GetFiles(fileInfo.DirectoryName).Where(x => Filter(x, file));

            Clear(fileType);
            foreach (var resx in allResx)
            {
                var rsx = resx.Split('.');
                string name = string.Empty;
                if (rsx.Length == 2)
                    name = $".ru.{rsx[1]}"; //Используется для русского языка.
                else if (rsx.Length == 3)
                    name = $".{rsx[1]}.{rsx[2]}";

                switch (fileType)
                {
                    case FileType.Source:
                        _sourceResourceReaders.Add(name, new ResXResourceReader(resx));
                        break;
                    case FileType.Target:

                        _targetResourceReaders.Add(name, new ResXResourceReader(resx));
                        _targetResourceWriters.Add(name, new ResXResourceWriter(resx));
                        break;
                }
            }

            GetDefault(fileType);
        }

        private void Clear(FileType fileType)
        {
            switch (fileType)
            {
                case FileType.Source:
                    _sourceResourceReaders.Clear();
                    break;
                case FileType.Target:
                    _targetResourceReaders.Clear();
                    _targetResourceWriters.Clear();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(fileType), fileType, null);
            }
        }

        private void GetDefault(FileType source)
        {
            switch (source)
            {
                case FileType.Source:
                    SourceDictionaryEntryElements.Clear();
                    foreach (var resource in _sourceResourceReaders)
                    {
                        if (!resource.Key.Equals(RU))
                            continue;

                        foreach (DictionaryEntry dictionaryEntry in resource.Value)
                            SourceDictionaryEntryElements.Add(new DictionaryEntryElement(dictionaryEntry));
                        break;
                    }
                    break;
            }
        }

        public void Save()
        {
            if (!CheckNames())
                return;

            foreach (var item in _sourceResourceReaders)
            {
                _logService.AddMessage($"Выбран файл локализации: {item.Key}");
                SaveOnceFile(item.Key, item.Value, _targetResourceReaders[item.Key], _targetResourceWriters[item.Key]);
            }
        }

        /// <summary>
        /// Получить значение для английского языка.
        /// </summary>
        private object GetEnDefaultValue(string sourceName)
        {
            foreach (var item in _sourceResourceReaders)
            {
                if (!item.Key.Equals(EN))
                    continue;
                foreach (DictionaryEntry value in item.Value)
                {
                    if (value.Key.Equals(sourceName))
                        return value.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Получить ключ по значению.
        /// </summary>
        private object GetKeyFromValue(object sourceValue, string resxLang)
        {
            foreach (var item in _targetResourceReaders)
            {
                if (!item.Key.Equals(resxLang))
                    continue;
                foreach (DictionaryEntry value in item.Value)
                {
                    if (value.Value.Equals(sourceValue))
                        return value.Key;
                }
            }
            return null;
        }

        /// <summary>
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey, ResXResourceReader sourceResourceReader, ResXResourceReader targetResourceReader, ResXResourceWriter targetResourceWriter)
        {
            var newValue = new Dictionary<string, string>();
            foreach (DictionaryEntry item in sourceResourceReader)
            {
                foreach (var sourceName in SourceDictionaryEntryElements.Where(x=>x.IsCopy).ToList())
                {
                    if (!item.Key.Equals(sourceName.Key))
                        continue;

                    object enValue = GetEnDefaultValue(sourceName.Key);

                    object valueInResx = GetKeyFromValue(item.Value, itemKey);

                    if (valueInResx != null)
                    {
                        _logService.AddMessage($"Значение {item.Value} для файла {itemKey} доступно по ключу {valueInResx}.");
                        continue;
                    }

                    // TODO Добавление в  *.Designer.cs для быстрого доступа к данным ресурсам.
                    _logService.AddMessage($"Получено значение по ключу {item.Key} со значением: {item.Value} для файла {itemKey}");

                    // Переносить для английского языка в любом случае.
                    // Если значение в текущем языке совпадет со значеним для ангийского языка,
                    // значит данная фраза не локализована и нет необходимости ее переносить.
                    if (enValue == null || !item.Value.Equals(enValue) || itemKey == EN)
                        newValue.Add(sourceName.Value, item.Value.ToString());
                }
            }

            if (newValue.Count > 0)
                SaveNewValue(targetResourceReader, targetResourceWriter, newValue);
        }

        /// <summary>
        /// Записать зничения в файл.
        /// </summary>
        private void SaveNewValue(ResXResourceReader targetResourceReader, ResXResourceWriter targetResourceWriter, Dictionary<string, string> keyValuePairs)
        {
            try
            {
                var node = targetResourceReader.GetEnumerator();
                while (node.MoveNext())
                    targetResourceWriter.AddResource(node.Key.ToString(), node.Value.ToString());

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

        public bool CheckNames()
        {
            foreach (DictionaryEntryElement sourceDictionaryEntryElement in SourceDictionaryEntryElements)
            {
                if (!sourceDictionaryEntryElement.IsCopy)
                    continue;

                _logService.AddMessage($"Старый ключ: {sourceDictionaryEntryElement.Key}, " +
                                     $"новый ключ: {sourceDictionaryEntryElement.NewKey}");

                if (string.IsNullOrWhiteSpace(sourceDictionaryEntryElement.NewKey))
                    return false;
            }

            return true;
        }
    }
}
