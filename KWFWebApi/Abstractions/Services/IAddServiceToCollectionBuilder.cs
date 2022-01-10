namespace KWFWebApi.Abstractions.Services
{
    public interface IAddServiceToCollectionBuilder : IServiceCollectionBuilderReturn
    {
        IAddServiceToCollectionBuilder AddServiceDefinition(IServiceDefinition serviceDefinition);
    }
}
