using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Indicator
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public int DirectionId { get; set; }
        public Direction Direction { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("abbreviation")]
        public string Abbreviation { get; set; }

        [JsonPropertyName("units")]
        public string Units { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("comment")]
        public string? Comment { get; set; }

        [JsonPropertyName("isAdditional")]
        public bool IsAdditional { get; set; }

        [JsonPropertyName("isNormExist")]
        public bool IsNormExist { get; set; }

        [JsonPropertyName("isInReference")]
        public bool? IsInReference { get; set; }

        [JsonPropertyName("group")]
        public string Group { get; set; }

        [JsonPropertyName("groupOrderNumber")]
        public int GroupOrderNumber { get; set; }

        [JsonPropertyName("sortNumber")]
        public int SortNumber { get; set; }

        [JsonPropertyName("minStandardValue")]
        public double? MinStandardValue { get; set; }

        [JsonPropertyName("maxStandardValue")]
        public double? MaxStandardValue { get; set; }

        [JsonPropertyName("resultVal")]
        public double? ResultVal { get; set; }

        [JsonPropertyName("resultStr")]
        public string? ResultStr { get; set; }

        [JsonPropertyName("textStandards")]
        public List<string>? TextStandards { get; set; }

        [JsonPropertyName("possibleStringValues")]
        public List<string>? PossibleStringValues { get; set; }

        [JsonPropertyName("dynamicValues")]
        public List<string>? DynamicValues { get; set; }
    }
}
