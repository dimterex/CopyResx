using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CopyToLocale.Core.Models;
using CopyToLocale.Core.Modules;

namespace CopyToLocates.Resx
{
     public class ResxFormatManager : IFormatManager
    {
        #region Constants

        private const string RESX_EXTENSION = ".resx";

        #endregion

        #region Fields

        private readonly List<string> _blackList;

        #endregion

        #region Properties

        public event EventHandler<string> StateChangeEvent;
        public string FormatType => "Resx";

        #endregion

        #region Constructors

        public ResxFormatManager()
        {
            _blackList = new List<string>()
            {
                ".Anchor",
                ".ImeMode",
                ".Location",
                ".Size",
                ".TabIndex",
                ".Name",
                ".Type",
                ".Parent",
                ".ZOrder",
                ".AutoSize",
                ".Dock",
                ".ScrollBars",
                ".AutoScaleDimensions",
                ".TrayLocation",
                ".Visible",
                ".Margin",
                ".MaxLength",
                ".Multiline",
                ".Padding",
                ".Localizable",
                ".Index",
                ".ImageStream",
                ".SelectedImageIndex",
                ".SplitterDistance",
                ".ImageIndex"
            };
        }

        #endregion

        #region Methods

        private void ParseResx(SelectFileModel selectFileViewModel, Dictionary<string, ResourceModel> result)
        {
            XmlDocument itemDoc = new XmlDocument();
            itemDoc.Load(selectFileViewModel.FullPath);

            foreach (XmlNode itemNode in itemDoc.DocumentElement.ChildNodes)
            {
                if (itemNode is not XmlElement itemElement)
                    continue;
                if (!itemElement.OuterXml.StartsWith("<data"))
                    continue;
               
                var key = itemElement.Attributes["name"]?.Value;
                    
                if (_blackList.Any(x => key.EndsWith(x)))
                    continue;

                Dictionary<string, string> testDic;
                    
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

                foreach (XmlNode childNode in itemElement.ChildNodes)
                {
                    if (childNode is not XmlElement childItem)
                        continue;
                        
                    var value = childItem.InnerText;
                    testDic.Add(selectFileViewModel.SourceKey, value);
                    break;
                }
            }
        }
            

        /// <summary>
        /// Инициализировать ресурсные файлы.
        /// </summary>
        public Dictionary<string, ResourceModel> InitReaders(IList<SelectFileModel> selectFileViewModels)
        {
            var result = new Dictionary<string, ResourceModel>();

            foreach (var selectFileViewModel in selectFileViewModels)
            {
                ParseResx(selectFileViewModel, result);
            }

            return result;
        }

        public void Update(List<ChangedResourceModel> forUpdate, IList<SelectFileModel> targetFileList)
        {
            var readList = new Dictionary<string, List<DictionaryEntry>>();
            var writers = new Dictionary<string, XmlDocument>();
            var paths = new Dictionary<string, string>();

            foreach (var fileViewModel in targetFileList)
            {
                var selectFileViewModel = targetFileList.FirstOrDefault(x => x.SourceKey.Equals(fileViewModel.SourceKey));
                var writer = new XmlDocument();
                writer.Load(selectFileViewModel.FullPath);
                
                writers.Add(fileViewModel.SourceKey, writer);
                paths.Add(fileViewModel.SourceKey, selectFileViewModel.FullPath);

                var ls = new List<DictionaryEntry>();

                foreach (XmlNode itemNode in writer.DocumentElement.ChildNodes)
                {
                    // because we know that the node is an element, we can do this:
                    if (itemNode is not XmlElement itemElement)
                        continue;
                    if (!itemElement.OuterXml.StartsWith("<data"))
                        continue;
               
                    var key = itemElement.Attributes["name"]?.Value;
                    

                    foreach (XmlNode childNode in itemElement.ChildNodes)
                    {
                        if (childNode is not XmlElement childItem)
                            continue;
                        
                        var value = childItem.InnerText;
                        ls.Add(new DictionaryEntry(key, value));
                        break;
                    }
                }
                
                readList.Add(fileViewModel.SourceKey, ls);
            }

            foreach (var keyValuePair in forUpdate)
            {
                foreach (var valuePair in keyValuePair.KeyValueList)
                {
                    SaveValue(keyValuePair.NewKey, valuePair.Value, readList[valuePair.Key]);
                }
            }

            foreach (var writer in writers)
            {
                var list = readList[writer.Key].ToList();

                foreach (XmlNode itemNode in writer.Value.DocumentElement.ChildNodes)
                {
                    // because we know that the node is an element, we can do this:
                    if (itemNode is not XmlElement itemElement)
                        continue;
                    if (!itemElement.OuterXml.StartsWith("<data"))
                        continue;

                    writer.Value.DocumentElement.RemoveChild(itemElement);
                    // var key = itemElement.Attributes["name"]?.Value;
                    //
                    // var existValue = list.FirstOrDefault(x => x.Key == key);
                    // if (existValue.Key != null && existValue.Value != null)
                    // {
                    //     foreach (XmlNode childNode in itemElement.ChildNodes)
                    //     {
                    //         if (childNode is not XmlElement childItem)
                    //             continue;
                    //
                    //         childItem.InnerText = existValue.Value.ToString();
                    //         // list.Remove(existValue);
                    //         break;
                    //     }
                    // }
                }
                
                foreach (var dictionaryEntry in list)
                {
                    //Create a new node.
                    XmlElement dataElement = writer.Value.CreateElement("data");
                    dataElement.SetAttribute("xml:space", "preserve");
                    dataElement.SetAttribute("name", dictionaryEntry.Key.ToString());
                
                    XmlElement valueElement = writer.Value.CreateElement("value");
                    valueElement.InnerText = dictionaryEntry.Value.ToString();
                
                    dataElement.AppendChild(valueElement);
                    writer.Value.DocumentElement.AppendChild(dataElement);
                }
              
                // <data name="strErrorIPAddressIncorrect" xml:space="preserve">
                // <value>Ошибка запуска сервиса, адрес {0} недоступен.</value>
                // </data>
                // TODO Добавление в  *.Designer.cs для быстрого доступа к данным ресурсам.
                writer.Value.Save(paths[writer.Key]);
            }

            writers.Clear();
            StateChangeEvent?.Invoke(this, "Все значения записаны.");
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
                StateChangeEvent?.Invoke(this, $"Добавлен ключ: {key} со значением: {value}");
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