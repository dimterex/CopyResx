
using CopyToLocales.Services.Interfaces;

using Prism.Commands;
using Prism.Mvvm;

using System.Collections.ObjectModel;
using System.Windows.Input;

namespace CopyToLocales.ViewModel
{
    public class MainViewModel : BindableBase
    {
        #region Fields

        private readonly ILogService _logService;
        private readonly ISettingsManager _settingsManager;

        #endregion Fields

        #region Properties

        public ObservableCollection<string> LogCollection => _logService.Logs;

        public ICommand ClosedCommand { get; }

        #endregion Properties

        #region Constuctors

        public MainViewModel(ILogService logService, ISettingsManager settingsManager)
        {
            _logService = logService;
            _settingsManager = settingsManager;
            ClosedCommand = new DelegateCommand(ClosedCommandExecuteMethod);
        }

        #endregion Constuctors

        #region Methods

        private void ClosedCommandExecuteMethod()
        {
            _settingsManager.Save();
        }

        #endregion Methods
    }
}
