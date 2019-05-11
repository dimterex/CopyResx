using System.IO;
using CopyToLocales.Core;
using CopyToLocales.Services.Interfaces;
using CopyToLocales.View;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System.Windows.Input;

namespace CopyToLocales.ViewModel
{
    public class SourceFolderSelectViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IOutputsManager _outputsManager;
        private readonly ILogService _logService;
        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Properties

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardkCommand { get; }

        public SelectFileViewModel SelectFileViewModel { get; }

        #endregion Properties

        #region Constuctors

        public SourceFolderSelectViewModel(IRegionManager regionManager,
            ISettingsManager settingsManager,
            IFileManager fileManager,
            IOutputsManager outputsManager,
            ILogService logService)
        {
            _regionManager = regionManager;
            _outputsManager = outputsManager;
            _logService = logService;

            SelectFileViewModel = new SelectFileViewModel(FileType.Source, fileManager, settingsManager.Settings);

            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);
        }

        #endregion Constuctors

        #region Methods

        private void GoBack()
        {
            //_journal.GoBack();
            //_regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        private void GoForward()
        {
            // _journal.GoForward();
            if (string.IsNullOrEmpty(SelectFileViewModel.FullPath))
            {
                _logService.AddMessage("Файл не выбран.");
                return;
            }

            _outputsManager.Read(FileType.Source, new FileInfo(SelectFileViewModel.FullPath));

            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        #endregion Methods

    }
}