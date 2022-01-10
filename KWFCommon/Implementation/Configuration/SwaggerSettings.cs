namespace KWFCommon.Implementation.Configuration
{
    public sealed class SwaggerSettings
    {
        public string? ApiName { get; set; }
        public string BearerHeaderKey { get; set; } = "Authorization";
        public int ApiVersion { get; set; } = 1;
    }
}
