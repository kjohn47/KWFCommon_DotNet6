namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationServicesBuilder : IKwfApplicationEndpointsBuilder
    {
        IKwfApplicationEndpointsBuilder AddServiceConfiguration(Func<IServiceCollectionBuilder, IServiceCollectionBuilderReturn> serviceCollectionBuilder);
    }
}
