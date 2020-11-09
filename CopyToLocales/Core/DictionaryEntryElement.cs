using System.Collections;
using Prism.Mvvm;

namespace CopyToLocales.Core
{
    public class DictionaryEntryElement : BindableBase
    {
        private bool _isCopy;
        private string _newKey;

        public bool IsCopy
        {
            get => _isCopy;
            set => SetProperty(ref _isCopy, value);
        }

        public string NewKey
        {
            get => _newKey;
            set => SetProperty(ref _newKey, value);
        }

        public string Key { get; }
        public string Value { get; }

        public DictionaryEntryElement(DictionaryEntry dictionaryEntry)
            :this (dictionaryEntry.Key.ToString(), dictionaryEntry.Value.ToString())
        {
        }

        public DictionaryEntryElement(string key, string value)
        {
            Key = key;
            Value = value;
            NewKey = Key;
        }
    }
}
