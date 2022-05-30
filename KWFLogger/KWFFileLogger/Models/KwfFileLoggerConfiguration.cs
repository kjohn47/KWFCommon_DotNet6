namespace KWFLogger.KWFFileLogger.Models
{
    using System.Collections.Generic;

    using KWFLogger.KWFFileLogger.Interfaces;

    using Microsoft.Extensions.Logging;

    public class KwfFileLoggerConfiguration : IKwfFileLoggerConfiguration
    {
        public string Path { get; set; } = "KwfLogs";

        public LogLevel? DefaultLogLevel { get; set; }

        public IEnumerable<KwfLogEventConfiguration>? LogEventConfigurations { get; set; }

        public bool LogOnlyEventsInConfiguration { get; set; }
    }
}
