

namespace CopyToLocales.ViewModel
{
    using CopyToLocales.Services.Interfaces;

    using Core;

    using Prism.Commands;
    using Prism.Mvvm;

    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    public class SelectFileViewModel : BindableBase
    {
        private readonly IFileManager _fileManager;
        private string _fullPath;
        private string _sourceKey;

        public string FullPath
        {
            get => _fullPath;
            set => SetProperty(ref _fullPath, value);
        }

        public List<string> SourceKeys { get; }
          
        public string SourceKey
        {
            get => _sourceKey;
            set => SetProperty(ref _sourceKey, value);
        }

        public ICommand SourceOpenButton { get; }

        public List<DictionaryEntryElement> DictionaryEntryElements { get; }

        public SelectFileViewModel(IFileManager fileManager, IOutputsManager outputsManager, KeyValuePair<string, string> path)
        {
            _fileManager = fileManager;

            SourceKeys = outputsManager.SourceDictionaryEntryElements.Keys.ToList();

            DictionaryEntryElements = new List<DictionaryEntryElement>();
            SourceOpenButton = new DelegateCommand(SourceOpenButton_Click);
            SourceKey = path.Value;
            FullPath = path.Key;
        }

        /// <summary>
        /// Указать папку - источник ресурсов.
        /// </summary>
        private void SourceOpenButton_Click()
        {
            var tmp = _fileManager.OpenFolderPath();
            if (tmp == null)
                return;

            FullPath = tmp.ToString();
        }
    }
}
