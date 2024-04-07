using System;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Direction
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("cardNumber")]
        public string CardNumber { get; set; }

        [JsonPropertyName("patientFullName")]
        public string PatientFullName { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("sexDescription")]
        public string SexDescription { get; set; }
    }
}
