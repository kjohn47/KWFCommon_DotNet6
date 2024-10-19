namespace KWFCommon.Implementation.Configuration
{
    public sealed class OpenApiSettings
    {
        public string? ApiName { get; set; }
        public string BearerHeaderKey { get; set; } = "Authorization";
        public int ApiVersion { get; set; } = 1;
        public bool UseUI { get; set; } = false;
        public bool UseDocumentation { get; set; } = false;
    }
}
