namespace KWFOpenApi.Metadata.Models
{ 
    public class KwfModelProperty
    {
        public required string Name { get; set; }
        public string? Reference { get; set; }
        public string? Description { get; set; }
        public required string Type { get; set; }
        public bool IsRequired { get; set; } = false;
        public bool IsEnum { get; set; } = false;
        public bool IsObject { get; set; } = false;
        public bool IsArray { get; set; } = false;
        public bool IsDate { get; set; } = false;
        public bool IsDictionary { get; set; } = false;
    }
}
