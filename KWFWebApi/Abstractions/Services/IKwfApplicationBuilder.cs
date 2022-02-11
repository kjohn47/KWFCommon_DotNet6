namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public interface IKwfApplicationBuilder : IKwfApplicationMiddlewareBuilder
    {
        /// <summary>
        /// Add Custom logger provider
        /// </summary>
        /// <param name="providerName">Logger provider name used to enable/disable provider on settings</param>
        /// <param name="providerConfigure">The Provider configuration</param>
        /// <returns>IKwfApplicationBuilder</returns>
        IKwfApplicationBuilder AddLoggerProvider(string providerName, Action<ILoggingBuilder, IConfiguration> providerConfigure);
    }
}
