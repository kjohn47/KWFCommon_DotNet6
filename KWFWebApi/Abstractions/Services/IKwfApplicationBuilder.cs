namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public interface IKwfApplicationBuilder : IKwfApplicationServicesBuilder
    {
        IKwfApplicationBuilder AddLoggerProvider(string providerName, Action<ILoggingBuilder, IConfiguration> providerConfigure);
    }
}
