using System;
using System.Collections.Generic;

namespace MyApi.DTOs
{
    public class PatientDto
    {
        public int Id { get; set; }
        public string? IdentificationNumber { get; set; }
        public int PatientId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string SecondName { get; set; }
        public string FullName { get; set; }
        public int Sex { get; set; }
        public DateTime BirthDate { get; set; }
        public string SexDescription { get; set; }
        public int Age { get; set; }
    }
}
