using CopyToLocale.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CopyToLocales.DAL
{
    public class FileDto
    {
        [JsonProperty(PropertyName = "Key")]
        public string FilePath { get; set; }
        
        [JsonProperty(PropertyName = "Value")]
        [JsonConverter(typeof(StringEnumConverter))]
        public SupportedLocalization Localization { get; set; }
    }
}