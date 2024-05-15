using System;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class DirectionStatusHistory
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("directionId")]
    public int DirectionId { get; set; }
    public Direction Direction { get; set; }

    [JsonPropertyName("dateTime")]
    public DateTime DateTime { get; set; }

    [JsonPropertyName("directionStatusId")]
    public int DirectionStatusId { get; set; }

    [JsonPropertyName("userFio")]
    public string? UserFio { get; set; }

    [JsonPropertyName("comment")]
    public string? Comment { get; set; }
}

}
