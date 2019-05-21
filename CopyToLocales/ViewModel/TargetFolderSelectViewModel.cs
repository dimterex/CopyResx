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

    public class TargetFolderSelectViewModel : BindableBase
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
        public ICommand UpdateCommand { get; }
        public ICommand GoForwardkCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }

        public ObservableCollection<SelectFileViewModel> SelectFilesViewModel { get; }

        #endregion Properties

        #region Constuctors

        public TargetFolderSelectViewModel(IRegionManager regionManager,
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

            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);

            SelectFilesViewModel = new ObservableCollection<SelectFileViewModel>();

            SelectFilesViewModel = new ObservableCollection<SelectFileViewModel>();

            foreach (var settingsManagerSetting in _settingsManager.Settings.TargetPath)
            {
                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _outputsManager, settingsManagerSetting));
            }

            GoBackCommand = new DelegateCommand(GoBack);
            UpdateCommand = new DelegateCommand(Update);
            AddCommand = new DelegateCommand(Add);
            GoForwardkCommand = new DelegateCommand(GoForward);
            ClearCommand = new DelegateCommand(Clear);
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

            var fileInfo = new FileInfo(selectFileViewModel.FullPath);
            var file = fileInfo.Name.Split('.').FirstOrDefault();

            var allResx = Directory.GetFiles(fileInfo.DirectoryName).Where(x => Filter(x, file));

            foreach (string resx in allResx)
            {
                if (SelectFilesViewModel.Any(x => x.FullPath.Equals(resx)))
                    continue;

                var source = string.Empty;

                var tm = selectFileViewModel.SourceKeys.FirstOrDefault(x => resx.EndsWith(x));
                if (!string.IsNullOrEmpty(tm))
                    source = tm;

                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _outputsManager, new KeyValuePair<string, string>(resx, source)));
            }
        }

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionOutputView));
        }

        private void GoForward()
        {
            // _journal.GoForward();
            if (SelectFilesViewModel.Any(x => string.IsNullOrWhiteSpace(x.FullPath)))
            {
                _logService.AddMessage("Файл не выбран.");
                return;
            }

            _outputsManager.Clear(FileType.Target);
            _settingsManager.Settings.TargetPath.Clear();

            foreach (var selectFileViewModel in SelectFilesViewModel)
            {
                _outputsManager.Read(FileType.Target, selectFileViewModel);
                _settingsManager.Settings.TargetPath.Add(new KeyValuePair<string, string>(selectFileViewModel.FullPath, selectFileViewModel.SourceKey));
            }

            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(KeysEditorView));
        }

        #endregion Methods

    }
}