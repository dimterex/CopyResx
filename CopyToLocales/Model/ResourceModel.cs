namespace CopyToLocales.Model
{
    using System.Collections.Generic;

    /// <summary>
    /// Модель ресурса локализации.
    /// </summary>
    public class ResourceModel
    {
        #region Properties

        /// <summary>
        /// Ключ локализационной строки.
        /// </summary>
        public string Key { get; }
        
        /// <summary>
        /// Список значений.
        /// В качестве ключа в данном списке используется язык локализации.
        /// </summary>
        public Dictionary<string, string> KeyValueList { get; }
        
        #endregion Properties

        #region Constructors

        public ResourceModel(string key)
        {
            Key = key;
            KeyValueList = new Dictionary<string, string>();
        }

        #endregion Constructors
    }
}