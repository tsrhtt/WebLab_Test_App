namespace MyApi.DTOs
{
    public class IndicatorDto
    {
        public int IndicatorId { get; set; }
        public int DirectionId { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public string ReferenceRange { get; set; }
        public string Comment { get; set; }
    }

}
