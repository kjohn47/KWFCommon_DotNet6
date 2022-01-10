namespace KWFWebApi.Extensions
{
    using KWFWebApi.Abstractions.Logging;
    using KWFWebApi.Implementation.Logging;

    using Microsoft.Extensions.Logging;

    public static class KWFLoggerExtensions
    {
        public static IKWFLogger<T> CreateKwfLogger<T>(this ILoggerFactory loggerFactory)
        {
            return KWFLogger<T>.CreateKwfLogger(loggerFactory);
        }
    }
}
