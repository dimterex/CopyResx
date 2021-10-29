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

    public class SelectionInputViewModel : BindableBase, INavigationViewModel
    {
        #region Fields

        private readonly IFormatsManager _formatsManager;

        private readonly IRegionManager _regionManager;
        private readonly ISettingsManager _settingsManager;
        private FormatTypes _selectedFormatType;
        private string _textParser;

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

        /// <summary>
        /// TODO. Add Regex to view to text file.
        /// </summary>
        public string TextParser
        {
            get => _textParser;
            set => SetProperty(ref _textParser, value);
        }

        #endregion

        #region Constructors

        public SelectionInputViewModel(IRegionManager regionManager, IFormatsManager formatsManager,
            ISettingsManager settingsManager)
        {
            _regionManager = regionManager;
            _formatsManager = formatsManager;
            _settingsManager = settingsManager;
            Outputs = _formatsManager.SupportsFormat;
            SelectedFormatType = _settingsManager.GetFormatType(DirectionType.Source);
            GoBackCommand = new DelegateCommand(() => { });
            GoForwardCommand = new DelegateCommand(GoForward);
        }

        #endregion

        #region Methods

        private void GoForward()
        {
            _formatsManager.SelectedInputType = SelectedFormatType;
            _settingsManager.SetFormatType(SelectedFormatType, DirectionType.Source);
            switch (SelectedFormatType)
            {
                case FormatTypes.Text:
                case FormatTypes.Xaml:
                case FormatTypes.Resx:
                    _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SourceFolderSelectView));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion
    }
}