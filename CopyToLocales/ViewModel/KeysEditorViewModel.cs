using CopyToLocales.Core;
using CopyToLocales.Services.Interfaces;
using CopyToLocales.View;

using Prism.Commands;
using Prism.Regions;

using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace CopyToLocales.ViewModel
{
    public class KeysEditorViewModel : INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IOutputsManager _outputsManager;
        private readonly ILogService _logService;

        #endregion Fields

        #region Properties

        public ObservableCollection<DictionaryEntryElement> DictionaryEntryElements { get; }
        public ICommand GoBackCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand GoForwardkCommand { get; }
        public ICommand LoadedCommand { get; }

        #endregion Properties

        #region Constuctors

        public KeysEditorViewModel(IRegionManager regionManager, IOutputsManager outputsManager, ILogService logService)
        {
            _regionManager = regionManager;
            _outputsManager = outputsManager;
            _logService = logService;
            DictionaryEntryElements = new ObservableCollection<DictionaryEntryElement>();
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);
            LoadedCommand = new DelegateCommand(LoadedCommandExecute);
            StartCommand = new DelegateCommand(StartButton_Click);
        }

        #endregion Constuctors

        #region Methods

        /// <summary>
        /// Начать копирование значений.
        /// </summary>
        private void StartButton_Click()
        {
            if (DictionaryEntryElements.Any(x => string.IsNullOrWhiteSpace(x.NewKey)))
            {
                _logService.AddMessage($"Один из элементов пустой");
                return;
            }

            _logService.AddMessage($"Запущен процесс копирования.");

         
            _outputsManager.Save();

            _logService.AddMessage("Выполнено");
            System.Windows.Forms.MessageBox.Show("Выполнено");
        }

        private void LoadedCommandExecute()
        {
            DictionaryEntryElements.Clear();
            DictionaryEntryElements.AddRange(_outputsManager.SourceDictionaryEntryElements.Where(x => x.IsCopy));
        }

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(TargetFolderSelectView));
        }

        private void GoForward()
        {
            //_journal.GoForward();
            //_regionManager.RequestNavigate("ContentRegion", nameof(KeysEditorView));
        }

        #endregion Methods

    }
}
