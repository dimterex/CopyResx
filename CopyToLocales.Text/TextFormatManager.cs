using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CopyToLocale.Core;
using CopyToLocale.Core.Models;
using CopyToLocale.Core.Modules;

namespace CopyToLocales.Text
{
     public class TextFormatManager : IFormatManager
    {
        #region Constants

        private const string FILE_PATH = "TextOutputs";

        #endregion Constants
        
        private readonly Dictionary<string, SupportedLocalization> _supportedLocalizationMapper;
        private readonly IList<string> _blackList;


        #region Properties

        public event EventHandler<string> StateChangeEvent;
        public string FormatType => "Text csv for xaml";

        #endregion Properties

        #region Constuctors

        public TextFormatManager()
        {

            _blackList = new List<string>()
            {
                "HelpPageName",
            };
            _supportedLocalizationMapper = new Dictionary<string, SupportedLocalization>();
            _supportedLocalizationMapper.Add(string.Empty, SupportedLocalization.None);
            _supportedLocalizationMapper.Add(".resources_ar.csv", SupportedLocalization.Arabic);
            _supportedLocalizationMapper.Add(".resources_cs.csv", SupportedLocalization.Czech);
            _supportedLocalizationMapper.Add(".resources_de.csv", SupportedLocalization.German);
            _supportedLocalizationMapper.Add(".resources_el.csv", SupportedLocalization.Greek);
            _supportedLocalizationMapper.Add(".resources_en.csv", SupportedLocalization.English_US);
            _supportedLocalizationMapper.Add(".resources_es.csv", SupportedLocalization.Spanish);
            _supportedLocalizationMapper.Add(".resources_fr.csv", SupportedLocalization.French);
            _supportedLocalizationMapper.Add(".resources_hu.csv", SupportedLocalization.Hungarian);
            _supportedLocalizationMapper.Add(".resources_it.csv", SupportedLocalization.Italian);
            _supportedLocalizationMapper.Add(".resources_ja.csv", SupportedLocalization.Japanese);
            _supportedLocalizationMapper.Add(".resources_ko.csv", SupportedLocalization.South_Korean);
            _supportedLocalizationMapper.Add(".resources_nl.csv", SupportedLocalization.Dutch);
            _supportedLocalizationMapper.Add(".resources_pl.csv", SupportedLocalization.Polish);
            _supportedLocalizationMapper.Add(".resources_pt-BR.csv", SupportedLocalization.Brazilian_Portuguese);
            _supportedLocalizationMapper.Add(".resources.csv", SupportedLocalization.Russian);
            _supportedLocalizationMapper.Add(".resources_sk.csv", SupportedLocalization.Slovak);
            _supportedLocalizationMapper.Add(".resources_sr.csv", SupportedLocalization.Serbian);
            _supportedLocalizationMapper.Add(".resources_tr.csv", SupportedLocalization.Turkish);
            _supportedLocalizationMapper.Add(".resources_uk.csv", SupportedLocalization.Ukrainian);
            _supportedLocalizationMapper.Add(".resources_zh-CHS.csv", SupportedLocalization.Simplified_Chinese);
            _supportedLocalizationMapper.Add(".resources_zh-CHT.csv", SupportedLocalization.Traditional_Chinese);
        }

        #endregion Constuctors

        #region Methods

        #endregion Methods
        
        public SupportedLocalization GetKey(string path)
        {
            foreach (var supportedLocalization in _supportedLocalizationMapper)
            {
                if (string.IsNullOrEmpty(supportedLocalization.Key))
                    continue;
                
                if (path.ToLower().EndsWith(supportedLocalization.Key))
                    return _supportedLocalizationMapper[supportedLocalization.Key];
            }
            
            return SupportedLocalization.None;
        }
        

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public Dictionary<string, ResourceModel> InitReaders(IList<SelectFileModel> selectFileViewModel)
        {
            var result = new Dictionary<string, ResourceModel>();
            foreach (var selectFileModel in selectFileViewModel)
            {
                var rawLines = File.ReadAllLines(selectFileModel.FullPath);
                foreach (var rawLine in rawLines)
                {
                    if (string.IsNullOrEmpty(rawLine))
                        continue;
                
                    var array = rawLine.Split(',');
                    if (array.Length == 7)
                    {
                        string rawKey = array[1];

                        var arrayKey = rawKey.Split(':');
                        if (_blackList.Contains(arrayKey[0]))
                            continue;
                        
                        if (arrayKey[1].StartsWith("System.String"))
                        {

                            string key = arrayKey[0];
                            string value = array[6];
                            
                            Dictionary<SupportedLocalization, string> testDic;
                    
                            if (result.ContainsKey(key))
                            {
                                testDic = result[key].KeyValueList;
                            }
                            else
                            {
                                var model = new ResourceModel(key);
                                testDic = model.KeyValueList;
                                result.Add(key, model);
                            }

                            if (testDic.ContainsKey(selectFileModel.SourceKey))
                            {
                                if (testDic[selectFileModel.SourceKey] == value)
                                {
                                    
                                }
                                else
                                {
                                    StateChangeEvent?.Invoke(this, $"Получено значение по ключу '{key}' для языка: '{selectFileModel.SourceKey}' со значением: '{value}' отличается от имеющегося: '{testDic[selectFileModel.SourceKey]}' ");
                                }
                            }
                            else
                            {
                                testDic.Add(selectFileModel.SourceKey, value);
                            }
                        }
                       
                    }
                }
            }
            

            return result;
        }

        public void Update(List<ChangedResourceModel> forUpdate, IList<SelectFileModel> targetFileList)
        {
        }

        public List<string> GetAnotherLanguages(string path)
        {
            var fileInfo = new FileInfo(path);
            return Directory.GetFiles(fileInfo.DirectoryName).Where(x => Filter(path, x)).ToList();
        }
        
        private bool Filter(string realpathfile, string filePath)
        {
            if (filePath == realpathfile)
                return false;
            
            foreach (var supportedLocalization in _supportedLocalizationMapper)
            {
                if (string.IsNullOrEmpty(supportedLocalization.Key))
                    continue;
                
                if (filePath.ToLower().EndsWith(supportedLocalization.Key))
                    return true;
            }
            
            // var key = _supportedLocalizationMapper.Keys.ToList().FirstOrDefault(filePath.ToLower().EndsWith);
            // if (!string.IsNullOrEmpty(key))
            //     return true;

            return false;
        }
        

        public void Save(Dictionary<string, SelectFileModel> sourceDictionaryEntryElements,
                         Dictionary<string, SelectFileModel> targetDictionaryEntryElements)
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


            // foreach (var item in sourceDictionaryEntryElements)
            // {
            //     StateChangeEvent?.Invoke(this, $"Выбран файл локализации: {item.Key}");
            //     foreach (KeyValuePair<string, SelectFileModel> targetDictionaryEntryElement in targetDictionaryEntryElements)
            //     {
            //         if (targetDictionaryEntryElement.Value.SourceKey == item.Key)
            //             SaveOnceFile(item.Key, item.Value, targetDictionaryEntryElement.Value, defaultSource);
            //     }
            // }
        }

        private string GetStringFormat(string key, string value)
        {
            return $"{key}\t{value}" + Environment.NewLine;
        }

        /// <summary>
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey,
                                  SelectFileModel sourceDictionaryEntryElements,
                                  SelectFileModel targetResourceReader,
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

            SaveFile(Path.Combine(FILE_PATH, "new" + itemKey + ".txt"), sb1.ToString(), exist1.ToString(), dublicate1.ToString());

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
        
        private void SaveFile(string filePath, params string[] strings)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);

            var fs = File.Create(filePath);
            var sw = new StreamWriter(fs);

            foreach (var s in strings)
                sw.Write(s);

            sw.Close();
            fs.Close();
            sw.Dispose();
            fs.Dispose();
        }
    }
}