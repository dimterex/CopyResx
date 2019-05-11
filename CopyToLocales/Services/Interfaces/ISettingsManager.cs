namespace CopyToLocales.Services.Interfaces
{
    public interface ISettingsManager
    {
        Settings.Settings Settings { get; }

        void Save();
    }
}
