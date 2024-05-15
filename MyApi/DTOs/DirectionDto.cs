using System;
using System.Collections.Generic;

namespace MyApi.DTOs
{
    public class DirectionDto
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public PatientDto Patient { get; set; }
        public int LaboratoryId { get; set; }
        public string Laboratory { get; set; }
        public int AnalysTypeId { get; set; }
        public string AnalysTypeName { get; set; }
        public int AnalysTypeFormat { get; set; }
        public int? DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public List<DirectionStatusHistoryDto> DirectionStatusHistory { get; set; } // Renamed property in DTO
        public List<IndicatorDto> Indicators { get; set; }
        public string PatientFullName { get; set; }
        public bool Cito { get; set; }
        public bool IsArchived { get; set; }
        public string DirectionStatus { get; set; }
        public int DirectionStatusId { get; set; }
        public string Category { get; set; }
        public DateTime RequestDate { get; set; }
        public string RequestedBy { get; set; }
        public DateTime? AcceptedDate { get; set; }
        public string AcceptedBy { get; set; }
        public DateTime? OnDate { get; set; }
        public DateTime? ReadyDate { get; set; }
        public string Sid { get; set; }
        public bool HasAnyResults { get; set; }
        public string LaborantComment { get; set; }
        public DateTime? SamplingDate { get; set; }
        public string SamplingDateStr { get; set; }
        public string SampleNumber { get; set; }
        public string SamplingDoctorFio { get; set; }
        public string DoctorLabDiagnosticFio { get; set; }
        public string DoctorFeldsherLaborantFio { get; set; }
        public string DoctorBiologFio { get; set; }
        public int? BioMaterialCount { get; set; }
        public string BioMaterialType { get; set; }
        public int? NumberByJournal { get; set; }
    }
}
