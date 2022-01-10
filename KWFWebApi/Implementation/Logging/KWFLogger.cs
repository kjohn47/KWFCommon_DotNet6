namespace KWFWebApi.Implementation.Logging
{
    using KWFWebApi.Abstractions.Logging;

    using Microsoft.Extensions.Logging;

    using System;

    public sealed class KWFLogger<T> : IKWFLogger<T>
    {
        private ILogger<T> _logger;

        private KWFLogger(ILogger<T> logger)
        {
            _logger = logger;
        }

        public void Log(LogLevel logLevel, EventId eventId, string message)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, message);
            }
        }

        public void Log<T0>(LogLevel logLevel, string message, EventId eventId, T0 arg0)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, message, arg0);
            }
        }

        public void Log<T0, T1>(LogLevel logLevel, string message, EventId eventId, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, message, arg0, arg1);
            }
        }

        public void Log<T0, T1, T2>(LogLevel logLevel, string message, EventId eventId, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, message, arg0, arg1, arg2);
            }
        }

        public void Log<T0, T1, T2, T3>(LogLevel logLevel, string message, EventId eventId, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, message, arg0, arg1, arg2, arg3);
            }
        }

        public void Log(LogLevel logLevel, string message, EventId eventId, params object[] args)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, message, args);
            }
        }

        public void Log(LogLevel logLevel, EventId eventId, Exception ex, string message)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, ex, message);
            }
        }

        public void Log<T0>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, ex, message, arg0);
            }
        }

        public void Log<T0, T1>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, ex, message, arg0, arg1);
            }
        }

        public void Log<T0, T1, T2>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, ex, message, arg0, arg1, arg2);
            }
        }

        public void Log<T0, T1, T2, T3>(LogLevel logLevel, string message, EventId eventId, Exception ex, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, ex, message, arg0, arg1, arg2, arg3);
            }
        }

        public void Log(LogLevel logLevel, string message, EventId eventId, Exception ex, params object[] args)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, eventId, ex, message, args);
            }
        }

        public void Log(LogLevel logLevel, Exception ex, string message)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, ex, message);
            }
        }

        public void Log<T0>(LogLevel logLevel, string message, Exception ex, T0 arg0)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, ex, message, arg0);
            }
        }

        public void Log<T0, T1>(LogLevel logLevel, string message, Exception ex, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, ex, message, arg0, arg1);
            }
        }

        public void Log<T0, T1, T2>(LogLevel logLevel, string message, Exception ex, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, ex, message, arg0, arg1, arg2);
            }
        }

        public void Log<T0, T1, T2, T3>(LogLevel logLevel, string message, Exception ex, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, ex, message, arg0, arg1, arg2, arg3);
            }
        }

        public void Log(LogLevel logLevel, string message, Exception ex, params object[] args)
        {
            if (_logger.IsEnabled(logLevel))
            {
                _logger.Log(logLevel, ex, message, args);
            }
        }

        public void LogCritical(string message)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message);
            }
        }

        public void LogCritical<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0);
            }
        }

        public void LogCritical<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0, arg1);
            }
        }

        public void LogCritical<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0, arg1, arg2);
            }
        }

        public void LogCritical<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, arg0, arg1, arg2, arg3);
            }
        }

        public void LogCritical(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Critical))
            {
                _logger.LogCritical(message, args);
            }
        }

        public void LogDebug(string message)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message);
            }
        }

        public void LogDebug<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0);
            }
        }

        public void LogDebug<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1);
            }
        }

        public void LogDebug<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1, arg2);
            }
        }

        public void LogDebug<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, arg0, arg1, arg2, arg3);
            }
        }

        public void LogDebug(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug(message, args);
            }
        }

        public void LogError(string message)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message);
            }
        }

        public void LogError<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0);
            }
        }

        public void LogError<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0, arg1);
            }
        }

        public void LogError<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0, arg1, arg2);
            }
        }

        public void LogError<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, arg0, arg1, arg2, arg3);
            }
        }

        public void LogError(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(message, args);
            }
        }

        public void LogError(string message, Exception ex)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message);
            }
        }

        public void LogError<T0>(string message, Exception ex, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message, arg0);
            }
        }

        public void LogError<T0, T1>(string message, Exception ex, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message, arg0, arg1);
            }
        }

        public void LogError<T0, T1, T2>(string message, Exception ex, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message, arg0, arg1, arg2);
            }
        }

        public void LogError<T0, T1, T2, T3>(string message, Exception ex, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message, arg0, arg1, arg2, arg3);
            }
        }

        public void LogError(string message, Exception ex, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Error))
            {
                _logger.LogError(ex, message, args);
            }
        }

        public void LogInformation(string message)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message);
            }
        }

        public void LogInformation<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0);
            }
        }

        public void LogInformation<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1);
            }
        }

        public void LogInformation<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2);
            }
        }

        public void LogInformation<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, arg0, arg1, arg2, arg3);
            }
        }

        public void LogInformation(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation(message, args);
            }
        }

        public void LogTrace(string message)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message);
            }
        }

        public void LogTrace<T0>(string message, T0 arg0)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0);
            }
        }

        public void LogTrace<T0, T1>(string message, T0 arg0, T1 arg1)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0, arg1);
            }
        }

        public void LogTrace<T0, T1, T2>(string message, T0 arg0, T1 arg1, T2 arg2)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0, arg1, arg2);
            }
        }

        public void LogTrace<T0, T1, T2, T3>(string message, T0 arg0, T1 arg1, T2 arg2, T3 arg3)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, arg0, arg1, arg2, arg3);
            }
        }

        public void LogTrace(string message, params object[] args)
        {
            if (_logger.IsEnabled(LogLevel.Trace))
            {
                _logger.LogTrace(message, args);
            }
        }

        public static IKWFLogger<T> CreateKwfLogger(ILoggerFactory loggerFactory)
        {
            return new KWFLogger<T>(loggerFactory.CreateLogger<T>());
        }
    }
}
