namespace KWFWebApi.Implementation.Logging
{
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public sealed class KwfLoggerProviderBuilder
    {
        private KwfLoggerProviderBuilder(string providerName, Action<ILoggingBuilder, IConfiguration> addProvider)
        {
            ProviderName = providerName;
            AddProvider = addProvider;
        }

        public string ProviderName { get; init; }
        public Action<ILoggingBuilder, IConfiguration> AddProvider { get; init; }

        public static KwfLoggerProviderBuilder AddProviderConfiguration(string name, Action<ILoggingBuilder, IConfiguration> provider)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new KwfLoggerProviderBuilder(name, provider);
        }
    }
}
