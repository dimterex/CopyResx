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

    public class TargetFolderSelectViewModel : BindableBase
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
            IFormatsManager formatsManager,
            ILogService logService)
        {
            _regionManager = regionManager;
            _settingsManager = settingsManager;
            _fileManager = fileManager;
            _formatsManager = formatsManager;
            _logService = logService;

            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);

            SelectFilesViewModel = new ObservableCollection<SelectFileViewModel>();

            foreach (var settingsManagerSetting in _settingsManager.GetPathList(DirectionType.Target))
            {
                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, settingsManagerSetting.Key, settingsManagerSetting.Value));
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
            SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, string.Empty, string.Empty));
        }

        private void Update()
        {
            var selectFileViewModel = SelectFilesViewModel.FirstOrDefault();
            if (selectFileViewModel == null)
                return;

            List<string> filterResult = _formatsManager.GetAnotherLanguages(selectFileViewModel.FullPath, DirectionType.Target);

            foreach (string resx in filterResult)
            {
                if (SelectFilesViewModel.Any(x => x.FullPath.Equals(resx)))
                    continue;

                var source = string.Empty;

                var tm = selectFileViewModel.SourceKeys.FirstOrDefault(x => resx.EndsWith(x));
                if (!string.IsNullOrEmpty(tm))
                    source = tm;

                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, resx, source));
            }
        }

        private void GoBack()
        {
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionOutputView));
        }

        private void GoForward()
        {
            if (SelectFilesViewModel.Any(x => string.IsNullOrWhiteSpace(x.FullPath)))
            {
                _logService.AddMessage("���� �� ������.");
                return;
            }

            var ls = new List<KeyValuePair<string, string>>();
            foreach (var selectFileViewModel in SelectFilesViewModel)
            {
                ls.Add(new KeyValuePair<string, string>(selectFileViewModel.FullPath, selectFileViewModel.SourceKey));
                
                if (selectFileViewModel.IsDefault)
                    _formatsManager.SetDefaultKey(selectFileViewModel.SourceKey);
            }

            _settingsManager.UpdatePathList(DirectionType.Target, ls);
            _formatsManager.Load(DirectionType.Target, SelectFilesViewModel);
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(KeysEditorView));
        }

        #endregion Methods

    }
}