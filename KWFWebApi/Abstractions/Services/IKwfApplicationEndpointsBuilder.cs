namespace KWFWebApi.Abstractions.Services
{
    public interface IKwfApplicationEndpointsBuilder: IKwfApplicationEndpointBuilder
    {
        /// <summary>
        /// Add Collection of classes implementing IEndpointConfiguration or EndpointConfigurationBase
        /// </summary>
        /// <param name="endpointConfiguration">The Endpoint Configuration</param>
        /// <returns>IKwfApplicationRun</returns>
        IKwfApplicationRun AddEndpointDefinitionRange(params IEndpointConfiguration[] endpointConfiguration);

        /// <summary>
        /// Add all classes implementing IEndpointConfiguration or EndpointConfigurationBase on assemblies from types
        /// </summary>
        /// <param name="typeInAssembly">Any type from the assembly with classes implementing IEndpointConfiguration or EndpointConfigurationBase to be searched</param>
        /// <returns>IKwfApplicationRun</returns>
        IKwfApplicationRun AddEndpointDefinitionFromAssemblies(params Type[] typeInAssembly);

        /// <summary>
        /// Add all classes implementing IEndpointConfiguration or EndpointConfigurationBase on assemblies from types
        /// </summary>
        ///<typeparam name="T">Type from assembly with classes implementing IEndpointConfiguration or EndpointConfigurationBase</typeparam>
        /// <returns>IKwfApplicationRun</returns>
        IKwfApplicationRun AddEndpointDefinitionFromAssembly<T>();
    }
}
