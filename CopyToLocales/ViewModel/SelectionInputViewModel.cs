namespace CopyToLocales.ViewModel
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

    public class SelectionInputViewModel : BindableBase, INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;
        private readonly IOutputsManager _outputsManager;
        private readonly ISettingsManager _settingsManager;
        private OutputTypes _selectedOutputType;
        private string _textParser;

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

        public string TextParser
        {
            get => _textParser;
            set => SetProperty(ref _textParser, value);
        }

        #endregion Properties

        #region Constuctors

        public SelectionInputViewModel(IRegionManager regionManager, IOutputsManager outputsManager, ISettingsManager settingsManager)
        {
            _regionManager = regionManager;
            _outputsManager = outputsManager;
            _settingsManager = settingsManager;
            Outputs = new List<OutputTypes>(outputsManager.OutputManagers.Keys);
            SelectedOutputType = _settingsManager.Settings.SelectedInputType;
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
            //_journal.GoForward();
            _outputsManager.SelectedInputType = SelectedOutputType;
            _settingsManager.Settings.SelectedInputType = SelectedOutputType;
            switch (SelectedOutputType)
            {
                case OutputTypes.Text:
                case OutputTypes.Xaml:
                case OutputTypes.Resx:
                    _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SourceFolderSelectView));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Methods
    }
}
