using CopyToLocale.Core.Models;
using CopyToLocale.Core.Modules;

namespace CopyToLocales.Model.Outputs
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel;

    public class TextFormatManager : IFormatManager
    {
        #region Constants

        private const string FILE_PATH = "TextOutputs";
        private const string EN = ".en.resx";

        #endregion Constants

        #region Fields

        private readonly IFileManager _fileManager;

        #endregion Fields

        #region Properties

        public event EventHandler<string> StateChangeEvent;
        public string FormatType => "Text";

        #endregion Properties

        #region Constuctors

        public TextFormatManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
        }

        #endregion Constuctors

        #region Methods

        #endregion Methods

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public Dictionary<string, ResourceModel> InitReaders(IList<SelectFileModel> selectFileViewModel)
        {
            // var txt = File.ReadAllLines(selectFileViewModel.FullPath);
            //
            // foreach (var keyValuePair in txt)
            // {
            //     if (string.IsNullOrEmpty(keyValuePair))
            //         continue;
            //
            //     var keyValue = keyValuePair.Split('\t');
            //
            //     string key = keyValue[0];
            //     string value = keyValue[1];
            //
            //     selectFileViewModel.DictionaryEntryElements.Add(key, value);
            // }

            return new Dictionary<string, ResourceModel>();
        }

        public void Update(List<ChangedResourceModel> forUpdate, IList<SelectFileModel> targetFileList)
        {
        }

        public List<string> GetAnotherLanguages(string path)
        {
            return new List<string>();
        }

        public void Save(Dictionary<string, SelectFileViewModel> sourceDictionaryEntryElements,
                         Dictionary<string, SelectFileViewModel> targetDictionaryEntryElements)
        {
           Dictionary<string, string> defaultSource = null;

            // foreach (var item in sourceDictionaryEntryElements)
            // {
            //     if (!item.Key.Equals(EN))
            //         continue;
            //
            //     defaultSource = item.Value.DictionaryEntryElements;
            //     break;
            // }
            //
            // if (defaultSource == null)
            //     defaultSource = sourceDictionaryEntryElements.FirstOrDefault().Value.DictionaryEntryElements;


            foreach (var item in sourceDictionaryEntryElements)
            {
                StateChangeEvent?.Invoke(this, $"Выбран файл локализации: {item.Key}");
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
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey,
                                  SelectFileViewModel sourceDictionaryEntryElements,
                                  SelectFileViewModel targetResourceReader,
                                  Dictionary<string, string> defaultSourceElements)
        {
            var sb = new StringBuilder();
            var exist = new StringBuilder();
            var dublicate = new StringBuilder();
          
            // foreach (var sourceName in sourceDictionaryEntryElements.DictionaryEntryElements.Where(x => x.IsCopy).ToList())
            // {
            //     object valueInResx = GetKeyFromValue(sourceName.Value, targetResourceReader.DictionaryEntryElements);
            //
            //     var t = GetEnDefaultValue(sourceName.Key, defaultSourceElements);
            //     var keyInXaml = GetValueFromKey(t?.ToString() ?? sourceName.Key, targetResourceReader.DictionaryEntryElements);
            //
            //
            //     var tValue = string.Empty;
            //     if (t != null)
            //         tValue = GetKeyFromValue(t.ToString(), defaultSourceElements)?.ToString();
            //
            //     if (t != null && (valueInResx != null || keyInXaml != null))
            //     {
            //         exist.Append(GetStringFormat(t.ToString(), sourceName.Value));
            //         continue; 
            //     }
            //
            //     if (valueInResx != null && keyInXaml == null)
            //     {
            //         sb.Append(GetStringFormat(valueInResx.ToString(), sourceName.Value));
            //         continue;
            //     }
            //
            //     //if (valueInResx != null)
            //     //{
            //     //    exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            //     //    StateChangeEvent?.Invoke(this, $"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
            //     //    continue;
            //     //}
            //
            //     if (keyInXaml != null)
            //     {
            //         exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            //         StateChangeEvent?.Invoke(this, $"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
            //         continue;
            //     }
            //
            //     StateChangeEvent?.Invoke(this, $"Получено значение по ключу {sourceName.Key} со значением: {sourceName.Value}");
            //     if (!string.IsNullOrEmpty(tValue) )
            //     {
            //         if(!t.Equals(sourceName.Value))
            //             sb.Append(GetStringFormat(t.ToString(), sourceName.Value));
            //     }
            //     else
            //     {
            //         sb.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            //     }
            // }

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
    }
}
