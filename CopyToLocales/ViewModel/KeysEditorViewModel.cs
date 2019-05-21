namespace CopyToLocales.ViewModel
{
    using System.Collections.Generic;

    using CopyToLocales.Core;
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.View;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;

    public class KeysEditorViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IOutputsManager _outputsManager;
        private readonly ILogService _logService;
        private string _selectedKey;
        private ObservableCollection<DictionaryEntryElement> _dic;

        #endregion Fields

        #region Properties

        public ObservableCollection<DictionaryEntryElement> DictionaryEntryElements
        {
            get => _dic;
            set => SetProperty(ref _dic, value);
        }

        public string SelectedKey
        {
            get => _selectedKey;
            set => SetProperty(ref _selectedKey, value);
        }

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

            foreach (KeyValuePair<string, SelectFileViewModel> outputsManagerSourceDictionaryEntryElement in _outputsManager.SourceDictionaryEntryElements)
            {
                foreach (var entryElement in outputsManagerSourceDictionaryEntryElement.Value.DictionaryEntryElements)
                {
                    var dictionaryEntryElement = DictionaryEntryElements.FirstOrDefault(x => x.Key.Equals(entryElement.Key));

                    if (dictionaryEntryElement != null)
                        entryElement.NewKey = dictionaryEntryElement.NewKey;
                }
            }

            _outputsManager.Save();

            _logService.AddMessage("Выполнено");
            System.Windows.Forms.MessageBox.Show("Выполнено");
        }

        private void LoadedCommandExecute()
        {
            DictionaryEntryElements.Clear();
            SelectedKey = _outputsManager.SourceDictionaryEntryElements.FirstOrDefault().Key;
            DictionaryEntryElements.AddRange(_outputsManager.SourceDictionaryEntryElements[SelectedKey].DictionaryEntryElements.Where(x => x.IsCopy));
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
