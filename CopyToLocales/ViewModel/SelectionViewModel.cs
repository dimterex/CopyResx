using CopyToLocales.Core;
using CopyToLocales.Services.Interfaces;
using CopyToLocales.View;

using Prism.Commands;
using Prism.Regions;

using System.Collections.Generic;
using System.Windows.Input;

namespace CopyToLocales.ViewModel
{
    public class SelectionViewModel : INavigation
    {
        #region Fields

        private readonly IRegionManager _regionManager;

        #endregion Fields

        #region Properties

        public List<DictionaryEntryElement> DictionaryEntryElements { get; }

        public ICommand GoBackCommand { get; }
        public ICommand GoForwardkCommand { get; }

        #endregion Properties

        #region Constuctors

        public SelectionViewModel(IRegionManager regionManager, IOutputsManager outputsManager)
        {
            DictionaryEntryElements = outputsManager.SourceDictionaryEntryElements;
            _regionManager = regionManager;
            GoBackCommand = new DelegateCommand(GoBack);
            GoForwardkCommand = new DelegateCommand(GoForward);
        }

        #endregion Constuctors

        #region Methods

        private void GoBack()
        {
            //_journal.GoBack();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SourceFolderSelectView));
        }

        private void GoForward()
        {
            //_journal.GoForward();
            _regionManager.RequestNavigate(Constants.ContentRegion, nameof(SelectionOutputView));
        }

        #endregion Methods
    }
}
