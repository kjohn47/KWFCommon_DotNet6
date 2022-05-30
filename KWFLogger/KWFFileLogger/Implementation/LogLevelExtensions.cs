namespace KWFLogger.KWFFileLogger.Implementation
{
    using Microsoft.Extensions.Logging;

    internal static class LogLevelExtensions
    {
        public static string? GetStringValue(this LogLevel? logLevel)
        {
            if (logLevel == null)
            {
                return null;
            }

            return GetStringValue(logLevel.Value);
        }

        public static string? GetStringValue(this LogLevel logLevel)
        {
            return logLevel switch
            {
                LogLevel.None => nameof(LogLevel.None),
                LogLevel.Trace => nameof(LogLevel.Trace),
                LogLevel.Debug => nameof(LogLevel.Debug),
                LogLevel.Information => nameof(LogLevel.Information),
                LogLevel.Warning => nameof(LogLevel.Warning),
                LogLevel.Error => nameof(LogLevel.Error),
                LogLevel.Critical => nameof(LogLevel.Critical),
                _ => null,
            };
        }
    }
}
