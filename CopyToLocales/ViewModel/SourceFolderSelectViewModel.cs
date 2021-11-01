using CopyToLocale.Core;
using CopyToLocales.DAL;

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
                    new SelectFileViewModel(_fileManager, _formatsManager, settingsManagerSetting.FilePath, settingsManagerSetting.Localization));
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
            SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, string.Empty, SupportedLocalization.None));
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

                SelectFilesViewModel.Add(new SelectFileViewModel(_fileManager, _formatsManager, resx, SupportedLocalization.None));
            }
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

            var ls = new List<FileDto>();
            foreach (var selectFileViewModel in SelectFilesViewModel)
            {
                selectFileViewModel.SourceKey = _formatsManager.GetKey(selectFileViewModel.FullPath, DirectionType.Source);
                ls.Add(new FileDto()
                {
                    FilePath = selectFileViewModel.FullPath,
                    Localization = selectFileViewModel.SourceKey,
                });
            }

            _settingsManager.UpdatePathList(DirectionType.Source, ls);

            _formatsManager.Load(DirectionType.Source, SelectFilesViewModel);

            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        #endregion Methods

    }
}