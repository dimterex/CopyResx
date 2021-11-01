using System.Collections.Generic;

namespace CopyToLocale.Core.Models
{
    public class ChangedResourceModel
    {
        /// <summary>
        /// Новый ключ для сохранения.
        /// </summary>
        public string NewKey { get; set; }
        
        /// <summary>
        /// Ключ локализационной строки.
        /// </summary>
        public string Key { get; set; }
        
        /// <summary>
        /// Список значений.
        /// В качестве ключа в данном списке используется язык локализации.
        /// </summary>
        public Dictionary<SupportedLocalization, string> KeyValueList { get; set; }
    }
}