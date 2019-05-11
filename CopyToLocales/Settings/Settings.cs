using CopyToLocales.ViewModel.Enums;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CopyToLocales.Settings
{
    public class Settings
    {
        [JsonProperty(PropertyName = "SourcePath")]
        public string SourcePath { get; set; }

        [JsonProperty(PropertyName = "TargetPath")]
        public string TargetPath { get; set; }
    
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "SelectedOutputType")]
        public OutputTypes SelectedOutputType { get; set; }

        public Settings()
        {
        }

    }
}
