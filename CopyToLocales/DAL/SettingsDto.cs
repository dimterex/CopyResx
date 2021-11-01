namespace CopyToLocales.DAL
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class SettingsDto
    {
        [JsonProperty(PropertyName = "SourcesPath")]
        public List<KeyValuePair<string, string>> SourcesPath { get; set; }

        [JsonProperty(PropertyName = "TargetsPath")]
        public List<KeyValuePair<string, string>> TargetPath { get; set; }

        [JsonProperty(PropertyName = "SelectedOutputType")]
        public string SelectedOutputType { get; set; }

        [JsonProperty(PropertyName = "SelectedInputType")]
        public string SelectedInputType { get; set; }

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