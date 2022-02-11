namespace KWFWebApi.Abstractions.Services
{ 
    using Microsoft.Extensions.DependencyInjection;

    public interface IServiceDefinition
    {
        /// <summary>
        /// Add Services
        /// </summary>
        /// <param name="services">The Service Collection</param>
        void AddServices(IServiceCollection services);

        /// <summary>
        /// Configure App Services
        /// </summary>
        /// <param name="app">The Application Builder</param>
        void ConfigureServices(IServiceProvider app)
        {

        }
    }
}
