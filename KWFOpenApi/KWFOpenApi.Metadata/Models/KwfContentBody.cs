namespace KWFOpenApi.Metadata.Models
{ 
    public class KwfContentBody
    {
        public string? MediaTypeString { get; set; }
        public string? Body { get; set; }
        public string? BodyObjectName { get; set; } //Models => name of root model to get from dictionary
    }
}
