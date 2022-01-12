namespace Sample.SampleApi
{
    using KWFCaching.Memory.Extensions;

    using KWFWebApi.Abstractions.Query;
    using KWFWebApi.Abstractions.Services;

    public class SampleServiceDefinitions : IServiceDefinition
    {
        private readonly IConfiguration _configuration;

        public SampleServiceDefinitions(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<WeatherForecastServices>();
            services.AddTransient<IQueryHandler<WeatherForecastQueryRequest, WeatherForecastQueryResponse>, WeatherForecastQueryHandler>();
            services.AddKwfCacheOnMemory(_configuration);
        }

        public void ConfigureServices(IApplicationBuilder app)
        {
        }
    }
}
