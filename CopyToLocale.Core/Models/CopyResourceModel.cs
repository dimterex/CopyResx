using System.Collections.Generic;

namespace CopyToLocale.Core.Models
{
    public class CopyResourceModel
    {
        public bool IsCopy { get; set; }

        public string Key { get; }

        public Dictionary<string, string> Values { get; }
    }
}