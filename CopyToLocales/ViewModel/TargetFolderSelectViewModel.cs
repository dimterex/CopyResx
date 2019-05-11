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
    public class TargetFolderSelectViewModel : BindableBase
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

        public TargetFolderSelectViewModel(IRegionManager regionManager,
            ISettingsManager settingsManager,
            IFileManager fileManager,
            IOutputsManager outputsManager,
            ILogService logService)
        {
            _regionManager = regionManager;
            _outputsManager = outputsManager;
            _logService = logService;

            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);

            SelectFileViewModel = new SelectFileViewModel(FileType.Target, fileManager, settingsManager.Settings);
       }

        #endregion Constuctors

        #region Methods

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionOutputView));
        }

        private void GoForward()
        {
            // _journal.GoForward();
            if (string.IsNullOrEmpty(SelectFileViewModel.FullPath))
            {
                _logService.AddMessage("Файл не выбран.");
                return;
            }
            _outputsManager.Read(FileType.Target, new FileInfo(SelectFileViewModel.FullPath));
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(KeysEditorView));
        }

        #endregion Methods

    }
}