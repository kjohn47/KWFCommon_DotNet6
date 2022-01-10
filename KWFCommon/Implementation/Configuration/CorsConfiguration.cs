namespace KWFCommon.Implementation.Configuration
{
    public sealed class CorsConfiguration
    {
        public bool AllowAnyOrigin { get; set; }
        public bool AllowAnyHeader { get; set; }
        public bool AllowAnyMethod { get; set; }
        public string[]? AllowedOrigins { get; set; }
        public string[]? AllowedHeaders { get; set; }
        public string[]? AllowedMethods { get; set; }
    }
}
