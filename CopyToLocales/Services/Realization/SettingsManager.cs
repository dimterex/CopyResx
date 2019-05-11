using CopyToLocales.Services.Interfaces;

namespace CopyToLocales.Services.Realization
{
    public class SettingsManager : ISettingsManager
    {
        #region Constants

        private const string FILE_NAME = "Settings.json";

        #endregion

        #region Fields

        private readonly IFileManager _fileManager;

        #endregion Fields

        #region Properties

        public Settings.Settings Settings { get; }

        #endregion Properties

        #region Constuctors

        public SettingsManager(IFileManager fileManager)
        {
            _fileManager = fileManager;
            Settings = _fileManager.GetSettings(FILE_NAME);
        }

        #endregion Constuctors

        #region Methods

        public void Save()
        {
            _fileManager.SaveSettings(FILE_NAME, Settings);
        }

        #endregion Methods

    }
}
