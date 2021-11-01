using System.Windows;
using CopyToLocale.Core.Models;

namespace CopyToLocales.ViewModel
{
    using System.Collections.Generic;

    using CopyToLocales.Core;
    using CopyToLocales.View;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using CopyToLocales.Model;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.ViewModel.Common;
    using CopyToLocales.ViewModel.Interfaces;

    public class KeysEditorViewModel : BindableBase, INavigationViewModel
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IFormatsManager _formatsManager;
        private readonly ILogService _logService;
        private string _selectedKey;

        #endregion Fields

        #region Properties

        public ObservableCollection<ChangedResourceViewModel> DictionaryEntryElements { get; }
        
        public ObservableCollection<string> SourceKeys { get; }

        public string SelectedKey
        {
            get => _selectedKey;
            set => SetProperty(ref _selectedKey, value);
        }

        public ICommand GoBackCommand { get; }
        public ICommand StartCommand { get; }
        public ICommand GoForwardCommand { get; }
        public ICommand LoadedCommand { get; }

        #endregion Properties

        #region Constuctors

        public KeysEditorViewModel(IRegionManager regionManager, IFormatsManager formatsManager, ILogService logService)
        {
            _regionManager = regionManager;
            _formatsManager = formatsManager;
            _logService = logService;
            DictionaryEntryElements = new ObservableCollection<ChangedResourceViewModel>();
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
            LoadedCommand = new DelegateCommand(LoadedCommandExecute);
            StartCommand = new DelegateCommand(StartButton_Click);
            SourceKeys = new ObservableCollection<string>();
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

            
            
            _formatsManager.Save(DictionaryEntryElements);

            _logService.AddMessage("Выполнено");
            MessageBox.Show("Выполнено");
        }

        private void LoadedCommandExecute()
        {
            DictionaryEntryElements.Clear();
            SourceKeys.Clear();
            SourceKeys.AddRange(_formatsManager.GetSourceKeys());

            foreach (var dictionaryEntryElement in _formatsManager.SelectedSourceElements.Values)
            {
                var newKey = GetExistKey(dictionaryEntryElement);
                
                if (string.IsNullOrEmpty(newKey))
                    newKey = dictionaryEntryElement.Key;

                var dictionaryElement = new ChangedResourceViewModel(dictionaryEntryElement);
                dictionaryElement.NewKey = newKey;
                DictionaryEntryElements.Add(dictionaryElement);
            }
            
            SelectedKey = SourceKeys.FirstOrDefault();
        }

        private string GetExistKey(ResourceModel value)
        {
            foreach (var targetDictionaryEntryElement in _formatsManager.TargetDictionaryEntryElements.Values)
            {
                foreach (var targetValues in targetDictionaryEntryElement.KeyValueList)
                {
                    foreach (var sourceValue in value.KeyValueList)
                    {
                        if (sourceValue.Value.Equals(targetValues.Value))
                            return targetDictionaryEntryElement.Key;
                    }
                }
            }

            return string.Empty;
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
