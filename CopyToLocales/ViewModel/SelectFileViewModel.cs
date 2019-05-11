using CopyToLocales.Services.Interfaces;

using Prism.Commands;
using Prism.Mvvm;

using System;
using System.Windows.Input;

namespace CopyToLocales.ViewModel
{
    public class SelectFileViewModel : BindableBase
    {
        private readonly FileType _fileType;
        private readonly IFileManager _fileManager;
        private readonly Settings.Settings _settings;
        private string _fullPath;

        public string FullPath
        {
            get => _fullPath;
            set => SetProperty(ref _fullPath, value);
        }

        public ICommand SourceOpenButton { get; }

        public SelectFileViewModel(FileType fileType, IFileManager fileManager, Settings.Settings settings)
        {
            _fileType = fileType;
            _fileManager = fileManager;
            _settings = settings;
            SourceOpenButton = new DelegateCommand(SourceOpenButton_Click);

            switch (_fileType)
            {
                case FileType.Source:
                    FullPath = _settings.SourcePath;
                    break;
                case FileType.Target:
                    FullPath = _settings.TargetPath;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Указать папку - источник ресурсов.
        /// </summary>
        private void SourceOpenButton_Click()
        {
            var tmp = _fileManager.OpenFolderPath();
            if(tmp == null)
                return;

            FullPath = tmp.ToString();

            switch (_fileType)
            {
                case FileType.Source:
                    _settings.SourcePath = FullPath;
                    break;
                case FileType.Target:
                    _settings.TargetPath = FullPath;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
