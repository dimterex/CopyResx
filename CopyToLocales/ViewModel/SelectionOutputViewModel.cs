namespace CopyToLocales.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Input;
    using CopyToLocales.Core;
    using CopyToLocales.Model.Interfaces;
    using CopyToLocales.View;
    using CopyToLocales.ViewModel.Enums;
    using CopyToLocales.ViewModel.Interfaces;
    using Prism.Commands;
    using Prism.Mvvm;
    using Prism.Regions;

    public class SelectionOutputViewModel : BindableBase, INavigationViewModel
    {
        #region Fields

        private readonly IFormatsManager _formatsManager;

        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;
        private FormatTypes _selectedFormatType;

        #endregion

        #region Properties

        public ICommand GoBackCommand { get; }

        public ICommand GoForwardCommand { get; }

        public List<FormatTypes> Outputs { get; }

        public FormatTypes SelectedFormatType
        {
            get => _selectedFormatType;
            set => SetProperty(ref _selectedFormatType, value);
        }

        #endregion

        #region Constructors

        public SelectionOutputViewModel(IRegionManager regionManager, IFormatsManager formatsManager,
            ISettingsManager settingsManager)
        {
            _regionManager = regionManager;
            _formatsManager = formatsManager;
            _settingsManager = settingsManager;
            Outputs = _formatsManager.SupportsFormat;
            SelectedFormatType = _settingsManager.GetFormatType(DirectionType.Target);
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardCommand = new DelegateCommand(GoForward);
        }

        #endregion

        #region Methods

        private void GoBack()
        {
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionView));
        }

        private void GoForward()
        {
            _formatsManager.SelectedFormatType = SelectedFormatType;
            _settingsManager.SetFormatType(SelectedFormatType, DirectionType.Target);
            switch (SelectedFormatType)
            {
                case FormatTypes.Resx:
                case FormatTypes.Text:
                case FormatTypes.Xaml:
                    _regionManager.RequestNavigate(Constants.ContentRegion, nameof(TargetFolderSelectView));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}