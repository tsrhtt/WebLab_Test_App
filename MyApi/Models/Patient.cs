using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Patient
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("identificationNumber")]
        public string? IdentificationNumber { get; set; }

        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("surname")]
        public string Surname { get; set; }

        [JsonPropertyName("secondName")]
        public string SecondName { get; set; }

        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("sex")]
        public int Sex { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("sexDescription")]
        public string SexDescription { get; set; }

        [JsonPropertyName("age")]
        public int Age { get; set; }

        public List<Direction> Directions { get; set; }
    }
}
