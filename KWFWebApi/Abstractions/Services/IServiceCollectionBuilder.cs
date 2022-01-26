namespace KWFWebApi.Abstractions.Services
{
    using Microsoft.Extensions.Configuration;

    public interface IServiceCollectionBuilder
    {
        /// <summary>
        /// The Configuration
        /// </summary>
        IConfiguration Configuration { get; }

        /// <summary>
        /// Is Development Environment
        /// </summary>
        bool IsDev { get; }

        /// <summary>
        /// Add Instance of IServiceDefinition or ServiceDefinitionBase
        /// </summary>
        /// <param name="serviceDefinition">The service defenition</param>
        /// <returns>IAddServiceToCollectionBuilder</returns>
        IAddServiceToCollectionBuilder AddServiceDefinition(IServiceDefinition serviceDefinition);

        /// <summary>
        /// Add Collection of IServiceDefinition or ServiceDefinitionBase
        /// </summary>
        /// <param name="serviceDefinitions">The service defenitions</param>
        /// <returns>IServiceCollectionBuilderReturn</returns>
        IServiceCollectionBuilderReturn AddServiceDefinitionRange(params IServiceDefinition[] serviceDefinitions);

        /// <summary>
        /// Add all classes implementing IServiceDefinition on assemblies from types
        /// </summary>
        /// <param name="typeInAssembly">Any type from the assembly containing classes implementing IServiceDefinition to be searched</param>
        /// <returns>IServiceCollectionBuilderReturn</returns>
        IServiceCollectionBuilderReturn AddServiceDefinitionFromAssemblies(params Type[] typeInAssembly);

        /// <summary>
        /// Add all classes implementing IServiceDefinition or ServiceDefinitionBase abstract class on assemblies from types
        /// </summary>
        ///<typeparam name="T">Type from assembly containing classes implementing IServiceDefinition or ServiceDefinitionBase abstract class</typeparam>
        /// <returns>IServiceCollectionBuilderReturn</returns>
        IServiceCollectionBuilderReturn AddServiceDefinitionFromAssembly<T>();
    }
}
