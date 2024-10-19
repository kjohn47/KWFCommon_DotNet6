namespace KWFCommon.Implementation.Configuration
{
    public sealed class AppConfiguration
    {
        public OpenApiSettings? OpenApiSettings { get; set; }
        public KestrelConfiguration? KestrelConfiguration { get; set; }
        public LocalizationConfiguration? LocalizationConfiguration { get; set; }
        public CorsConfiguration? CorsConfiguration { get; set; }
    }
}
