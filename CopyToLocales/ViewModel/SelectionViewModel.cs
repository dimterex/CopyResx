namespace CopyToLocales.ViewModel
{
    using CopyToLocales.Core;
    using CopyToLocales.View;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using CopyToLocales.Model;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel.Common;
    using CopyToLocales.ViewModel.Interfaces;

    public class SelectionViewModel : BindableBase, INavigationViewModel
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IFormatsManager _formatsManager;
        private List<CopyResourceViewModel> _dict;
        private string _selectedKey;

        #endregion Fields

        #region Properties

        public List<CopyResourceViewModel> DictionaryEntryElements
        {
            get => _dict;
            set => SetProperty(ref _dict, value);
        }
        
        public ObservableCollection<string> SourceKeys { get; }

        public string SelectedKey
        {
            get => _selectedKey;
            set => SetProperty(ref _selectedKey, value);
        }

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardCommand { get; }
        public ICommand SelectAllCommand { get; }

        public ICommand LoadedCommand { get; }

        #endregion Properties

        #region Constuctors

        public SelectionViewModel(IRegionManager regionManager, IFormatsManager formatsManager)
        {
            DictionaryEntryElements = new List<CopyResourceViewModel>();
            _regionManager = regionManager;
            _formatsManager = formatsManager;
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
            SelectAllCommand = new DelegateCommand(SelectAll);
            LoadedCommand = new DelegateCommand(LoadedCommandExecute);
            SourceKeys = new ObservableCollection<string>();
        }

        #endregion Constuctors

        #region Methods

        private void LoadedCommandExecute()
        {
            SourceKeys.Clear();
            SourceKeys.AddRange(_formatsManager.GetSourceKeys());
            
            var ls = new List<CopyResourceViewModel>();
            foreach (var entryElement in _formatsManager.SourceDictionaryEntryElements)
            {
                var item = new CopyResourceViewModel(entryElement.Value.Key, entryElement.Value.KeyValueList);
                item.IsCopy = _formatsManager.SelectedSourceElements.ContainsKey(entryElement.Value.Key);
                ls.Add(item);
            }
            DictionaryEntryElements = ls;
            _formatsManager.SelectedSourceElements.Clear();
            
            SelectedKey = SourceKeys.FirstOrDefault();
        }

        private void SelectAll()
        {
            DictionaryEntryElements.ForEach(x => x.IsCopy = true);
        }

        private void GoBack()
        {
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SourceFolderSelectView));
        }

        private void GoForward()
        {
            foreach (var dictionaryEntryElement in DictionaryEntryElements.Where(x => x.IsCopy))
            {
                var tt = new ResourceModel(dictionaryEntryElement.Key);
                foreach (var value in dictionaryEntryElement.Values)
                    tt.KeyValueList.Add(value.Key, value.Value);
                
                _formatsManager.SelectedSourceElements.Add(dictionaryEntryElement.Key, tt);
            }

            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionOutputView));
        }

        #endregion Methods
    }
}
