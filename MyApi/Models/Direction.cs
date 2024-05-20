using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Direction
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        public int PatientId { get; set; }

        [JsonPropertyName("patient")]
        public Patient Patient { get; set; }

        [JsonPropertyName("laboratoryId")]
        public int LaboratoryId { get; set; }
        public LaboratoryData LaboratoryData { get; set; }

        [JsonPropertyName("analysTypeId")]
        public int AnalysTypeId { get; set; }
        public AnalysType AnalysType { get; set; }

        [JsonPropertyName("departmentId")]
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        [JsonPropertyName("patientFullName")]
        public string PatientFullName { get; set; }

        [JsonPropertyName("cito")]
        public bool Cito { get; set; }

        [JsonPropertyName("isArchived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("laboratory")]
        public string Laboratory { get; set; }

        [JsonPropertyName("analysTypeName")]
        public string AnalysTypeName { get; set; }

        [JsonPropertyName("analysTypeFormat")]
        public int AnalysTypeFormat { get; set; }

        [JsonPropertyName("directionStatusId")]
        public int DirectionStatusId { get; set; }

        [JsonPropertyName("directionStatus")]
        public string DirectionStatus { get; set; }

        [JsonPropertyName("departmentName")]
        public string? DepartmentName { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("requestedBy")]
        public string RequestedBy { get; set; }

        [JsonPropertyName("acceptedDate")]
        public DateTime? AcceptedDate { get; set; }

        [JsonPropertyName("acceptedBy")]
        public string? AcceptedBy { get; set; }

        [JsonPropertyName("onDate")]
        public DateTime? OnDate { get; set; }

        [JsonPropertyName("readyDate")]
        public DateTime? ReadyDate { get; set; }

        [JsonPropertyName("sid")]
        public string? Sid { get; set; }

        [JsonPropertyName("hasAnyResults")]
        public bool HasAnyResults { get; set; }

        [JsonPropertyName("laborantComment")]
        public string? LaborantComment { get; set; }

        [JsonPropertyName("samplingDate")]
        public DateTime? SamplingDate { get; set; }

        [JsonPropertyName("samplingDateStr")]
        public string SamplingDateStr { get; set; }

        [JsonPropertyName("sampleNumber")]
        public string? SampleNumber { get; set; }

        [JsonPropertyName("samplingDoctorFio")]
        public string? SamplingDoctorFio { get; set; }

        [JsonPropertyName("doctorLabDiagnosticFio")]
        public string? DoctorLabDiagnosticFio { get; set; }

        [JsonPropertyName("doctorFeldsherLaborantFio")]
        public string? DoctorFeldsherLaborantFio { get; set; }

        [JsonPropertyName("doctorBiologFio")]
        public string? DoctorBiologFio { get; set; }

        [JsonPropertyName("bioMaterialCount")]
        public int? BioMaterialCount { get; set; }

        [JsonPropertyName("bioMaterialType")]
        public string? BioMaterialType { get; set; }

        [JsonPropertyName("numberByJournal")]
        public int? NumberByJournal { get; set; }

        [JsonPropertyName("directionStatusHistory")]
        public List<DirectionStatusHistory>? DirectionStatusHistory { get; set; } = new List<DirectionStatusHistory>();

        [JsonPropertyName("indicators")]
        public List<IndicatorGroup> IndicatorGroups { get; set; } = new List<IndicatorGroup>();

        public List<Indicator> Indicators { get; set; } = new List<Indicator>();
    }
}
