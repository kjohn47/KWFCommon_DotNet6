namespace KWFWebApi.Implementation.Services
{
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Implementation.Logging;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Collections.Generic;

    public sealed class KwfApplicationBuilder : IKwfApplicationBuilder
    {
        private readonly WebApplicationBuilder _applicationBuilder;
        private Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]>? _serviceFactories;
        private ICollection<IEndpointConfiguration>? _endpointConfigurations;
        private ICollection<KwfLoggerProviderBuilder>? _loggerProviders;

        private KwfApplicationBuilder(WebApplicationBuilder applicationBuilder)
        {
            _applicationBuilder = applicationBuilder;
        }

        public IKwfApplicationBuilder AddLoggerProvider(string providerName, Action<ILoggingBuilder, IConfiguration> providerConfigure)
        {
            if (_loggerProviders is null)
            {
                _loggerProviders = new List<KwfLoggerProviderBuilder>();
            }

            if (_loggerProviders.Any(x => x.ProviderName.Equals(providerName)))
            {
                throw new ArgumentException("Cannot add logger provider with same configuration name", nameof(providerName));
            }

            _loggerProviders.Add(KwfLoggerProviderBuilder.AddProviderConfiguration(providerName, providerConfigure));

            return this;
        }

        public IKwfApplicationEndpointsBuilder AddServiceConfiguration(Func<IServiceCollectionBuilder, IServiceCollectionBuilderReturn> serviceCollectionBuilder)
        {
            _serviceFactories = builder =>
            {
                var serviceCollectionFactory = new ServiceCollectionBuilder(builder.configuration, builder.isDev);
                serviceCollectionBuilder(serviceCollectionFactory);
                return serviceCollectionFactory.Services?.ToArray()?? Array.Empty<IServiceDefinition>();
            };

            return this;
        }

        public IKwfApplicationEndpointBuilder AddEndpointDefinition(IEndpointConfiguration endpointConfiguration)
        {
            if (_endpointConfigurations is null)
            {
                _endpointConfigurations = new List<IEndpointConfiguration>();
            }

            _endpointConfigurations.Add(endpointConfiguration);

            return this;
        }

        public IKwfApplicationRun AddEndpointDefinitionRange(params IEndpointConfiguration[] endpointConfiguration)
        {
            if (endpointConfiguration is null)
            {
                throw new ArgumentNullException(nameof(endpointConfiguration));
            }

            _endpointConfigurations = endpointConfiguration;

            return this;
        }

        public void Run()
        {
            _applicationBuilder.RunKwfApplication(_loggerProviders, _serviceFactories, _endpointConfigurations?.ToArray());
        }

        public static IKwfApplicationBuilder BuildKwfApplication(WebApplicationBuilder applicationBuilder)
        {
            return new KwfApplicationBuilder(applicationBuilder);
        }
    }
}
