using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using CopyToLocale.Core;
using CopyToLocale.Core.Models;
using CopyToLocale.Core.Modules;

namespace CopyToLocales.Xaml
{
    public class XamlFormatManager : IFormatManager
    {
        #region Constants

        private const string FILE_PATH = "XamlOutputs";

        #endregion

        #region Fields

        private readonly string _header;
        private readonly string _bottom;

        #endregion Fields

        #region Properties

        public event EventHandler<string> StateChangeEvent;
        public string FormatType => "Xaml";

        #endregion Properties

        #region Constuctors

        public XamlFormatManager()
        {
            _header = $"<ResourceDictionary" + Environment.NewLine +
                            "x: Uid = \"StringDictionary\" xmlns = \"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"" + Environment.NewLine +
                            "xmlns: x = \"http://schemas.microsoft.com/winfx/2006/xaml\" " + Environment.NewLine +
                            "xmlns: system = \"clr-namespace:System;assembly=mscorlib\" >" + Environment.NewLine;

            _bottom = $"</ResourceDictionary>" + Environment.NewLine;
        }

        #endregion Constuctors

        #region Methods
        
        
        public SupportedLocalization GetKey(string path)
        {
            return SupportedLocalization.Russian;
        }
        

        public Dictionary<string, ResourceModel> InitReaders(IList<SelectFileModel> selectFileViewModel)
        {
            var ls = new Dictionary<string, ResourceModel>();

            foreach (var selectFileModel in selectFileViewModel)
            {
                XmlDocument itemDoc = new XmlDocument();
                itemDoc.Load(selectFileModel.FullPath);

                // iterate through top-level elements
                foreach (XmlNode itemNode in itemDoc.DocumentElement.ChildNodes)
                {
                    // because we know that the node is an element, we can do this:
                    if (itemNode is not XmlElement itemElement)
                        continue;
                    if (!itemElement.OuterXml.StartsWith("<system:String"))
                        continue;
                    
                    var value = itemElement.InnerXml;
                    var key = itemElement.Attributes["x:Key"]?.Value;
                    
                    if (ls.ContainsKey(key))
                    {
                        ls[key].KeyValueList.Add(selectFileModel.SourceKey, value);
                    }
                    else
                    {
                        var model = new ResourceModel(key);
                        model.KeyValueList.Add(selectFileModel.SourceKey, value);
                        
                        ls.Add(key, model);
                    }
                }
                
                // ResourceDictionary resourceDictionary = null;
                // try
                // {
                //     if (!(XamlReader.Parse(all) is ResourceDictionary resourceDictionary1))
                //     {
                //         StateChangeEvent?.Invoke(this, $"Ошибка преобразования {all} в ResourceDictionary.");
                //         continue;
                //     }
                //
                //     resourceDictionary = resourceDictionary1;
                // }
                // catch (Exception e)
                // {
                //     StateChangeEvent?.Invoke(this, $"Ошибка преобразования {all} в ResourceDictionary.");
                //     continue;
                // }

                // foreach (var item in resourceDictionary.OfType<DictionaryEntry>())
                // {
                //     selectFileViewModel.DictionaryEntryElements.Add(new DictionaryEntryElement(item));
                // }

                // foreach (var item in resourceDictionary)
                // {
                //     selectFileModel.DictionaryEntryElements.Add(new CopyResourceModel());
                // }
            }

            return ls;
        }

        public void Update(List<ChangedResourceModel> forUpdate, IList<SelectFileModel> targetFileList)
        {
        }

        public List<string> GetAnotherLanguages(string path)
        {
            return new List<string>();
        }

        public void Save(Dictionary<string, SelectFileModel> sourceDictionaryEntryElements,
                         Dictionary<string, SelectFileModel> targetDictionaryEntryElements)
        {
            List<CopyResourceModel> defaultSource  = sourceDictionaryEntryElements.FirstOrDefault().Value.DictionaryEntryElements;

            // foreach (var item in sourceDictionaryEntryElements)
            // {
            //     StateChangeEvent?.Invoke(this, $"Выбран файл локализации: {item.Key}");
            //     foreach (KeyValuePair<string, SelectFileViewModel> targetDictionaryEntryElement in targetDictionaryEntryElements)
            //     {
            //         if (targetDictionaryEntryElement.Value.SourceKey == item.Key)
            //             SaveOnceFile(item.Key, item.Value, targetDictionaryEntryElement.Value, defaultSource);
            //     }
            // }
        }

        private string GetStringFormat(string key, string value)
        {
            return $"<system:String x:Uid=\"{key}\" x:Key=\"{key}\">{value}</system:String>" + Environment.NewLine;
        }

        /// <summary>
        /// Сохранение в файл ресурсов.
        /// </summary>
        private void SaveOnceFile(string itemKey,
            SelectFileModel sourceDictionaryEntryElements,
            SelectFileModel targetResourceReader,
                                  List<CopyResourceModel> defaultSourceElements)
        {
            var sb = new StringBuilder();
            var exist = new StringBuilder();
            var dublicate = new StringBuilder();
            var keys = new StringBuilder();
            sb.Append($"<!-- ============================== Не содержаться -->" + Environment.NewLine);
            exist.Append($"<!-- ============================== Уже содержаться в данном файле -->" + Environment.NewLine);
            dublicate.Append($"<!-- ============================== Измененный ключ -->" + Environment.NewLine);
            keys.Append($"<!-- ============================== Копии ключа -->" + Environment.NewLine);

            // foreach (var sourceName in sourceDictionaryEntryElements.DictionaryEntryElements.Where(x => x.IsCopy).ToList())
            // {
            //     object valueInResx = GetKeyFromValue(sourceName.Value, targetResourceReader.DictionaryEntryElements);
            //     var keyInXaml = GetValueFromKey(sourceName.Key, targetResourceReader.DictionaryEntryElements);
            //
            //     if (valueInResx != null && keyInXaml == null)
            //     {
            //         keys.Append($"{valueInResx} => {sourceName.Key}{Environment.NewLine}");
            //         dublicate.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            //         continue;
            //     }
            //
            //     if (valueInResx != null)
            //     {
            //         //keys.Append($"{valueInResx} => {sourceName.Key}{Environment.NewLine}");
            //         exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            //         StateChangeEvent?.Invoke(this, $"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
            //         continue;
            //     }
            //
            //     if (keyInXaml != null)
            //     {
            //         //keys.Append($"{keyInXaml} => {sourceName.Key}{Environment.NewLine}");
            //         exist.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            //         StateChangeEvent?.Invoke(this, $"Значение {sourceName.Value} доступно по ключу {sourceName.Key}.");
            //         continue;
            //     }
            //
            //     StateChangeEvent?.Invoke(this, $"Получено значение по ключу {sourceName.Key} со значением: {sourceName.Value}");
            //     sb.Append(GetStringFormat(sourceName.Key, sourceName.Value));
            // }
            //
            // _fileManager.SaveFile(Path.Combine(FILE_PATH, "new" + itemKey + ".xaml"), 
            //     _header, sb.ToString(), exist.ToString(), dublicate.ToString(), _bottom, keys.ToString());
        }

        #endregion Methods
    }
}