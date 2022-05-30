namespace KWFLogger.KWFFileLogger.Models
{
    using KWFLogger.KWFFileLogger.Implementation;

    using Microsoft.Extensions.Logging;

    public class KwfLogEventConfiguration
    {
        private string? _path;

        public int? EventId { get; set; }
        public string? EventName { get; set; }
        public LogLevel? LogLevel { get; set; }
        public bool LogLevelPath { get; set; }
        public string? Path
        {
            get
            {
                return _path is not null
                    ? _path
                    : EventName is not null
                      ? EventName
                      : LogLevel.GetStringValue();
            }
            set
            {
                _path = value;
            }
        }
    }
}
