namespace KWFWebApi.Abstractions.Services
{ 
    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;

    public interface IServiceDefinition
    {
        void AddServices(IServiceCollection services);
        void ConfigureServices(IApplicationBuilder app);
    }
}
