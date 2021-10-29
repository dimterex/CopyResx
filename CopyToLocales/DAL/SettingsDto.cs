namespace CopyToLocales.DAL
{
    using System.Collections.Generic;
    using CopyToLocales.ViewModel.Enums;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public class SettingsDto
    {
        [JsonProperty(PropertyName = "SourcesPath")]
        public List<KeyValuePair<string, string>> SourcesPath { get; set; }

        [JsonProperty(PropertyName = "TargetsPath")]
        public List<KeyValuePair<string, string>> TargetPath { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "SelectedOutputType")]
        public FormatTypes SelectedOutputType { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "SelectedInputType")]
        public FormatTypes SelectedInputType { get; set; }

        [JsonProperty(PropertyName = "BlackList")]
        public List<string> BlackList { get; set; }

        public SettingsDto()
        {
            SourcesPath = new List<KeyValuePair<string, string>>();
            TargetPath = new List<KeyValuePair<string, string>>();
            BlackList = new List<string>();
        }
    }
}