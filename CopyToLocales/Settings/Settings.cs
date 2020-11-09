using CopyToLocales.ViewModel.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CopyToLocales.Settings
{
    using System.Collections.Generic;

    public class Settings
    {
        [JsonProperty(PropertyName = "SourcesPath")]
        public List<KeyValuePair<string, string>> SourcesPath { get; set; }

        [JsonProperty(PropertyName = "TargetsPath")]
        public List<KeyValuePair<string, string>> TargetPath { get; set; }
    
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "SelectedOutputType")]
        public OutputTypes SelectedOutputType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "SelectedInputType")]
        public OutputTypes SelectedInputType { get; set; }

        public Settings()
        {
            SourcesPath = new List<KeyValuePair<string, string>>();
            TargetPath = new List<KeyValuePair<string, string>>();
        }
    }
}
