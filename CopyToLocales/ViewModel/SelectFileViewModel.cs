namespace CopyToLocales.ViewModel
{
    using Prism.Commands;
    using Prism.Mvvm;

    using System.Collections.Generic;
    using System.Windows.Input;
    using CopyToLocales.Model.Interfaces;

    public class SelectFileViewModel : BindableBase
    {
        private readonly IFileManager _fileManager;
        private string _fullPath;
        private string _sourceKey;
        private bool _isDefault;

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

        public bool IsDefault
        {
            get => _isDefault;
            set => SetProperty(ref _isDefault, value);
        }

        public SelectFileViewModel(IFileManager fileManager, IFormatsManager formatsManager, string fullPath, string key)
        {
            _fileManager = fileManager;

            SourceKeys = formatsManager.GetSourceKeys();

            SourceOpenButton = new DelegateCommand(SourceOpenButton_Click);
            
            SourceKey = key;
            FullPath = fullPath;
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
