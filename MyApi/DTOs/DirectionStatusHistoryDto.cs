using System;

namespace MyApi.DTOs
{
    public class DirectionStatusHistoryDto
    {
        public int Id { get; set; }
        public int DirectionId { get; set; }
        public DateTime DateTime { get; set; }
        public int DirectionStatusId { get; set; }
        public string UserFio { get; set; }
        public string? Comment { get; set; }
    }
}
