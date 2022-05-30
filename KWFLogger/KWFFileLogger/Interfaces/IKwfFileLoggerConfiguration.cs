namespace KWFLogger.KWFFileLogger.Interfaces
{
    using KWFLogger.KWFFileLogger.Models;

    using Microsoft.Extensions.Logging;

    public interface IKwfFileLoggerConfiguration
    {
        string Path { get; }
        LogLevel? DefaultLogLevel { get; }
        IEnumerable<KwfLogEventConfiguration>? LogEventConfigurations { get; }
        bool LogOnlyEventsInConfiguration { get; }
    }
}
