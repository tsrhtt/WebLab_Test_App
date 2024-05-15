using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Department
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        public List<Direction> Directions { get; set; }
    }
}
