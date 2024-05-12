using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyApi.Models
{
    public class Laboratory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public List<Direction> Directions { get; set; }
    }

    public class AnalysisType
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("format")]
        public int Format { get; set; }

        public List<Direction> Directions { get; set; }
    }

    public class Department
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        public List<Direction> Directions { get; set; }
    }

    public class Patient
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("identificationNumber")]
        public string IdentificationNumber { get; set; }

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

    public class Direction
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("patientId")]
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        [JsonPropertyName("laboratoryId")]
        public int LaboratoryId { get; set; }
        public Laboratory Laboratory { get; set; }

        [JsonPropertyName("analysisTypeId")]
        public int AnalysisTypeId { get; set; }
        public AnalysisType AnalysisType { get; set; }

        [JsonPropertyName("departmentId")]
        public int? DepartmentId { get; set; }
        public Department Department { get; set; }

        public List<StatusHistory> StatusHistories { get; set; }
        public List<Indicator> Indicators { get; set; }

        [JsonPropertyName("patientFullName")]
        public string PatientFullName { get; set; }

        [JsonPropertyName("cito")]
        public bool Cito { get; set; }

        [JsonPropertyName("isArchived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("laboratoryName")]
        public string LaboratoryName { get; set; }

        [JsonPropertyName("analysisTypeName")]
        public string AnalysisTypeName { get; set; }

        [JsonPropertyName("analysisTypeFormat")]
        public int AnalysisTypeFormat { get; set; }

        [JsonPropertyName("directionStatusId")]
        public int DirectionStatusId { get; set; }

        [JsonPropertyName("directionStatus")]
        public string DirectionStatus { get; set; }

        [JsonPropertyName("departmentName")]
        public string DepartmentName { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("requestDate")]
        public DateTime RequestDate { get; set; }

        [JsonPropertyName("requestedBy")]
        public string RequestedBy { get; set; }

        [JsonPropertyName("acceptedDate")]
        public DateTime? AcceptedDate { get; set; }

        [JsonPropertyName("acceptedBy")]
        public string AcceptedBy { get; set; }

        [JsonPropertyName("onDate")]
        public DateTime? OnDate { get; set; }

        [JsonPropertyName("readyDate")]
        public DateTime? ReadyDate { get; set; }

        [JsonPropertyName("sid")]
        public string Sid { get; set; }

        [JsonPropertyName("hasAnyResults")]
        public bool HasAnyResults { get; set; }

        [JsonPropertyName("laborantComment")]
        public string LaborantComment { get; set; }

        [JsonPropertyName("samplingDate")]
        public DateTime? SamplingDate { get; set; }

        [JsonPropertyName("samplingDateStr")]
        public string SamplingDateStr { get; set; }

        [JsonPropertyName("sampleNumber")]
        public string SampleNumber { get; set; }

        [JsonPropertyName("samplingDoctorFio")]
        public string SamplingDoctorFio { get; set; }

        [JsonPropertyName("doctorLabDiagnosticFio")]
        public string DoctorLabDiagnosticFio { get; set; }

        [JsonPropertyName("doctorFeldsherLaborantFio")]
        public string DoctorFeldsherLaborantFio { get; set; }

        [JsonPropertyName("doctorBiologFio")]
        public string DoctorBiologFio { get; set; }

        [JsonPropertyName("bioMaterialCount")]
        public int? BioMaterialCount { get; set; }

        [JsonPropertyName("bioMaterialType")]
        public string BioMaterialType { get; set; }

        [JsonPropertyName("numberByJournal")]
        public int? NumberByJournal { get; set; }
    }

    public class StatusHistory
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("directionId")]
        public int DirectionId { get; set; }
        public Direction Direction { get; set; }

        [JsonPropertyName("dateTime")]
        public DateTime DateTime { get; set; }

        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }

        [JsonPropertyName("userFio")]
        public string UserFio { get; set; }

        [JsonPropertyName("comment")]
        public string Comment { get; set; }
    }

    public class Indicator
    {
        [JsonPropertyName("directionId")]
        public int DirectionId { get; set; }

        [JsonPropertyName("indicatorId")]
        public int IndicatorId { get; set; }

        public Direction Direction { get; set; }
    }

    public class User
    {
        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("token")]
        public string Token { get; set; }
    }
}
