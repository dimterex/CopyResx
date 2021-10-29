namespace CopyToLocales.Model.Interfaces
{
    using System.Collections.ObjectModel;

    public interface ILogService
    {
        ObservableCollection<string> Logs { get; }

        void AddMessage(string message);
    }
}
