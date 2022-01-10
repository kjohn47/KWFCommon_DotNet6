namespace KWFWebApi.Abstractions.Logging
{
    using Microsoft.Extensions.Logging;

    public interface IKWFLogger<T>
    {
        void Log(LogLevel logLevel, EventId eventId, string message);
        void Log<T0>(LogLevel logLevel, string message, EventId eventId, T0 arg0);
        void Log<T0, T1>(LogLevel logLevel, string message, EventId eventId, T0 arg0, T1 arg1);
        void Log<T0, T1, T2>(LogLevel logLevel, string message, EventId eventId, T0 arg0, T1 arg1, T2 arg2);
        void Log<T0, T1, T2, T3>(LogLevel logLevel, string message, EventId eventId, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void Log(LogLevel logLevel, string message, EventId eventId, params object[] args);
        void Log(LogLevel logLevel, EventId eventId, Exception ex, string message);
        void Log<T0>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0);
        void Log<T0, T1>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0, T1 arg1);
        void Log<T0, T1, T2>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0, T1 arg1, T2 arg2);
        void Log<T0, T1, T2, T3>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void Log(LogLevel logLevel, string message, EventId eventId, Exception ex, params object[] args);
        void Log(LogLevel logLevel, Exception ex, string message);
        void Log<T0>(LogLevel logLevel, string message, Exception ex, T0 arg0);
        void Log<T0, T1>(LogLevel logLevel, string message, Exception ex, T0 arg0, T1 arg1);
        void Log<T0, T1, T2>(LogLevel logLevel, string message, Exception ex, T0 arg0, T1 arg1, T2 arg2);
        void Log<T0, T1, T2, T3>(LogLevel logLevel, string message, Exception ex, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void Log(LogLevel logLevel, string message, Exception ex, params object[] args);

        void LogInformation(string message);
        void LogInformation<T0>(string message, T0 arg0);
        void LogInformation<T0, T1>(string message, T0 arg0, T1 arg1);
        void LogInformation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
        void LogInformation<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void LogInformation(string message, params object[] args);

        void LogDebug(string message);
        void LogDebug<T0>(string message, T0 arg0);
        void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1);
        void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
        void LogDebug<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void LogDebug(string message, params object[] args);

        void LogError(string message);
        void LogError<T0>(string message, T0 arg0);
        void LogError<T0, T1>(string message, T0 arg0, T1 arg1);
        void LogError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
        void LogError<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void LogError(string message, params object[] args);
        void LogError(string message, Exception ex);
        void LogError<T0>(string message, Exception ex, T0 arg0);
        void LogError<T0, T1>(string message, Exception ex, T0 arg0, T1 arg1);
        void LogError<T0, T1, T2>(string message, Exception ex, T0 arg0, T1 arg1, T2 arg2);
        void LogError<T0, T1, T2, T3>(string message, Exception ex, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void LogError(string message, Exception ex, params object[] args);

        void LogCritical(string message);
        void LogCritical<T0>(string message, T0 arg0);
        void LogCritical<T0, T1>(string message, T0 arg0, T1 arg1);
        void LogCritical<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
        void LogCritical<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void LogCritical(string message, params object[] args);

        void LogTrace(string message);
        void LogTrace<T0>(string message, T0 arg0);
        void LogTrace<T0, T1>(string message, T0 arg0, T1 arg1);
        void LogTrace<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2);
        void LogTrace<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3);
        void LogTrace(string message, params object[] args);
    }
}
