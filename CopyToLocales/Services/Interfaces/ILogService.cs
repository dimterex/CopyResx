using System.Collections.ObjectModel;

namespace CopyToLocales.Services.Interfaces
{
    public interface ILogService
    {
        ObservableCollection<string> Logs { get; }

        void AddMessage(string message);
    }
}
