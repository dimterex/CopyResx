using System;
using CopyToLocales.Services.Interfaces;
using CopyToLocales.View;
using CopyToLocales.ViewModel.Enums;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using System.Collections.Generic;
using System.Windows.Input;

using Constants = CopyToLocales.Core.Constants;

namespace CopyToLocales.ViewModel
{
    public class SelectionOutputViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;
        private OutputTypes _selectedOutputType;

        #endregion Fields

        #region Properties

        public ICommand GoBackCommand { get; }

        public ICommand GoForwardkCommand { get; }

        public List<OutputTypes> Outputs { get; }

        public OutputTypes SelectedOutputType
        {
            get => _selectedOutputType;
            set => SetProperty(ref _selectedOutputType, value, 
                () => _settingsManager.Settings.SelectedOutputType = value);
        }

        #endregion Properties

        #region Constuctors

        public SelectionOutputViewModel(IRegionManager regionManager, IOutputsManager outputsManager, ISettingsManager settingsManager)
        {
            _regionManager = regionManager;
            _settingsManager = settingsManager;
            Outputs = new List<OutputTypes>(outputsManager.OutputManagers.Keys);
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);
        }

        #endregion Constuctors

        #region Methods

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        private void GoForward()
        {
            //_journal.GoForward();
            switch (SelectedOutputType)
            {
                case OutputTypes.Resx:
                    _regionManager.RequestNavigate(Constants.ContentRegion, nameof(TargetFolderSelectView));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Methods
    }
}
