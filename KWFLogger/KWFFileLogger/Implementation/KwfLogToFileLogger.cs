namespace KWFLogger.KWFFileLogger.Implementation
{
    using Microsoft.Extensions.Logging;

    using System;

    public class KwfLogToFileLogger : ILogger
    {
        protected readonly KwfLogToFileProvider _KwfLogToFileProvider;
        private object _fileWriterLock;
        private const string LogPathTemplate = "{0}\\Log_{1}.txt";
        private const string LogLineTemplate = "[{0}] {1} | {2} | {3} | {4} | {5}";

        public KwfLogToFileLogger(KwfLogToFileProvider kwfLogToFileProvider)
        {
            _KwfLogToFileProvider = kwfLogToFileProvider;
            _fileWriterLock = new Object();
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default!;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            lock (_fileWriterLock)
            {
                var logConfig = _KwfLogToFileProvider.Configuration.LogEventConfigurations?.FirstOrDefault(x => (x.EventId != null && x.EventId == eventId.Id) || (x.EventName != null && x.EventName == x.EventName));

                if (logConfig == null && _KwfLogToFileProvider.Configuration.LogOnlyEventsInConfiguration)
                {
                    return;
                }

                var logPath = string.Empty;
                var currDate = DateTime.UtcNow;
                var logLevelStr = logLevel.GetStringValue();

                if (logConfig != null)
                {
                    if ((logConfig.LogLevel != null && logConfig.LogLevel.Value > logLevel) 
                        || (logConfig.LogLevel == null && _KwfLogToFileProvider.Configuration.DefaultLogLevel != null && _KwfLogToFileProvider.Configuration.DefaultLogLevel > logLevel))
                    {
                        return;
                    }

                    var eventPath = logConfig.LogLevelPath 
                                    ? string.Format("{0}\\{1}\\{2}", _KwfLogToFileProvider.BasePath, logConfig.Path, logLevelStr) 
                                    : string.Format("{0}\\{1}", _KwfLogToFileProvider.BasePath, logConfig.Path);

                    if (logConfig.LogLevelPath && !Directory.Exists(eventPath))
                    {
                        Directory.CreateDirectory(eventPath);
                    }

                    logPath = string.Format(LogPathTemplate, eventPath, currDate.ToString("yyyyMMdd"));
                }
                else
                {
                    if (_KwfLogToFileProvider.Configuration.DefaultLogLevel != null && _KwfLogToFileProvider.Configuration.DefaultLogLevel > logLevel)
                    {
                        return;
                    }

                    logPath = string.Format(string.Format("{0}\\{1}", _KwfLogToFileProvider.BasePath, logLevelStr), currDate.ToString("yyyyMMdd"));
                }

                try
                {
                    //Try add new record
                    using (var streamWriter = new StreamWriter(logPath, true))
                    {
                        streamWriter.WriteLine(string.Format(LogLineTemplate, 
                            currDate.ToString("yyyy-MM-ddTHH:mm:ss:fff"), 
                            eventId.Id,
                            eventId.Name?? "Api",
                            logLevelStr,
                            formatter(state, exception), 
                            exception != null ? exception.StackTrace : ""));
                    }
                }
                catch
                {
                }
            }
        }
    }
}
