namespace CopyToLocales.ViewModel
{
    using CopyToLocales.Core;
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.View;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;

    public class SelectionViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IOutputsManager _outputsManager;
        private List<DictionaryEntryElement> _dict;

        #endregion Fields

        #region Properties

        public List<DictionaryEntryElement> DictionaryEntryElements
        {
            get => _dict;
            set => SetProperty(ref _dict, value);
        }

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardkCommand { get; }
        public ICommand SelectAllCommand { get; }

        public ICommand LoadedCommand { get; }

        #endregion Properties

        #region Constuctors

        public SelectionViewModel(IRegionManager regionManager, IOutputsManager outputsManager)
        {
            DictionaryEntryElements = outputsManager.SourceDictionaryEntryElements.FirstOrDefault().Value.DictionaryEntryElements;
            _regionManager = regionManager;
            _outputsManager = outputsManager;
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);
            SelectAllCommand = new DelegateCommand(SelectAll);
            LoadedCommand = new DelegateCommand(LoadedCommandExecute);
        }

        #endregion Constuctors

        #region Methods

        private void LoadedCommandExecute()
        {
            DictionaryEntryElements = _outputsManager.SourceDictionaryEntryElements.FirstOrDefault().Value.DictionaryEntryElements;
        }

        private void SelectAll()
        {
            DictionaryEntryElements.ForEach(x => x.IsCopy = true);
        }

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SourceFolderSelectView));
        }

        private void GoForward()
        {
            foreach (KeyValuePair<string, SelectFileViewModel> selectFileViewModel in _outputsManager.SourceDictionaryEntryElements)
            {
                foreach (DictionaryEntryElement dictionaryEntryElement in selectFileViewModel.Value.DictionaryEntryElements)
                {
                    var tmp = DictionaryEntryElements.FirstOrDefault(x => x.Key.Equals(dictionaryEntryElement.Key));
                    if (tmp != null)
                    {
                        dictionaryEntryElement.IsCopy = tmp.IsCopy;
                    }
                }
            }

            //_journal.GoForward();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionOutputView));
        }

        #endregion Methods
    }
}
