namespace KWFLogger.KWFFileLogger.Implementation
{
    using KWFLogger.KWFFileLogger.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class KwfFileLoggerExtensions
    {
        public static ILoggingBuilder AddKwfLogToFile(this ILoggingBuilder builder, string configurationKey = "KwfFileLoggerConfiguration")
        {
            builder.Services.AddSingleton<ILoggerProvider>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                return new KwfLogToFileProvider(configuration?.GetSection(configurationKey)?.Get<KwfFileLoggerConfiguration>() ?? new KwfFileLoggerConfiguration());
            });

            return builder;
        }

        public static ILoggingBuilder AddKwfLogToFile(this ILoggingBuilder builder, Action<KwfFileLoggerConfiguration> configure)
        {
            var configuration = new KwfFileLoggerConfiguration();
            configure(configuration);

            builder.Services.AddSingleton<ILoggerProvider>(sp => new KwfLogToFileProvider(configuration));

            return builder;
        }
    }
}
