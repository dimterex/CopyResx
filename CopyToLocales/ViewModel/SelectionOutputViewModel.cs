﻿namespace CopyToLocales.ViewModel
{
    using CopyToLocales.Services.Interfaces;
    using CopyToLocales.View;
    using CopyToLocales.ViewModel.Enums;

    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    using System;
    using System.Collections.Generic;
    using System.Windows.Input;

    using Constants = CopyToLocales.Core.Constants;

    public class SelectionOutputViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IOutputsManager _outputsManager;
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
            set => SetProperty(ref _selectedOutputType, value);
        }

        #endregion Properties

        #region Constuctors

        public SelectionOutputViewModel(IRegionManager regionManager, IOutputsManager outputsManager, ISettingsManager settingsManager)
        {
            _regionManager = regionManager;
            _outputsManager = outputsManager;
            _settingsManager = settingsManager;
            Outputs = new List<OutputTypes>(outputsManager.OutputManagers.Keys);
            SelectedOutputType = _settingsManager.Settings.SelectedOutputType;
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
            _outputsManager.SelectedOutputType = SelectedOutputType;
            _settingsManager.Settings.SelectedOutputType = SelectedOutputType;
            //_journal.GoForward();
            switch (SelectedOutputType)
            {
                case OutputTypes.Resx:
                case OutputTypes.Text:
                case OutputTypes.Xaml:
                    _regionManager.RequestNavigate(Constants.ContentRegion, nameof(TargetFolderSelectView));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Methods
    }
}
