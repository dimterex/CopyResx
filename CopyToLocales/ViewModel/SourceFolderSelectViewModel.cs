namespace CopyToLocales.ViewModel
{
    using CopyToLocales.Core;
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.View;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;

    public class SourceFolderSelectViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IOutputsManager _outputsManager;
        private readonly ILogService _logService;
        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IFileManager _fileManager;

        #endregion Fields

        #region Properties

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardkCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }

        public ObservableCollection<SelectFileViewModel> SelectFilesViewModel { get; }

        #endregion Properties

        #region Constuctors

        public SourceFolderSelectViewModel(IRegionManager regionManager,
            ISettingsManager settingsManager,
            IFileManager fileManager,
            IOutputsManager outputsManager,
            ILogService logService)
        {
            _regionManager = regionManager;
            _settingsManager = settingsManager;
            _fileManager = fileManager;
            _outputsManager = outputsManager;
            _logService = logService;

            SelectFilesViewModel = new ObservableCollection<SelectFileViewModel>();

            foreach (var settingsManagerSetting in _settingsManager.Settings.SourcesPath)
            {
                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _outputsManager, settingsManagerSetting));
            }

            GoBackCommand = new DelegateCommand(GoBack);
            UpdateCommand = new DelegateCommand(Update);
            ClearCommand = new DelegateCommand(Clear);
            AddCommand = new DelegateCommand(Add);
            GoForwardkCommand = new DelegateCommand(GoForward);
        }

        #endregion Constuctors

        #region Methods

        private void Clear()
        {
            SelectFilesViewModel.Clear();
        }

        private void Add()
        {
            SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _outputsManager, new KeyValuePair<string, string>(string.Empty, string.Empty)));
        }

        private void Update()
        {
            const string RESX_EXTENSION = ".resx";

            bool Filter(string filePath, string name)
            {
                if (string.IsNullOrEmpty(name))
                    return true;

                var fi = new FileInfo(filePath);

                var fileName = fi.Name.Split('.').FirstOrDefault();

                if (name.Equals(fileName) && fi.Extension.Equals(RESX_EXTENSION))
                    return true;

                return false;
            }

            var selectFileViewModel = SelectFilesViewModel.FirstOrDefault();
            if (selectFileViewModel == null)
                return;

            var fileInfo = new FileInfo(selectFileViewModel.FullPath);
            var file = fileInfo.Name.Split('.').FirstOrDefault();

            var allResx = Directory.GetFiles(fileInfo.DirectoryName).Where(x => Filter(x, file));


            foreach (string resx in allResx)
            {
                if (SelectFilesViewModel.Any(x => x.FullPath.Equals(resx)))
                    continue;

                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _outputsManager, new KeyValuePair<string, string>(resx, string.Empty)));
            }

        }

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionInputView));
        }

        private void GoForward()
        {
            // _journal.GoForward();
            if (SelectFilesViewModel.Any(x => string.IsNullOrWhiteSpace(x.FullPath)))
            {
                _logService.AddMessage("Файл не выбран.");
                return;
            }

            _outputsManager.Clear(FileType.Source);
            _settingsManager.Settings.SourcesPath.Clear();

            foreach (var selectFileViewModel in SelectFilesViewModel)
            {
                _settingsManager.Settings.SourcesPath.Add(new KeyValuePair<string, string>(selectFileViewModel.FullPath, selectFileViewModel.SourceKey));
                _outputsManager.Read( FileType.Source, selectFileViewModel);
            }

            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        #endregion Methods

    }
}