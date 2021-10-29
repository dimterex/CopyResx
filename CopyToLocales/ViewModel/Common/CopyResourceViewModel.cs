namespace CopyToLocales.ViewModel.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using Prism.Mvvm;

    /// <summary>
    /// ViewModel ресурса локализации для определния необходимости копирования.
    /// </summary>
    public class CopyResourceViewModel : BindableBase
    {
        private bool _isCopy;

        public bool IsCopy
        {
            get => _isCopy;
            set => SetProperty(ref _isCopy, value);
        }

        public string Key { get; }

        public Dictionary<string, string> Values { get; }

        public CopyResourceViewModel(string key, Dictionary<string, string> values)
        {
            Key = key;
            Values = values;
        }
    }
}
