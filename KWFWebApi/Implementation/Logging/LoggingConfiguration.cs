namespace KWFWebApi.Implementation.Logging
{
    public sealed class LoggingConfiguration
    {
        public bool EnableApiLogs { get; set; } = false;
        public bool EnableHttpLogs { get; set; } = false;
        public IEnumerable<string>? Providers { get; set; }
    }
}
