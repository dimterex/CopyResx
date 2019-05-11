using CopyToLocales.Services.Interfaces;

using System;
using System.Collections.ObjectModel;

namespace CopyToLocales.Services.Realization
{
    public class LogService : ILogService
    {
        public ObservableCollection<string> Logs { get; }

        public LogService()
        {
            Logs = new ObservableCollection<string>();
        }

        public void AddMessage(string message)
        {
            Logs.Add($"{DateTime.Now:T} {message}");
        }
    }
}
