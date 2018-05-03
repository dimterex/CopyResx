namespace CopyToLocales
{
    using System.Configuration;
    using System.Linq;

    public class ControlSettings
    {
        private readonly Configuration _configuration;
        private readonly AppSettingsSection _appSettingsSection;

        public static ControlSettings Instance { get; } = new ControlSettings();

        public ControlSettings()
        {
            _configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _appSettingsSection = _configuration.AppSettings;
        }

        public string LoadSetting(string property)
        {
            return _appSettingsSection.Settings.AllKeys.ToList().Contains(property)
                ? _appSettingsSection.Settings[property].Value
                : string.Empty;
        }

        public void SaveFolderForHistory(string property, string path)
        {
            if (_appSettingsSection.Settings.AllKeys.ToList().Contains(property))
                _appSettingsSection.Settings[property].Value = path;
            else
                _appSettingsSection.Settings.Add(property, path);
            _configuration.Save();
        }
    }
}
