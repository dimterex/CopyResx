namespace CopyToLocales.ViewModel
{
    using CopyToLocales.Core;
    using CopyToLocales.View;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Windows.Input;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel.Interfaces;

    public class SourceFolderSelectViewModel : BindableBase, INavigationViewModel
    {
        #region Fields

        private readonly IFormatsManager _formatsManager;
        private readonly ILogService _logService;
        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IFileManager _fileManager;

        #endregion Fields

        #region Properties

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardCommand { get; }
        public ICommand UpdateCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand ClearCommand { get; }

        public ObservableCollection<SelectFileViewModel> SelectFilesViewModel { get; }

        #endregion Properties

        #region Constuctors

        public SourceFolderSelectViewModel(IRegionManager regionManager,
            ISettingsManager settingsManager,
            IFileManager fileManager,
            IFormatsManager formatsManager,
            ILogService logService)
        {
            _regionManager = regionManager;
            _settingsManager = settingsManager;
            _fileManager = fileManager;
            _formatsManager = formatsManager;
            _logService = logService;

            SelectFilesViewModel = new ObservableCollection<SelectFileViewModel>();

            foreach (var settingsManagerSetting in _settingsManager.GetPathList(DirectionType.Source))
            {
                SelectFilesViewModel.Add(
                    new SelectFileViewModel(_fileManager, _formatsManager, settingsManagerSetting.Key, settingsManagerSetting.Value));
            }

            GoBackCommand = new DelegateCommand(GoBack);
            UpdateCommand = new DelegateCommand(Update);
            ClearCommand = new DelegateCommand(Clear);
            AddCommand = new DelegateCommand(Add);
            GoForwardCommand = new DelegateCommand(GoForward);
        }

        #endregion Constuctors

        #region Methods

        private void Clear()
        {
            SelectFilesViewModel.Clear();
        }

        private void Add()
        {
            SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, string.Empty, string.Empty));
        }

        private void Update()
        {
            var selectFileViewModel = SelectFilesViewModel.FirstOrDefault();
            if (selectFileViewModel == null)
                return;
            
            List<string> filterResult = _formatsManager.GetAnotherLanguages(selectFileViewModel.FullPath, DirectionType.Source);

            foreach (string resx in filterResult)
            {
                if (SelectFilesViewModel.Any(x => x.FullPath.Equals(resx)))
                    continue;

                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, resx, string.Empty));
            }
        }

        private string GetKey(string path)
        {
            var fileInfo = new FileInfo(path);
            var rsx = fileInfo.Name.Split('.');
            string name = string.Empty;
            if (rsx.Length == 2)
                name = $".ru.{rsx[1]}"; //Используется для русского языка.
            else if (rsx.Length == 3)
                name = $".{rsx[1]}.{rsx[2]}";
            return name;
        }

        private void GoBack()
        {
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionInputView));
        }

        private void GoForward()
        {
            if (SelectFilesViewModel.Any(x => string.IsNullOrWhiteSpace(x.FullPath)))
            {
                _logService.AddMessage("Файл не выбран.");
                return;
            }

            var ls = new List<KeyValuePair<string, string>>();
            foreach (var selectFileViewModel in SelectFilesViewModel)
            {
                selectFileViewModel.SourceKey = GetKey(selectFileViewModel.FullPath);
                ls.Add(new KeyValuePair<string, string>(selectFileViewModel.FullPath, selectFileViewModel.SourceKey));
            }

            _settingsManager.UpdatePathList(DirectionType.Source, ls);

            _formatsManager.Load(DirectionType.Source, SelectFilesViewModel);

            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        #endregion Methods

    }
}