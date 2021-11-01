using System.Collections.Generic;

namespace CopyToLocale.Core.Models
{
    public class SelectFileModel
    {
        public string FullPath { get; set; }

        public List<SupportedLocalization> SourceKeys  { get; set; }
          
        public SupportedLocalization SourceKey  { get; set; }

        public bool IsDefault { get; set; }
        public List<CopyResourceModel> DictionaryEntryElements { get; set; }
    }
}