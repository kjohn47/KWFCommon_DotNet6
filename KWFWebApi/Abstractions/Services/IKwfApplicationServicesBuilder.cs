namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationServicesBuilder : IKwfApplicationEndpointsBuilder
    {
        /// <summary>
        /// Add Services Configurations
        /// </summary>
        /// <param name="serviceCollectionBuilder"></param>
        /// <returns>IKwfApplicationEndpointsBuilder</returns>
        IKwfApplicationEndpointsBuilder AddServiceConfiguration(Func<IServiceCollectionBuilder, IServiceCollectionBuilderReturn> serviceCollectionBuilder);
    }
}
