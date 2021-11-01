using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using CopyToLocale.Core;
using CopyToLocale.Core.Models;
using CopyToLocale.Core.Modules;
using CopyToLocales.Postgressql._Interfaces_;
using CopyToLocales.Postgressql.Dto;

namespace CopyToLocales.Postgressql
{
    
    public class PostgresSqlFormatManager : IFormatManager
    {
        private readonly ILocalizationDataBase _localizationDataBase;

        #region Constants

        #endregion

        #region Fields


        #endregion

        #region Properties

        public event EventHandler<string> StateChangeEvent;
        public string FormatType => "Postgres SQL";

        #endregion

        #region Constructors

        public PostgresSqlFormatManager()
        {
            _localizationDataBase = new LocalizationDataBase();
        }

        #endregion

        #region Methods

        public SupportedLocalization GetKey(string path)
        {
            return SupportedLocalization.None;
        }

        public Dictionary<string, ResourceModel> InitReaders(IList<SelectFileModel> selectFileViewModels)
        {
            var result = new Dictionary<string, ResourceModel>();
            foreach (var localizationStringModel in _localizationDataBase.GetLocalization())
            {
                Dictionary<SupportedLocalization, string> testDic;
                
                if (result.ContainsKey(localizationStringModel.Key))
                {
                    testDic = result[localizationStringModel.Key].KeyValueList;
                }
                else
                {
                    var model = new ResourceModel(localizationStringModel.Key);
                    testDic = model.KeyValueList;
                    result.Add(localizationStringModel.Key, model);
                }
                
                testDic.Add(SupportedLocalization.Arabic, localizationStringModel.Arabic);
                testDic.Add(SupportedLocalization.Czech, localizationStringModel.Czech);
                testDic.Add(SupportedLocalization.German, localizationStringModel.German);
                testDic.Add(SupportedLocalization.Greek, localizationStringModel.Greek);
                testDic.Add(SupportedLocalization.English_US, localizationStringModel.English_US);
                testDic.Add(SupportedLocalization.Spanish, localizationStringModel.Spanish);
                testDic.Add(SupportedLocalization.French, localizationStringModel.French);
                testDic.Add(SupportedLocalization.Hungarian, localizationStringModel.Hungarian);
                testDic.Add(SupportedLocalization.Italian, localizationStringModel.Italian);
                testDic.Add(SupportedLocalization.Japanese, localizationStringModel.Japanese);
                testDic.Add(SupportedLocalization.South_Korean, localizationStringModel.South_Korean);
                testDic.Add(SupportedLocalization.Dutch, localizationStringModel.Dutch);
                testDic.Add(SupportedLocalization.Polish, localizationStringModel.Polish);
                testDic.Add(SupportedLocalization.Brazilian_Portuguese, localizationStringModel.Brazilian_Portuguese);
                testDic.Add(SupportedLocalization.Russian, localizationStringModel.Russian);
                testDic.Add(SupportedLocalization.Slovak, localizationStringModel.Slovak);
                testDic.Add(SupportedLocalization.Serbian, localizationStringModel.Serbian);
                testDic.Add(SupportedLocalization.Turkish, localizationStringModel.Turkish);
                testDic.Add(SupportedLocalization.Traditional_Chinese, localizationStringModel.Traditional_Chinese);
                testDic.Add(SupportedLocalization.Simplified_Chinese, localizationStringModel.Simplified_Chinese);
                testDic.Add(SupportedLocalization.Ukrainian, localizationStringModel.Ukrainian);
            }

            return result;
        }

        public void Update(List<ChangedResourceModel> forUpdate, IList<SelectFileModel> targetFileList)
        {
            var readList = new Dictionary<SupportedLocalization, List<DictionaryEntry>>();
            var writers = new Dictionary<SupportedLocalization, XmlDocument>();
            var paths = new Dictionary<SupportedLocalization, string>();

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
            return new List<string>();
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