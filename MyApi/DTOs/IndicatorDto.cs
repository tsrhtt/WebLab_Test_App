namespace MyApi.DTOs
{
    public class IndicatorDto
    {
        public int Id { get; set; }
        public int DirectionId { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Units { get; set; }
        public int Type { get; set; }
        public string? Comment { get; set; }
        public bool IsAdditional { get; set; }
        public bool IsNormExist { get; set; }
        public bool? IsInReference { get; set; }
        public string Group { get; set; }
        public int GroupOrderNumber { get; set; }
        public int SortNumber { get; set; }
        public double? MinStandardValue { get; set; }
        public double? MaxStandardValue { get; set; }
        public double? ResultVal { get; set; }
        public string ResultStr { get; set; }
        public List<string> TextStandards { get; set; }
        public List<string> PossibleStringValues { get; set; }
        public List<string> DynamicValues { get; set; }
    }
}
