namespace KWFLogger.KWFFileLogger.Implementation
{    
    using System;

    using KWFLogger.KWFFileLogger.Interfaces;

    using Microsoft.Extensions.Logging;

    [ProviderAlias("KWFLogToFileProvider")]
    public class KwfLogToFileProvider : ILoggerProvider
    {
        public IKwfFileLoggerConfiguration Configuration { get; init; }
        public string BasePath { get; init; }

        public KwfLogToFileProvider(IKwfFileLoggerConfiguration? configuration)
        {
            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration), "Missing Kwf log provider configuration");
            }

            Configuration = configuration;

            //initialize folders
            BasePath = Path.IsPathRooted(Configuration.Path) ? Configuration.Path : string.Format("{0}\\{1}", Environment.CurrentDirectory, Configuration.Path);

            if (!Directory.Exists(BasePath))
            {
                Directory.CreateDirectory(BasePath);
            }

            if (!Configuration.LogOnlyEventsInConfiguration)
            {
                var logLevelArray = Enum.GetValues(typeof(LogLevel));
                foreach (LogLevel? logLevel in logLevelArray)
                {
                    if (logLevel is null || (Configuration.DefaultLogLevel != null && logLevel.Value < Configuration.DefaultLogLevel.Value) || logLevel.Value == LogLevel.None)
                    {
                        continue;
                    }

                    var logLevelPath = string.Format("{0}\\{1}", BasePath, logLevel.GetStringValue());
                    if (!Directory.Exists(logLevelPath))
                    {
                        Directory.CreateDirectory(logLevelPath);
                    }
                }
            }

            if (Configuration.LogEventConfigurations != null)
            {
                foreach(var logEventConfiguration in Configuration.LogEventConfigurations)
                {
                    if (logEventConfiguration.EventId is null && string.IsNullOrEmpty(logEventConfiguration.EventName))
                    {
                        throw new ArgumentNullException("EventId", "EnentId or EventName must be defined");
                    }

                    var logLevelPath = string.Format("{0}\\{1}", BasePath, logEventConfiguration.Path);
                    if (!Directory.Exists(logLevelPath))
                    {
                        Directory.CreateDirectory(logLevelPath);
                    }
                }
            }
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new KwfLogToFileLogger(this);
        }

        public void Dispose()
        {
        }
    }
}
