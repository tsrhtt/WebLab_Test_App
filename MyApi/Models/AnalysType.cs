using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class AnalysType
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("format")]
        public int Format { get; set; }

        public List<Direction> Directions { get; set; }
    }
}
