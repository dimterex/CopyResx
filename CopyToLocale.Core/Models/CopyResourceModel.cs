using System.Collections.Generic;

namespace CopyToLocale.Core.Models
{
    public class CopyResourceModel
    {
        public bool IsCopy { get; set; }

        public string Key { get; }

        public Dictionary<SupportedLocalization, string> Values { get; }
    }
}