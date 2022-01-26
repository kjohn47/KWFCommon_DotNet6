namespace KWFWebApi.Abstractions.Services
{
    public interface IAddServiceToCollectionBuilder : IServiceCollectionBuilderReturn
    {
        /// <summary>
        /// Add instance of class implementing IServiceDefinition
        /// </summary>
        /// <param name="serviceDefinition">The ServiceDefinition</param>
        /// <returns>IAddServiceToCollectionBuilder</returns>
        IAddServiceToCollectionBuilder AddServiceDefinition(IServiceDefinition serviceDefinition);
    }
}
