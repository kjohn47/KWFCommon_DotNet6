namespace KWFCommon.Implementation.Configuration
{
    public sealed class AppConfiguration
    {
        public SwaggerSettings? SwaggerSettings { get; set; }
        public KestrelConfiguration? KestrelConfiguration { get; set; }
        public LocalizationConfiguration? LocalizationConfiguration { get; set; }
        public CorsConfiguration? CorsConfiguration { get; set; }
    }
}
