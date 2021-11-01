using CopyToLocale.Core;
using CopyToLocale.Core.Models;

namespace CopyToLocales.ViewModel.Common
{
    using System.Collections.Generic;
    using CopyToLocales.Model;
    using Prism.Mvvm;

    /// <summary>
    /// ViewModel ресурса для редактирования нового ключа.
    /// </summary>
    public class ChangedResourceViewModel : BindableBase
    {
        #region Fields

        private string _newKey;

        #endregion Fields

        #region Properties
        
        /// <summary>
        /// Новый ключ для сохранения.
        /// </summary>
        public string NewKey
        {
            get => _newKey;
            set => SetProperty(ref _newKey, value);
        }
        
        /// <summary>
        /// Ключ локализационной строки.
        /// </summary>
        public string Key { get; }
        
        /// <summary>
        /// Список значений.
        /// В качестве ключа в данном списке используется язык локализации.
        /// </summary>
        public Dictionary<SupportedLocalization, string> KeyValueList { get; }

        #endregion Properties

        #region Constructors

        public ChangedResourceViewModel(ResourceModel resourceModel)
        {
            KeyValueList = resourceModel.KeyValueList;
            Key = resourceModel.Key;
        }

        #endregion Constructors

        /// <summary>
        /// Убрать значения, одинаковые со значением по умолчанию.
        /// </summary>
        /// <param name="defaultSourceKey">Язык локализации значения по умолчанию.</param>
        public void Clear(SupportedLocalization defaultSourceKey)
        {
            if (defaultSourceKey == SupportedLocalization.None)
                return;
            if (!KeyValueList.ContainsKey(defaultSourceKey))
                return;
            
            var defaultValue = KeyValueList[defaultSourceKey];

            var toRemove = new List<SupportedLocalization>();
                
            // Переносить для языка по умолчанию в любом случае.
            // Если значение в текущем языке совпадет со значеним для языка по умолчанию,
            // значит данная фраза не локализована и нет необходимости ее переносить.
            foreach (var keyValuePair in KeyValueList)
            {
                if (keyValuePair.Key.Equals(defaultValue))
                    continue;
                    
                if (keyValuePair.Value.Equals(defaultValue))
                    toRemove.Add(keyValuePair.Key);
            }

            foreach (var removed in toRemove)
                KeyValueList.Remove(removed);
        }
    }
}