namespace KWFWebApi.Implementation.Services
{
    using KWFWebApi.Abstractions.Services;
    using KWFWebApi.Implementation.Logging;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public sealed class KwfApplicationBuilder : IKwfApplicationBuilder
    {
        private readonly WebApplicationBuilder _applicationBuilder;
        private readonly string? _customAppConfigurationKey;
        private readonly string? _customBearerConfigurationKey;
        private readonly string? _customLoggingConfigurationKey;
        private readonly bool _enableAuthentication;
        private Func<(IConfiguration configuration, bool isDev), IServiceDefinition[]>? _serviceFactories;
        private ICollection<IEndpointConfiguration>? _endpointConfigurations;
        private ICollection<KwfLoggerProviderBuilder>? _loggerProviders;
        private ICollection<Type>? _middlewares;

        private KwfApplicationBuilder(
            WebApplicationBuilder applicationBuilder, 
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            bool enableAuthentication)
        {
            _applicationBuilder = applicationBuilder;
            _customAppConfigurationKey = customAppConfigurationKey;
            _customBearerConfigurationKey = customBearerConfigurationKey;
            _customLoggingConfigurationKey = customLoggingConfigurationKey;
            _enableAuthentication = enableAuthentication;
        }

        public IKwfApplicationBuilder AddLoggerProvider(
            string providerName,
            Action<ILoggingBuilder> providerConfigure)
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

        public IKwfApplicationMiddlewareBuilder AddMiddleware<T>()
            where T : KwfMiddlewareBase
        {
            if (_middlewares is null)
            {
                _middlewares = new List<Type>();
            }

            _middlewares.Add(typeof(T));

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

        public IKwfApplicationRun AddEndpointDefinitionFromAssemblies(params Type[] typeInAssembly)
        {
            if (typeInAssembly is null)
            {
                throw new ArgumentNullException(nameof(typeInAssembly));
            }

            var assemblies = typeInAssembly.Select(x => x.Assembly);

            _endpointConfigurations = GetEndpointConfigurationsFromAssemblies(assemblies.ToArray());

            return this;
        }

        public IKwfApplicationRun AddEndpointDefinitionFromAssembly<T>()
        {
            var assembly = typeof(T).Assembly;
            
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            
            _endpointConfigurations = GetEndpointConfigurationsFromAssemblies(assembly);

            return this;
        }

        public void Run()
        {
            _applicationBuilder.RunKwfApplication(
                _customAppConfigurationKey,
                _customBearerConfigurationKey,
                _customLoggingConfigurationKey,
                _enableAuthentication,
                _loggerProviders,
                _middlewares,
                _serviceFactories,
                _endpointConfigurations?.ToArray());
        }

        public static IKwfApplicationBuilder BuildKwfApplication(
            WebApplicationBuilder applicationBuilder,
            string? customAppConfigurationKey,
            string? customBearerConfigurationKey,
            string? customLoggingConfigurationKey,
            bool enableAuthentication = true)
        {
            return new KwfApplicationBuilder(
                applicationBuilder,
                customAppConfigurationKey,
                customBearerConfigurationKey,
                customLoggingConfigurationKey,
                enableAuthentication);
        }

        private static ICollection<IEndpointConfiguration>? GetEndpointConfigurationsFromAssemblies(params Assembly[] assemblies)
        {
            var endpointConfigurations = new List<IEndpointConfiguration>();

            foreach (var assembly in assemblies)
            {
                var endpointInstallerTypes = assembly.DefinedTypes.Where(a =>
                                            typeof(IEndpointConfiguration).IsAssignableFrom(a) &&
                                            !a.IsInterface &&
                                            !a.IsAbstract);

                var endpointInstallers = endpointInstallerTypes.Select(Activator.CreateInstance).Cast<IEndpointConfiguration>();

                if (endpointInstallers is not null)
                    endpointConfigurations.AddRange(endpointInstallers);
            }

            return endpointConfigurations.Any() ? endpointConfigurations : null;
        }
    }
}
