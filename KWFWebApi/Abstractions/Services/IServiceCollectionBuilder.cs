namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.Extensions.Configuration;

    public interface IServiceCollectionBuilder
    {
        IConfiguration Configuration { get; }
        bool IsDev { get; }
        IAddServiceToCollectionBuilder AddServiceDefinition(IServiceDefinition serviceDefinition);
        IServiceCollectionBuilderReturn AddServiceDefinitionRange(params IServiceDefinition[] serviceDefinitions);
    }
}
