using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Indicator
    {
        [JsonPropertyName("indicatorId")]
        public int IndicatorId { get; set; }

        [JsonPropertyName("directionId")]
        public int DirectionId { get; set; }
        public Direction Direction { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("value")]
        public string? Value { get; set; }

        [JsonPropertyName("unit")]
        public string? Unit { get; set; }

        [JsonPropertyName("referenceRange")]
        public string? ReferenceRange { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }
    }
}