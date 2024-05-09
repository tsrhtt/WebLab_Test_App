using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Direction
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("patientFullName")]
        public string PatientFullName { get; set; }

        [JsonPropertyName("laboratory")]
        public string Laboratory { get; set; }

        [JsonPropertyName("analysTypeName")]
        public string AnalysTypeName { get; set; }

        [JsonPropertyName("directionStatus")]
        public string DirectionStatus { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("acceptedDate")]
        public DateTime? AcceptedDate { get; set; } // Made nullable in case date isn't always provided

        // TODO: OPTIONAL: To include nested patient info if needed later, i just added it here
        [JsonPropertyName("patient")]
        public PatientInfo Patient { get; set; }
    }

    public class PatientInfo
    {
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonPropertyName("sexDescription")]
        public string SexDescription { get; set; }
    }
}
