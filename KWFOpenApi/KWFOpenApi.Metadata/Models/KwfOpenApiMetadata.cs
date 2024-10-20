namespace KWFOpenApi.Metadata.Models
{
    public class KwfOpenApiMetadata
    {
        public IEnumerable<AuthorizationType> AuthorizationTypes { get; set; } = Array.Empty<AuthorizationType>();
        public string? ApiName { get; set; } = "KwfApi";
        public string ApiDescription { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = string.Empty;
        public string? OpenApiDocumentUrl { get; set; }
        public Dictionary<string, List<KwfOpenApiRoute>>? Entrypoints { get; set; }
        public Dictionary<string, List<KwfModelProperty>>? Models { get; set; }
        public Dictionary<string, List<string>>? Enums { get; set; }
    }
}
