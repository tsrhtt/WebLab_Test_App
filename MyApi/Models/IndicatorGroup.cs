using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class IndicatorGroup
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("value")]
        public List<Indicator> Value { get; set; }
    }
}
