namespace KWFWebApi.Implementation.Services
{
    using KWFWebApi.Abstractions.Services;

    using Microsoft.Extensions.Configuration;

    using System.Diagnostics.CodeAnalysis;

    internal sealed class ServiceCollectionBuilder : IServiceCollectionBuilder, IAddServiceToCollectionBuilder, IServiceCollectionBuilderReturn
    {
        public ServiceCollectionBuilder(IConfiguration configuration, bool isDev)
        {
            Configuration = configuration;
            IsDev = isDev;
        }

        public IConfiguration Configuration { get; init; }
        public bool IsDev { get; init; }

        public ICollection<IServiceDefinition>? Services { get; private set; }

        public IAddServiceToCollectionBuilder AddServiceDefinition(IServiceDefinition serviceDefinition)
        {
            if (Services is null)
            {
                Services = new List<IServiceDefinition>();
            }

            Services.Add(serviceDefinition);
            return this;
        }

        public IServiceCollectionBuilderReturn AddServiceDefinitionRange(params IServiceDefinition[] serviceDefinitions)
        {
            if (serviceDefinitions is null)
            {
                throw new ArgumentNullException(nameof(serviceDefinitions));
            }

            Services = serviceDefinitions;
            
            return this;
        }
    }
}
